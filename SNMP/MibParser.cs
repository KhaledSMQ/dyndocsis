using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DocsisLibrary
{
    public struct MIB
    {
        public string Name;
        public int OID;
        public string Parent;
    }

    public class MibParser
    {
        public string MIBFilePath = Environment.GetEnvironmentVariable("mibs");
        public IList<MIB> MibList = new List<MIB>();

        public MibParser()
        {
        }

        public MibParser(string FilePath)
        {
            MIBFilePath = FilePath;
        }

        public void ParseMibFiles()
        {
            if (MIBFilePath==null)
                throw new Exception("Path to mib files cant be null!!!");
            var mibDirectory = new DirectoryInfo(MIBFilePath);
            FileInfo[] lstFileName = mibDirectory.GetFiles("*.*");
            var dasf = new StringBuilder();

            foreach (FileInfo fileName in lstFileName)
            {
                using (var flSteam = new FileStream(MIBFilePath + Path.DirectorySeparatorChar + fileName.Name, FileMode.Open))
                {

                    var mss = new byte[flSteam.Length];
                    flSteam.Read(mss, 0, (int) flSteam.Length);
                    flSteam.Close();
                    StringBuilder SearchName = new StringBuilder((new UTF8Encoding()).GetString(mss)); //"";

                    Match matchSNMP;
                    //@"([a-zA-Z0-9._%+-]+[ ]+)(OBJECT-TYPE|OBJECT IDENTIFIER|OBJECT-IDENTITY|MODULE-IDENTITY|OBJECT-GROUP|MODULE-COMPLIANCE|NOTIFICATION-TYPE|NOTIFICATION-GROUP)([ ]|[ ]+|[^.*$]+)::=.*?}",
                    Regex regSNMP =
                        new Regex(
                            @"((^[a-zA-Z0-9-]+).*)(OBJECT-TYPE|OBJECT IDENTIFIER|OBJECT-IDENTITY|MODULE-IDENTITY|OBJECT-GROUP|MODULE-COMPLIANCE|NOTIFICATION-TYPE|NOTIFICATION-GROUP)((.*[\n\r])*?)(.*::=.*?[.\r\n ]*?.*})",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline );
                    //    @"((^[a-zA-Z0-9-]+).*)(OBJECT-TYPE|OBJECT IDENTIFIER|OBJECT-IDENTITY|MODULE-IDENTITY|OBJECT-GROUP|MODULE-COMPLIANCE|NOTIFICATION-TYPE|NOTIFICATION-GROUP)([.\n\r]*?)(.*::=[\r\n ]*?.*})",
                        

                    for (matchSNMP = regSNMP.Match(SearchName.ToString()); matchSNMP.Success; matchSNMP = matchSNMP.NextMatch())
                    {
                        ParseMib(matchSNMP.ToString());
                    }
                    flSteam.Close();
                }
            }
            MIB test = new MIB();
            test.Name = "noAuthNoPriv";
            test.OID = 1;
            test.Parent = null;
            MibList.Add(test);
        }

        public void ParseMib(string val)
        {
            MIB NewMib;
            string Value = "";
            try
            {
                string Name = val.Substring(0, val.IndexOf(' '));
                NewMib.Name = Name.Trim();

                Value = val.Substring(val.IndexOf("::=") + 3);
                Value = Value.Substring(Value.IndexOf("{") + 1).Trim('}').Trim();
                NewMib.Parent = Value.Substring(0, Value.IndexOf(' '));
                NewMib.OID = Convert.ToInt32(Value.Substring(Value.IndexOf(' ')));
                MibList.Add(NewMib);
            }
            catch
            {
                NewMib.OID = 0;
            }
        }

        private string FindOID(string Value)
        {
            if (MibList.Count() == 0)
                ParseMibFiles();
            if (Value == "iso")
                return "1";
            IEnumerable<MIB> Mas = from c in MibList
                                   where c.Name == Value
                                   select c;
            if (Mas.Count() > 0)
            {
                MIB Nasao = Mas.First();
                return FindOID(Nasao.Parent) + "." + Nasao.OID;
            }
            else
                return "."+Value; // throw new Exception("No Find Exec");
        }


        private string FindMIB(List<string> Value, string parent)
        {
            if (MibList.Count() == 0)
                ParseMibFiles();
            int Val;
            Val = Convert.ToInt32(Value[0]);
            if (parent == null)
            {
                parent = "iso";
                Value.RemoveAt(0);
                return FindMIB(Value, parent);
            }
            else
            {
                IEnumerable<MIB> Mas = from c in MibList
                                       where c.OID == Val && c.Parent == parent
                                       select c;
                if (Mas.Count() > 0)
                {
                    MIB Nasao = Mas.First();
                    Value.RemoveAt(0);
                    if (Value.Count() >= 1)
                        return FindMIB(Value, Nasao.Name);
                    else
                        return Nasao.Name;
                }
            }
            string Result = "";
            foreach (string a in Value)
                Result += "." + a;
            return parent + Result; // throw new Exception("No Find Exec");
        }

        public string getOID(string Value)
        {

            var MList = new List<string>(Value.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries));
            string Result = "";
            foreach (string txtValue in MList)
            {
                int i1 = 0;
                if (!int.TryParse(txtValue, out i1))
                {
                    if (txtValue.Contains('\'') || txtValue.Contains('\"'))
                    {
                        foreach (char toValue in txtValue.Trim('\'').Trim('\"'))
                            Result += "." + (byte)toValue;
                    }
                    else
                        Result += FindOID(txtValue);
                }
                else
                    Result += "." + txtValue;
            }
            return Result;
        }

        public string getMib(string Value)
        {
            var MList = new List<string>(Value.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries));

            return FindMIB(MList, null);
        }
    }
}
