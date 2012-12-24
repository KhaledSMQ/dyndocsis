using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DocsisLibrary
{
    public class TLVCMTSMIC : TLV
    {
        public IList<object> Chiled = new List<object>();

        public int maxSize;
        public int minSize;

        public short[] order = new short[]
        { 1, 2, 3, 4, 17, 43, 6, 18, 19, 20, 22, 23, 24, 25, 28, 29, 26, 35, 36, 37, 40 };

        public string secret;

        //        public static implicit operator expectedLexerType(byte test)
        //        {
        //            return (int)test;
        //        }
        public TLVCMTSMIC()
        {

        }

        public TLVCMTSMIC(TLVList tlvs, String secret)
        {
            tlvLength = 0;
            tlvCode = 7;
            this.secret = secret;
            setValue2(tlvs);
        }

        public TLVCMTSMIC(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVCMTSMIC(byte Ident, byte[] tlvValue)
        {
            tlvCode = Ident;
            tlvLength = tlvValue.Length;
            this.tlvValue = tlvValue;
            //            expectedLexerType = Ident;
        }

        public override TLV Parse(Param tlvs, string secret)
        {
            throw new Exception("notImplementer yet");
            return this;
        }

        public override TLV Parse(Param name, byte[] Value)
        {
            //find tlvCode
            tlvName = name.Name;
            tlvValue = Value;
            return this;
        }
        public void setValue2(TLVList tlvs)
        {
            tlvInit = false;
            tlvValue = null;
            tlvLength = 0;

            if (tlvs == null || secret == null)
                return;
            var Encoder = new MemoryStream();
            var os = new BinaryWriter(Encoder);
            foreach (short or in order)
                for (int i = 0; i < tlvs.Count(); i++)
                    if (tlvs[i].tlvCode == or)
                        os.Write(tlvs[i].tlvValue);

            byte[] digest = hmacMd5(Encoder.ToArray(), Encoding.UTF8.GetBytes(secret));
            //BitConverter.GetBytes(this.secret));
            tlvValue = digest;
            tlvLength = digest.Length;
            tlvInit = true;
        }

        public override void setValue(byte[] encodings)
        {
        }

        public override void setValue(string Value)
        {
            throw new NotImplementedException();
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit || secret == null)
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

        private byte[] hmacMd5(byte[] data, byte[] key)
        {
            if (data == null || key == null)
                return null;

            MD5 md5Hasher = null;
            try
            {
                md5Hasher = MD5.Create();
            }
            catch (Exception e)
            {
                // Log.Message(LogLevel.ERROR,"ERROR: MD5 algorithm is not supported with JRE/JDK");
                // Log.Message(LogLevel.ERROR,e);
                return null;
            }

            // If key size > 64, reduce it through MD5
            byte[] ikey = null;
            if (key.Length > 64)
            {
                md5Hasher.ComputeHash(key, 0, key.Length);
                ikey = md5Hasher.Hash;
            }
            else
                ikey = key;
            var k_ipad = new byte[65];
            var k_opad = new byte[65];
            Array.Copy(ikey, k_ipad, ikey.Length);
            Array.Copy(ikey, k_opad, ikey.Length);

            for (byte i = 0; i < 64; i++)
            {
                k_ipad[i] ^= 0x36;
                k_opad[i] ^= 0x5c;
            }
            md5Hasher = MD5.Create();
            var asd = new byte[64 + data.Length];
            md5Hasher.TransformBlock(k_ipad, 0, 64, asd, 0);
            md5Hasher.TransformFinalBlock(data, 0, data.Length);
            byte[] digest = md5Hasher.Hash;
            md5Hasher = MD5.Create();
            asd = new byte[64 + digest.Length];
            md5Hasher.TransformBlock(k_opad, 0, 64, asd, 0);
            md5Hasher.TransformFinalBlock(digest, 0, digest.Length);
            digest = md5Hasher.Hash;

            return digest;
        }
    }
}
