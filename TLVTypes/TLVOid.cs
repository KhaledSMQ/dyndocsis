using System;

namespace DocsisLibrary
{
    public class TLVOid : TLVCSnmp
    {
        public int maxSize;
        public int minSize;

        public TLVOid()
        {

        }

        public TLVOid(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVOid(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVOid(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVOid(string name, string Value)
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
        }

        public override void setValue(byte[] Value)
        {
            tlvValue = new byte[Value.Length - 2];
            Array.Copy(Value, 2, tlvValue, 0, Value.Length - 2);
            //tlvValue = Value;
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit)
                return null;

            byte[] buffer = _tlvOID;

            var tmp = new byte[buffer.Length + 4];
            tmp[0] = getTlvCode();
            tmp[1] = (byte) (_tlvOID.Length + 2);
            tmp[2] = 6;
            tmp[3] = (byte) _tlvOID.Length;
            short i = 0;
            for (i = 0; i < buffer.Length; i++)
                tmp[i + 4] = buffer[i];

            return tmp;
        }

        public override string ToString()
        {
            string str = "";
            var arrayOfInt = new int[tlvValue.Length + 1];
            arrayOfInt[0] = (tlvValue[0] / 40);
            arrayOfInt[1] = (tlvValue[0] % 40);
            int m = 2;
            for (int z = 1; z < tlvValue.Length; ++z)
                arrayOfInt[m++] = tlvValue[z];

            for (int z1 = 0; z1 < arrayOfInt.Length; ++z1)
                str += "." + arrayOfInt[z1];
            return tlvName + " " + str + ";";
        }

    }
}
