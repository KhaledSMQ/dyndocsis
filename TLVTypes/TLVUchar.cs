using System;

namespace DocsisLibrary
{
    public class TLVUchar : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVUchar()
        {

        }

        public TLVUchar(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
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
        public TLVUchar(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVUchar(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVUchar(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            setValue(Value);
        }

        public override void setValue(string Value)
        {
            int Result = Convert.ToUInt16(Value);
            if (Result < 256)
                tlvValue = new byte[1] { (byte) Result };
            else
                throw new Exception("Format value " + Result + " is not char");
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
            string str = tlvValue[0].ToString();
            return tlvName + " " + str + ";";
        }


    }
}
