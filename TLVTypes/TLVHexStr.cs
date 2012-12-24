using System;
using System.IO;
using System.Linq;

namespace DocsisLibrary
{
    public class TLVHexStr : TLV
    {
        private const int MAXFILESIZE = 255;
        private Boolean isHex;
        public int maxSize;
        public int minSize;

        public TLVHexStr()
        {

        }

        public TLVHexStr(Param Test, string Value)
        {
            isHex = Value.StartsWith("0x");
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVHexStr(Param Test, byte[] Value)
        {
            tlvName = Test.Name;
            tlvCode = Test.TLV;
            minSize = Convert.ToInt32(Test.MinValue);
            minSize = Convert.ToInt32(Test.MaxValue);
            setValue(Value);
        }

        public TLVHexStr(string name, byte[] Value)
        {
            //find tlvCode
            tlvName = name;
            tlvValue = Value;
        }

        public TLVHexStr(string name, string Value)
        {
            isHex = Value.StartsWith("0x");
            //find tlvCode
            tlvName = name;
            setValue(Value);
        }
        public override TLV Parse(Param Test, string Value)
        {
            isHex = Value.StartsWith("0x");
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

        public override sealed void setValue(byte[] Value)
        {
            tlvValue = Value;
        }

        public override sealed void setValue(string Value)
        {
            Value = Value.Replace('"', ' ').Trim();
            if (isHex)
            {
                Value = Value.Substring(2);
                int NumberChars = Value.Length;
                var bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(Value.Substring(i, 2), 16);
                tlvValue = bytes;
            }
            else
            {
                if (File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + Value))
                {
                    using (
                    var file2Read = new FileStream(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + Value, FileMode.Open,
                    FileAccess.Read))
                    {
                        tlvValue = new byte[file2Read.Length];
                        int test = 0;
                        for (int position = 0; position < file2Read.Length; position++)
                        {
                            tlvValue[position] = (byte) file2Read.ReadByte();
                        }
                        file2Read.Close();
                    }
                }
                else
                {
                    throw new Exception("File can't be found");
                }
            }
        }

        public override byte[] ToEncoding()
        {
            if (!tlvInit)
                return null;
            int count = (tlvValue.Count() / MAXFILESIZE) + 1;
            var tmp = new byte[tlvValue.Length + (2 * count)];
            int position = 0;
            int newElement = 0;
            foreach (byte byteValue in tlvValue)
            {
                if (position == 0 || (position % (MAXFILESIZE + 2)) == 0)
                {
                    tmp[position++] = getTlvCode();
                    tmp[position++] =
                    (byte) (tlvLength - position > MAXFILESIZE ? MAXFILESIZE : tlvLength - (MAXFILESIZE * newElement));
                    newElement++;
                }
                tmp[position++] = byteValue;
            }
            return tmp;
        }

        public override string ToString()
        {
            string txtValue = "";
            for (int byteVal = 0; byteVal < tlvValue.Length; ++byteVal)
                txtValue = txtValue + ((int) (tlvValue[byteVal])).ToString("X2");
            return tlvName + " " + "0x" + txtValue + ";";
        }
    }
}
