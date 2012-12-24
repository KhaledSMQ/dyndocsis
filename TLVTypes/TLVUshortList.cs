using System;
using System.Linq;

namespace DocsisLibrary
{
    public class TLVUshortList : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVUshortList()
        {

        }

        public TLVUshortList(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVUshortList(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVUshortList(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVUshortList(string name, string Value)
        {
            //find tlvCode
            tlvName = name;
            setValue(Value);
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
            string[] Result = Value.Split(',');
            var btResult = new byte[Result.Count() * 2];
            int i = 0;
            foreach (string ch in Result)
            {
                int shortVal = Convert.ToInt32(ch);
                var buf = new byte[2];
                if (shortVal < 65536)
                {
                    btResult[i++] = (byte) (shortVal >> 8);
                    btResult[i++] = (byte) shortVal;
                }
                else
                    throw new Exception("Given Value " + Value + " is not Short Format");
            }
            tlvValue = btResult;
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
            string Result = "";
            int i = 0;
            while (i < tlvValue.Length)
            {
                var uShort = new byte[2];
                uShort[0] = tlvValue[i++];
                uShort[1] = tlvValue[i++];
                Result += ToUShort(uShort);
                if (i < tlvValue.Length)
                    Result += ",";
            }
            return tlvName + " " + Result + ";";
        }
    }
}
