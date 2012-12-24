using System;
using System.Linq;

namespace DocsisLibrary
{
    public class TLVEtherMask : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVEtherMask()
        {

        }

        public TLVEtherMask(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVEtherMask(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVEtherMask(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVEtherMask(string name, string Value)
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
            if (Value.Contains("/"))
            {
                string[] ResultEt = Value.Split('/');
                int NumberChars = ResultEt.Count();
                var bytes = new byte[6 * NumberChars];
                int i = 0;
                foreach (string Ether in ResultEt)
                {
                    if (Ether.Contains(":"))
                    {
                        string[] Result = Ether.Split(':');

                        foreach (string st in Result)
                            bytes[i++] = Convert.ToByte(st, 16);
                    }
                }
                tlvValue = bytes;
            }
            else
                throw new Exception("Value is Not Regulat Ether Address");
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
            string str = "";
            for (int j = 0; j < tlvValue.Length; ++j)
            {
                str = str + ((int) (tlvValue[j])).ToString("X2");
                if (j == 5)
                    str += "/";
                else
                    str += ":";
            }
            return tlvName + " " + str.Substring(0, str.Length - 1) + ";";
        }
    }
}
