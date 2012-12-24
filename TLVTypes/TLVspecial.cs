using System;
using System.Text;

namespace DocsisLibrary
{
    public class TLVSpecial : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVSpecial()
        {
            //find tlvCode
            tlvName = "EndOfDataMkr";
            tlvCode = 255;
            tlvValue = new byte[] { 0, 0 };
            ;
        }

        public TLVSpecial(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVSpecial(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVSpecial(byte Code, byte[] Value)
        {
            //find tlvCode
            tlvName = "EndOfDataMkr";
            tlvCode = Code;
            tlvValue = Value;
        }

        public TLVSpecial(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVSpecial(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = (new UTF8Encoding()).GetBytes(Value);
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

            var tmp = new byte[buffer.Length + 1];
            tmp[0] = getTlvCode();
            for (short i = 0; i < buffer.Length; i++)
                tmp[i + 1] = buffer[i];

            return tmp;
        }

        public override string ToString()
        {
            return tlvName + "";
        }
    }
}
