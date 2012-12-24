using System;
using System.Text;

namespace DocsisLibrary
{
    public class TLVNothing : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVNothing()
        {

        }

        public TLVNothing(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVNothing(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVNothing(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVNothing(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = (new UTF8Encoding()).GetBytes(Value);
        }

        public override TLV Parse(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
            return this;
        }
        public override TLV Parse(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
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
            return tlvName + " " + "";
        }
    }
}
