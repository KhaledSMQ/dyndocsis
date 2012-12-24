using System;
using System.Security.Cryptography;

namespace DocsisLibrary
{
    public class TLVCMMIC : TLV
    {
        public int maxSize;
        public int minSize;

        //        public static implicit operator expectedLexerType(byte test)
        //        {
        //            return (int)test;
        //        }
        public TLVCMMIC()
        {

        }

        public TLVCMMIC(TLVList tlvEncodings)
        {
            tlvLength = 0;
            tlvCode = 6;
            if (tlvEncodings == null)
                tlvValue = null;
            setValue(tlvEncodings);
        }

        public TLVCMMIC(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVCMMIC(byte Ident, byte[] tlvValue)
        {
            tlvCode = Ident;
            tlvLength = tlvValue.Length;
            this.tlvValue = tlvValue;
            //            expectedLexerType = Ident;
        }

        public override TLV Parse(Param name, string Value)
        {
            tlvLength = 0;
            tlvCode = 6;
            if (Value == null)
                tlvValue = null;
            setValue(Value);
            return this;
        }
        public override TLV Parse(Param name, byte[] Value)
        {
            //find tlvCode
            tlvName = name.Name;
            tlvValue = Value;
            return this;
        }

        public override void setValue(byte[] tlvEncodings)
        {
        }

        public void setValue(TLVList tlvEncodings)
        {
            tlvInit = false;
            tlvValue = null;
            tlvLength = 0;

            if (tlvEncodings == null)
                return;
            MD5 md5Hasher = MD5.Create();
            byte[] Master = tlvEncodings.GetEncodings();
            md5Hasher.ComputeHash(Master, 0, Master.Length);
            tlvValue = md5Hasher.Hash;
            string sResult = BitConverter.ToString(tlvValue);
            tlvLength = tlvValue.Length;
            tlvInit = true;
        }

        public override void setValue(string Value)
        {
            throw new NotImplementedException();
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit)
                return null;

            var tmp = new byte[tlvLength + 2];
            tmp[0] = getTlvCode();
            tmp[1] = (byte) tlvLength;
            for (short i = 0; i < tlvLength; i++)
                tmp[i + 2] = tlvValue[i];

            return tmp;
        }

        public override string ToString()
        {
            return tlvName + " ";
        }
    }
}
