using System;

namespace DocsisLibrary
{
    public class TLVSnmp : TLVCSnmp
    {
        public int maxSize;
        public int minSize;

        public TLVSnmp()
        {

        }


        public TLVSnmp(Param Test, string Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVSnmp(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Convert.ToInt32(Test.TLV);
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVSnmp(string strTLVName, byte[] Value)
        {
            //find tlvCode
            tlvName = strTLVName;
            setValue(Value);
        }

        public TLVSnmp(string strTLVName, string Value)
        {
            //find tlvCode
            tlvName = strTLVName;
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
            tlvOID = Value.Substring(0, Value.IndexOf(' '));
            Value = Value.Substring(Value.IndexOf(' ') + 1);
            tlvSNMPType = Value.Substring(0, Value.IndexOf(' '));
            this.Value = Value.Substring(Value.IndexOf(' ') + 1).Trim();
            ;
        }

        public override void setValue(byte[] Value)
        {
            int i4 = 0;
            int i5 = 0;
            byte[] arrayOfByte2 = { 0 };
            byte[] arrayOfByte3 = { 0 };
            if (Value[1] != 130)
            {
                i4 = 2;
                i5 = Value[1];
            }
            else
            {
                i4 = 4;
                i5 = (Value[2] << 8) | Value[3];
            }
            arrayOfByte2 = new byte[i5];
            int i6 = i4;
            for (int i7 = 0; i7 < i5; ++i7)
            {
                if (i6 >= Value.Length)
                    break;
                arrayOfByte2[i7] = Value[i6];
                ++i6;
            }
            int i = 0;
            try
            {
                while (i < arrayOfByte2.Length)
                {
                    int j = arrayOfByte2[(i++)];
                    byte[] arrayOfByte = { 0 };

                    int k = 0;
                    if (arrayOfByte2[i++] != 130)
                    {
                        k = arrayOfByte2[(i - 1)];
                    }
                    else
                    {
                        var test = new byte[2];
                        test[0] = arrayOfByte2[i++];
                        test[1] = arrayOfByte2[i++];
                        string asd = ToInteger32(test);
                        k = Convert.ToInt32(asd);
                    }
                    arrayOfByte = new byte[k];
                    for (int l = 0; l < k; ++l)
                    {
                        if (i >= arrayOfByte2.Length)
                            break;
                        arrayOfByte[l] = arrayOfByte2[(i++)];
                    }

                    switch (j)
                    {
                        case 6:
                            _tlvOID = arrayOfByte;
                            break;
                        default:
                            tlvSNMPType = (byte) j;
                            tlvValue = arrayOfByte;
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        public byte[] Parse(byte Code, byte[] ByteArray)
        {
            int pos;
            byte[] tmp;
            if ((ByteArray.Length) > 254)
            {
                tmp = new byte[ByteArray.Length + 4];
                tmp[0] = Code;
                tmp[1] = 130;
                tlvCode = 64;
                int i5 = ByteArray.Length;
                tmp[2] = (byte) (i5 >> 8);
                //Da li ovde ide 1 ili zavisno od velicine koja ide uz string ushort ili tako nesto
                tmp[3] = (byte) i5;
                pos = 4;
            }
            else
            {
                tmp = new byte[ByteArray.Length + 2];
                tmp[0] = Code;
                tmp[1] = (byte) ByteArray.Length;
                pos = 2;
            }

            for (short i = 0; i < ByteArray.Length; i++)
                tmp[i + pos] = ByteArray[i];

            return tmp;
        }

        public override byte[] ToEncoding()
        {
            byte[] buffer;
            byte[] buffer2;

            if (!tlvInit)
                throw new Exception("Valid is not set in correct Value");
            if (tlvValue.Length > 255)
                tlvCode = 64;
            buffer = Parse(6, _tlvOID);
            buffer2 = Parse(_tlvSNMPType, tlvValue);

            // oid+typesnmp+buffer
            var tmp = new byte[buffer.Length + buffer2.Length];
            int i6 = 0;
            for (i6 = 0; i6 < buffer.Length; i6++)
                tmp[i6] = buffer[i6];
            for (int i7 = 0; i7 < buffer2.Length; i7++)
                tmp[i6++] = buffer2[i7];
            buffer = Parse(48, tmp);
            if (tlvCode == 64)
            {
                tmp = new byte[buffer.Length + 3];
                tmp[0] = (byte) tlvCode;
                tmp[1] = (byte) (buffer.Length >> 8);
                //Da li ovde ide 1 ili zavisno od velicine koja ide uz string ushort ili tako nesto
                tmp[2] = (byte) buffer.Length;
                i6 = 3;
            }
            else
            {
                tmp = new byte[buffer.Length + 2];
                tmp[0] = (byte) tlvCode;
                tmp[1] = (byte) buffer.Length;
                i6 = 2;
            }
            for (int i9 = 0; i9 < buffer.Length; i9++)
                tmp[i9 + i6] = buffer[i9];
            return tmp;
        }

        public override string ToString()
        {
            return tlvName + " " + DecodeFile.MibFileParese.getMib(tlvOID) + " " + tlvSNMPType + " " + Value + ";";
        }
    }
}
