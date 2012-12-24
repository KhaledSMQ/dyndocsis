using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocsisLibrary
{
    [Serializable]
    public class TLVList : IEnumerable
    {
        private readonly List<TLV> _tlvMembers;

        public TLVList()
        {
            _tlvMembers = new List<TLV>();
        }

        public TLVList(List<TLV> pArray)
        {
            _tlvMembers = pArray;
        }

        public TLV this[int Option]
        {
            get
            {
                return _tlvMembers[Option];
            }
            set
            {
                _tlvMembers[Option] = value;
            }
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public TLVEnum GetEnumerator()
        {
            return new TLVEnum(_tlvMembers);
        }

        public byte[] GetEncodings()
        {
            var memEncoder = new MemoryStream();
            var binWriter = new BinaryWriter(memEncoder);
            foreach (TLV tlvMember in _tlvMembers)
                binWriter.Write(tlvMember.ToEncoding());
            return memEncoder.ToArray();
        }

        public int Count()
        {
            return _tlvMembers.Count();
        }

        public void Remove(TLV tlvMember)
        {
            _tlvMembers.Remove(tlvMember);
        }

        public void Add(TLV tlvMember)
        {
            _tlvMembers.Add(tlvMember);
        }

        #region Nested type: TLVEnum

        public class TLVEnum : IEnumerator
        {
            public List<TLV> _tlvMembers;

            // Enumerators are positioned before the first element
            // until the first MoveNext() call.
            private int position = -1;

            public TLVEnum(List<TLV> list)
            {
                _tlvMembers = list;
            }

            public TLV Current
            {
                get
                {
                    try
                    {
                        return _tlvMembers[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            #region IEnumerator Members

            public bool MoveNext()
            {
                position++;
                return (position < _tlvMembers.Count);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            #endregion
        }

        #endregion
    }

    public abstract class TLV
    {
        public int _tlvLength;
        public TLVList children = new TLVList();

        public int expectedLexerType;

        //    private CommonTree node = null;
        public TLV parent;
        public int[] parentTLV;
        public int tlvCode = -1;
        public Boolean tlvInit = true;
        public string tlvName;
        public byte[] tlvValue;

        public int tlvLength
        {
            get
            {
                return tlvValue.Length;
            }
            set
            {
                _tlvLength = value;
            }
        }

        public Boolean processNode()
        {
            //node = node;
            return true;
        }

        public byte getTlvCode()
        {
            return (byte) tlvCode;
        }

        public string getTlvCodeString()
        {
            if (parent != null)
                return parent.getTlvCode() + "." + tlvCode;
            else
                return tlvCode.ToString();
        }

        public int getTLVLength()
        {
            return tlvLength;
        }

        public TLV setParent(TLV parent)
        {
            this.parent = parent;
            return this;
        }

        public abstract TLV Parse(Param name, string Context);
        public abstract TLV Parse(Param name, byte[] Context);

        public abstract byte[] ToEncoding();

        public abstract new string ToString();

        public static string ToString(byte[] Value)
        {
            return "\"" + Encoding.ASCII.GetString(Value) + "\"";
        }

        public abstract void setValue(string Value);
        public abstract void setValue(byte[] Value);

        public static string ToUShort(byte[] Value)
        {
            long l = (Value[Value.Length - 1]);
            for (int k = Value.Length - 2; k >= 0; --k)
            {
                l = (l + (Value[k]) * 256);
            }
            return l.ToString();
        }

        public static string ToInteger32(byte[] Value)
        {
            long result = ((Value[0] & 0x80) == 0x80) ? -1 : 0; // sign extended! Guy McIlroy
            for (int js = 0; js < Value.Length; js++)
            {
                result = (result << 8) | Value[js];
            }
            return result.ToString();
        }
        public static string ToHexString(byte[] Value)
        {
            string str = "";
            for (int j = 0; j < Value.Length; ++j)
                str = str + ((int) (Value[j])).ToString("X2");
            return "0x" + str;
        }

        private static char ToHexChar(int b)
        {
            const int ascii_zero = 48;
            const int ascii_a = 65;

            if (b >= 0 && b <= 9)
            {
                return (char) (b + ascii_zero);
            }
            if (b >= 10 && b <= 15)
            {
                return (char) (b + ascii_a - 10);
            }
            return '?';
        }

        public static string ToHexString(int b)
        {
            const int mask1 = 0x0F;
            const int mask2 = 0xF0;

            string ret = "";

            int c1 = (b & mask1);
            int c2 = (b & mask2) >> 4;

            ret = ret + ToHexChar(c2) + ToHexChar(c1);
            return ret;
        }

        public static TLV Create(Param name, byte[] Context)
        {
            if (name == null)
                throw new Exception("Can't Find Symbol decoder");
            var sample = ((TLV)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("DocsisLibrary.TLV" + name.Decode, true));
            if (sample == null)
                throw new Exception("Can't Find Symbol decoder");
            return sample.Parse(name, Context);
        }

        public static TLV Create(Param name, string Context)
        {
            if (name == null)
                throw new Exception("Can't Find Symbol decoder");
            var sample = ((TLV)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("DocsisLibrary.TLV" + name.Decode, true));
            if (sample == null)
                throw new Exception("Can't Find Symbol decoder");
            return sample.Parse(name, Context);
        }
    }
}
