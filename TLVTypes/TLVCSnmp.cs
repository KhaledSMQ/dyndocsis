using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace DocsisLibrary
{
    public abstract class TLVCSnmp : TLV
    {
        public byte[] _tlvOID;

        public byte _tlvSNMPType;

        public string Value
        {
            get
            {
                switch ((SnmpType) _tlvSNMPType)
                {
                    case SnmpType.Integer32:
                        return ToInteger32(tlvValue);
                    case SnmpType.BitString:
                        return ToHexString(tlvValue);
                    case SnmpType.OctetString:
                        return ToString(tlvValue);
                    case SnmpType.Counter32:
                        return ToInteger32(tlvValue);
                    case SnmpType.Gauge32:
                        return ToCounter32(tlvValue);
                    case SnmpType.IPAddress:
                        return ToIPAddress4(tlvValue);
                    case SnmpType.TimeTicks:
                        return ToCounter32(tlvValue);
                    default:
                        return "";
                }
            }
            set
            {
                switch ((SnmpType) _tlvSNMPType)
                {
                    case SnmpType.Integer32:
                        int intVal2 = Convert.ToInt32(value);
                        if (intVal2 > 255)
                        {
                            tlvValue = new byte[3];
                            tlvValue[2] = (byte) (intVal2);
                            tlvValue[1] = (byte) (intVal2 >> 8);
                            tlvValue[0] = (byte) (intVal2 >> 16);
                        }
                        else
                            tlvValue = new byte[1] { (byte) (intVal2) };

                        break;
                    case SnmpType.BitString:
                        value = value.Substring(value.IndexOf('x') + 1);
                        int NumberChars = value.Length;
                        tlvValue = new byte[NumberChars / 2];
                        for (int i = 0; i < NumberChars; i += 2)
                        {
                            tlvValue[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
                        }
                        break;
                    case SnmpType.OctetString:
                        tlvValue = (new UTF8Encoding()).GetBytes(value.Trim('\"').Trim('\''));
                        break;
                    case SnmpType.Gauge32:
                        uint intVal = Convert.ToUInt32(value);
                        tlvValue = new byte[3];
                        tlvValue[2] = (byte) intVal;
                        tlvValue[1] = (byte) (intVal >> 8);
                        tlvValue[0] = (byte) (intVal >> 16);
                        //tlvValue[0] = (byte)(intVal >> 24);
                        break;
                    case SnmpType.TimeTicks:
                    case SnmpType.Counter32:
                        uint intVal22 = Convert.ToUInt32(value);
                        tlvValue = new byte[4];
                        tlvValue[3] = (byte) intVal22;
                        tlvValue[2] = (byte) (intVal22 >> 8);
                        tlvValue[1] = (byte) (intVal22 >> 16);
                        tlvValue[0] = (byte) (intVal22 >> 24);
                        break;
                    case SnmpType.IPAddress:
                        tlvValue = IPAddress.Parse(value).GetAddressBytes();
                        break;
                    /*                    case SnmpType.Gauge32:
                                                                                                                            tlvValue = (byte[])BitConverter.GetBytes(Convert.ToInt32(value));
                                                                                                                            break;
                                                                                                                        case SnmpType.TimeTicks:
                                                                                                                            tlvValue = BitConverter.GetBytes(Convert.ToInt32(value));
                                                                                                                            break;
                                                                                                    */
                    default:
                        tlvValue = (new UTF8Encoding()).GetBytes(value);
                        break;
                }
            }
        }

        public object tlvSNMPType
        {
            get
            {
                switch ((SnmpType) _tlvSNMPType)
                {
                    case SnmpType.BitString:
                        return "HexString";
                        ;
                    case SnmpType.Boolean:
                        return "Boolean";
                    case SnmpType.Integer32:
                        return "Integer";
                    case SnmpType.OctetString:
                        return "String";
                    case SnmpType.Counter32:
                        return "Counter32";
                    case SnmpType.Gauge32:
                        return "Gauge32";
                    case SnmpType.IPAddress:
                        return "IPAddress";
                    case SnmpType.TimeTicks:
                        return "TimeTicks";
                    default:
                        throw new Exception("Cent find type you looking for");
                }
            }
            set
            {
                if (value.GetType() == typeof (byte))
                {
                    _tlvSNMPType = (byte) value;
                }
                else
                {
                    switch (((string) value).ToLower())
                    {
                        case "hexstring":
                            _tlvSNMPType = (byte) SnmpType.BitString;
                            break;
                        case "boolean":
                            _tlvSNMPType = (byte) SnmpType.Boolean;
                            break;
                        case "integer":
                        case "integer32":
                            _tlvSNMPType = (byte) SnmpType.Integer32;
                            break;
                        case "string":
                            _tlvSNMPType = (byte) SnmpType.OctetString;
                            break;
                        case "counter":
                        case "counter32":
                            _tlvSNMPType = (byte) SnmpType.Counter32;
                            break;
                        case "gauge":
                        case "gauge32":
                            _tlvSNMPType = (byte) SnmpType.Gauge32;
                            break;
                        case "ipaddress":
                            _tlvSNMPType = (byte) SnmpType.IPAddress;
                            break;
                        case "timeticks":
                            _tlvSNMPType = (byte) SnmpType.TimeTicks;
                            break;
                        default:
                            //throw new Exception("Cent find type you looking for");
                            break;
                    }
                }
            }
        }

        public string tlvOID
        {
            get
            {
                var str = new StringBuilder();
                var arrayOfInt = new int[_tlvOID.Length + 1];
                arrayOfInt[0] = (_tlvOID[0] / 40);
                arrayOfInt[1] = (_tlvOID[0] % 40);
                int m = 2;
                for (int z = 1; z < _tlvOID.Length; ++z)
                    arrayOfInt[m++] = _tlvOID[z];

                for (int z1 = 0; z1 < arrayOfInt.Length; ++z1)
                {
                    if (arrayOfInt[z1] < 128)
                        str.AppendFormat(".{0}", arrayOfInt[z1]);
                    else
                    {
                        str.AppendFormat(".{0}",
                        ((arrayOfInt[z1++] - 128) * 128) + arrayOfInt[z1]);
                    }
                }
                return str.ToString();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("dotted");
                }
                value = DecodeFile.MibFileParese.getOID(value);
                string[] parts = value.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new uint[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    try
                    {
                        result[i] = uint.Parse(parts[i], CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw new ExecutionEngineException("Mib parser is not translated, missing MIB files for: "+ parts[i]);
                    }
                }


                var temp = new List<byte>();
                var first = (byte) ((40 * result[0]) + result[1]);
                temp.Add(first);
                for (int i = 2; i < result.Length; i++)
                {
                    temp.AddRange(ConvertToBytes(result[i]));
                }

                _tlvOID = temp.ToArray(); // stream, TypeCode, temp.ToArray());
            }
        }

        private static IEnumerable<byte> ConvertToBytes(uint subIdentifier)
        {
            var result = new List<byte>();
            result.Add((byte) (subIdentifier & 0x7F));
            while ((subIdentifier = subIdentifier >> 7) > 0)
            {
                result.Add((byte) ((subIdentifier & 0x7F) | 0x80));
            }

            result.Reverse();
            return result;
        }


        public static string ToCounter32(byte[] Value)
        {
            var list = new List<byte>(Value);
            list.Reverse();
            while (list.Count > 4)
            {
                list.RemoveAt(list.Count - 1);
            }

            while (list.Count < 4)
            {
                list.Add(0);
            }


            return BitConverter.ToUInt32(list.ToArray(), 0).ToString(); // paramArrayOfByte,0).ToString();
        }

        public static string T2oInteger64(byte[] Value)
        {
            long result = ((Value[0] & 0x80) == 0x80) ? -1 : 0; // sign extended! Guy McIlroy

            for (int js = 0; js < Value.Length; js++)
            {
                result = (result << 8) | Value[js];
            }
            return result.ToString();
        }

        public static string ToCounter64(byte[] Value)
        {
            return BitConverter.ToUInt64(Value, 0).ToString();
        }

        public static string ToIPAddress4(byte[] Value)
        {
            return new IPAddress(Value).ToString();
        }
    }
}
