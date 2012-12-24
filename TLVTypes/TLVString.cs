using System;
using System.Text;

namespace DocsisLibrary
{
    public class TLVString : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVString()
        {

        }

        public TLVString(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVString(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVString(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVString(string name, string Value)
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
            if (Value[0] == '\"')
                Value = Value.Trim().Substring(1, Value.Length - 2);
            tlvValue = (new UTF8Encoding()).GetBytes(Value);
        }

        public override void setValue(byte[] Value)
        {
            tlvValue = Value;
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
            try
            {
                return tlvName + " " + "\"" + Encoding.ASCII.GetString(tlvValue) + "\";";
            }
            catch
            {
                return tlvName + " " + ";";
            }
        }
    }
}
