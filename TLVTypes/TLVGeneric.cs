using System;
using System.Text;

namespace DocsisLibrary
{
    public class TLVGeneric : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVGeneric()
        {

        }

        public TLVGeneric(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVGeneric(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVGeneric(string name, int Code, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvCode = Code;
            tlvValue = Value;
        }

        public TLVGeneric(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVGeneric(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            setValue(Value);
        }

        public override TLV Parse(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
            return this;
        }
        public override TLV Parse(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
            return this;
        }
        public override void setValue(string Value)
        {
            string[] Result = Value.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int Length = 0;
            for (int i = 0; i < Result.Length; i += 2)
            {
                if (Result[i].Equals("TlvCode", StringComparison.InvariantCultureIgnoreCase))
                    tlvCode = Convert.ToInt32(Result[i + 1]);
                if (Result[i].Equals("TlvLenght", StringComparison.InvariantCultureIgnoreCase))
                    Length = Convert.ToInt32(Result[i + 1]);
                if (Result[i].Equals("TlvHexStr", StringComparison.InvariantCultureIgnoreCase) ||
                Result[i].Equals("TlvValue", StringComparison.InvariantCultureIgnoreCase))
                {
                    string HexStr = Result[i + 1];
                    if (HexStr.Contains("0x"))
                        HexStr = HexStr.Substring(2);
                    int NumberChars = HexStr.Length;
                    var bytes = new byte[NumberChars / 2];
                    for (int i2 = 0; i2 < NumberChars; i2 += 2)
                        bytes[i2 / 2] = Convert.ToByte(HexStr.Substring(i2, 2), 16);
                    tlvValue = bytes;
                }
                if (Result[i].Equals("TlvString", StringComparison.InvariantCultureIgnoreCase))
                    tlvValue = (new UTF8Encoding()).GetBytes(Result[i + 1].Trim('"'));
                if (Result[i].Equals("TlvStringZero", StringComparison.InvariantCultureIgnoreCase))
                    tlvValue = (new UTF8Encoding()).GetBytes(Result[i + 1].Trim('"'));
            }
        }

        public override void setValue(byte[] Value)
        {
            throw new NotImplementedException();
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit)
                return null;

            byte[] buffer = tlvValue;

            var tmp = new byte[buffer.Length + 2];
            tmp[0] = getTlvCode();
            tmp[1] = (byte) tlvLength;

            for (short i = 0; i < buffer.Length; i++)
                tmp[i + 2] = buffer[i];
            return tmp;
        }

        public override string ToString()
        {
            if (tlvLength > 1 && char.IsLetterOrDigit((char)tlvValue[1]))
                return tlvName + " " + "TlvCode " + tlvCode + " TlvString \"" + Encoding.ASCII.GetString(tlvValue) +
                "\"; /* TlvLenght " + tlvLength + " */";
            else
                if (tlvLength > 1 &&
                tlvValue[tlvLength - 2] == 0 &&
                char.IsLetterOrDigit((char) tlvValue[tlvLength - 2]))
                    return tlvName + " " + "TlvCode " + tlvCode + " TlvStringZero \"" +
                    Encoding.ASCII.GetString(tlvValue) + "\"; /* TlvLenght " + tlvLength + " */";
                else
                    return tlvName + " " + "TlvCode " + tlvCode + " TlvLength " + tlvLength + " TlvValue " +
                    ToHexString(tlvValue) + "; /* TlvLenght " + tlvLength + " */";
        }
    }
}
