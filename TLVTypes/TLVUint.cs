using System;

namespace DocsisLibrary
{
    public class TLVUint : TLVCSnmp
    {
        public int maxSize;
        public int minSize;

        public TLVUint()
        {

        }

        public TLVUint(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            try
            {
                minSize = Convert.ToInt32(Test.MinValue);
                minSize = Convert.ToInt32(Test.MaxValue);
            }
            catch
            {
            }
            setValue(Value);
        }

        public TLVUint(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            try
            {
                minSize = Convert.ToInt32(Test.MinValue);
                minSize = Convert.ToInt32(Test.MaxValue);
            }
            catch
            {
            }
            setValue(Value);
        }

        public TLVUint(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVUint(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            setValue(Value);
        }

        public override TLV Parse(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            try
            {
                minSize = Convert.ToInt32(Test.MinValue);
                minSize = Convert.ToInt32(Test.MaxValue);
            }
            catch
            {
            }
            setValue(Value);
            return this;
        }
        public override TLV Parse(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            try
            {
                minSize = Convert.ToInt32(Test.MinValue);
                minSize = Convert.ToInt32(Test.MaxValue);
            }
            catch
            {
            }
            setValue(Value);
            return this;
        }
        public override void setValue(string Value)
        {
            uint intVal = Convert.ToUInt32(Value);
            tlvValue = new byte[4];
            tlvValue[3] = (byte) intVal;
            tlvValue[2] = (byte) (intVal >> 8);
            tlvValue[1] = (byte) (intVal >> 16);
            tlvValue[0] = (byte) (intVal >> 24);
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
            int size = 0;
            size = ((tlvValue[0] * 256 + tlvValue[1]) * 256 + tlvValue[2]) * 256 + tlvValue[3];
            return tlvName + " " + size.ToString() + ";";
        }


    }
}
