using System;

namespace DocsisLibrary
{
    public class TLVSnmpWd : TLVCSnmp
    {
        public int maxSize;
        public int minSize;

        public TLVSnmpWd()
        {

        }

        public TLVSnmpWd(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }
        public TLVSnmpWd(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVSnmpWd(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVSnmpWd(string name, string Value)
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
            //Catch SNMPType & Value //Check valid Min MAx Value
            //What To do with string on the end OID convert in what
            tlvValue = new byte[] { 0 };
            string[] getVal = Value.Split(' ');
            tlvOID = getVal[0];
            tlvSNMPType = (byte) SnmpType.Integer32;
            this.Value = getVal[1];
        }

        public override void setValue(byte[] Value)
        {
            tlvValue = Value;
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit)
                return null;

            byte[] buffer = _tlvOID;

            var tmp = new byte[buffer.Length + 5];
            tmp[0] = getTlvCode();
            tmp[1] = (byte) (buffer.Length + tlvLength + 2);
            tmp[2] = 6;
            tmp[3] = (byte) _tlvOID.Length;
            short i = 0;
            for (i = 0; i < buffer.Length; i++)
                tmp[i + 4] = buffer[i];
            tmp[i + 4] = tlvValue[0];

            return tmp;
        }

        public override string ToString()
        {
            try
            {
                string str = "";
                int Len = tlvValue[1];
                var arrayOfInt = new int[Len + 1];
                arrayOfInt[0] = (tlvValue[2] / 40);
                arrayOfInt[1] = (tlvValue[2] % 40);
                int m = 2;
                for (int z = 3; z < Len + 2; ++z)
                    arrayOfInt[m++] = tlvValue[z];

                for (int z1 = 0; z1 < arrayOfInt.Length; ++z1)
                    str += "." + arrayOfInt[z1];
                str += " ";

                return tlvName + " " + str + tlvValue[5] + ";";
            }
            catch
            {
                return " ;";
            }
        }
    }
}
