using System;

namespace DocsisLibrary
{
    public class TLVUshort : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVUshort()
        {

        }

        public TLVUshort(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVUshort(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVUshort(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVUshort(string name, string Value)
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
        public override void setValue(byte[] Value)
        {
            tlvValue = Value;
        }

        public override void setValue(string Value)
        {
            var buf = new byte[2];
            int i = Convert.ToInt32(Value);

            if (i < 65536)
            {
                buf[1] = (byte) i;
                buf[0] = (byte) (i >> 8);
            }
            else
                throw new Exception("Given Value " + Value + " is not Short Format");
            tlvValue = buf;
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
            long l = (tlvValue[tlvValue.Length - 1]);
            for (int k = tlvValue.Length - 2; k >= 0; --k)
            {
                l = (l + (tlvValue[k]) * 256);
            }
            return tlvName + " " + l.ToString() + ";";
        }


    }
}
