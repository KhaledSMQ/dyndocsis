using System;
using System.Text;

namespace DocsisLibrary
{
    public class TLVAggregate : TLV
    {
        public int maxSize;
        public int minSize;

        public TLVAggregate()
        {

        }

        public TLVAggregate(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVAggregate(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVAggregate(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVAggregate(string name, string Value)
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
            tlvValue = null;
            //if ((byte) tlvCode == 43)
                //TODO: What is this : //Console.WriteLine(23);
            if (children.Count() > 0)
            {
                int len = 0;
                foreach (TLV a in children)
                {
                    byte[] getVal = a.ToEncoding();
                    Array.Resize(ref tlvValue, len + getVal.Length);
                    Array.Copy(getVal, 0, tlvValue, len, getVal.Length);
                    len += getVal.Length;
                }
            }
            byte[] buffer = tlvValue;

            var tmp = new byte[buffer.Length + 2];
            tmp[0] = getTlvCode();
            tmp[1] = (byte) tlvLength;
            if ((byte) tlvLength > 255)
                throw new Exception("Data to long mast be less then 255");

            for (short i = 0; i < buffer.Length; i++)
                tmp[i + 2] = buffer[i];

            return tmp;
        }

        public override string ToString()
        {
            string space = "";
            TLV par = this;
            while (true)
            {
                par = par.parent;
                if (par == null)
                    break;
                space += "\t";
                
            }
            string str = space + tlvName;
            if (children.Count() > 0)
            {
                str += "\n" + space + "\t{\n";
                foreach (TLV a in children)
                    str += space + (a.children.Count() > 0 ? "\t" : "\t\t") + a.ToString() + "\n";
                str += space + "\t}";
            }
            else
                str += ";";
            return str;
        }
    }
}
