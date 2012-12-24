using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DocsisLibrary
{
    public class EncodeFile
    {
        #region OnDiscoverEventHandler

        #region Delegates

        public delegate void onDebugEventHandler(ref TLV tlvData);

        #endregion

        public event onDebugEventHandler DebugEventHandler;

        /// <summary>
        /// Triggers the DiscoverEventHandler event.
        /// </summary>
        public virtual void DebugEvent(ref TLV tlvData)
        {
            if (DebugEventHandler != null)
                DebugEventHandler(ref tlvData);
        }

        #endregion
        //Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
        public static string MIBFilePath = Environment.GetEnvironmentVariable("mibs");

        public static MibParser MibFileParese = new MibParser(MIBFilePath);
        private readonly SymbolTable itemSymbolTable = null;
        private byte[] BinaryFile;
        private TLVList EncodedFile;
        private string LastCommand = "";
        private string _secret = "DOCSIS";

        public EncodeFile()
        {
            itemSymbolTable = LoadSymbolTable();
        }

        public EncodeFile(string FileName)
        {
            itemSymbolTable = LoadSymbolTable();
            MIBFilePath = FileName;
            MibFileParese = new MibParser(MIBFilePath);
        }

        internal SymbolTable LoadSymbolTable()
        {

            if (File.Exists("docsis_symtable.xml"))
            {
                using (var fileStream = new FileStream("docsis_symtable.xml", FileMode.Open, FileAccess.Read))
                {
                    var formatter = new XmlSerializer(typeof (SymbolTable));
                    try
                    {
                        return (SymbolTable) formatter.Deserialize(fileStream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't Read Symbol table file !!!");
                    }
                }
            }
            else
            {
                using (
                Stream CertStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                @"DocsisLibrary.DocsisSymbols.docsis_symtable.xml"))
                {
                    var formatter = new XmlSerializer(typeof (SymbolTable));
                    try
                    {
                        return (SymbolTable)formatter.Deserialize(CertStream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't Read Symbol table file !!!");
                    }
                }
            }
        }

        public string Secret
        {
            get
            {
                return _secret;
            }
            set
            {
                _secret = value;
            }
        }

        public byte[] Encode2File(string FileName, string strSecret)
        {
            EncodedFile = null;
            BinaryFile = null;
            Secret = strSecret;
            var MS = new FileStream(FileName, FileMode.Open);
            BinaryFile = new byte[MS.Length];
            MS.Read(BinaryFile, 0, (int) MS.Length);
            EncodedFile = Encode(null, (new UTF8Encoding()).GetString(BinaryFile), itemSymbolTable);
            return ToEncoding();
        }

        public void Encode2File(string FileName, string strSecret, string ToFileName)
        {
            //try
            //{
                Secret = strSecret;
                byte[] NoviFile = Encode2File(FileName, Secret);
                // proveri prvo sve greske
                var MS = new FileStream(ToFileName, FileMode.Create);
                MS.Write(NoviFile, 0, NoviFile.Length);
                MS.Close();
            //}
            //catch
            //{
              //  throw new Exception("Error ocurred at position : \n" + LastCommand);
            //}
        }

        public byte[] Encoding(string EncodeString, string strSecret)
        {
            //try
            //{
                EncodedFile = null;
                BinaryFile = null;
                Secret = strSecret;
                EncodedFile = Encode(null, EncodeString, itemSymbolTable);
                return ToEncoding();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error ocurred at position : \n" + LastCommand);
           // }
        }

        private TLVList Encode(TLV Parent, string FileToParse, SymbolTable DocFile)
        {
            var NewTlvList = new TLVList();
            while (true)
            {
                int PosLine = FileToParse.IndexOf(';');
                int PosGroup = FileToParse.IndexOf('{');
                if (PosLine < 0 && PosGroup < 0)
                    break;
                if (PosLine > PosGroup && PosGroup > -1)
                {
                    // Exclude Comment Before Began // # /* */ 
                    //We Find Post Group Recursive Call
                    string Name =
                    FileToParse.Substring(0, PosGroup).Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim
                    ();
                    if (Name.StartsWith("/*"))
                        Name = Name.Substring(Name.IndexOf("*/") + 2).Trim();
                    LastCommand = Name;
                    if (Name != "Main")
                    {
                        Param FindElem = DocFile[Name];
                        string FileToParse2 = Split(FileToParse.Substring(PosGroup));
                        string Manage = FileToParse2.Substring(1, FileToParse2.Length - 2);
                        TLV newTlvElement = TLV.Create(FindElem, "");
                        newTlvElement.setParent(Parent);
                        newTlvElement.children = Encode(newTlvElement, Manage, FindElem.Group);
                        FileToParse = FileToParse.Substring(PosGroup + FileToParse2.Length);
                        NewTlvList.Add(newTlvElement);
                        DebugEvent(ref newTlvElement);
                    }
                    else
                    {
                        string FileToParse2 = Split(FileToParse.Substring(PosGroup));
                        string Manage = FileToParse2.Substring(1, FileToParse2.Length - 2);
                        NewTlvList = Encode(null, Manage, DocFile);
                        FileToParse = FileToParse.Substring(PosGroup + FileToParse2.Length);
                    }
                }
                else
                {
                    var getRegEx = new Regex(@"\b.*?;");
                    string NewValue = getRegEx.Match(FileToParse).ToString().Replace('\t', ' ').Replace('\n', ' ');
                    // string NewVal = FileToParse.Substring(0, PosLine ).Trim();
                    string SymbolName = NewValue.Split(' ')[0];
                    LastCommand = SymbolName;
                    NewValue = NewValue.Substring(SymbolName.Length + 1, NewValue.IndexOf(';') - SymbolName.Length - 1);
                    FileToParse = FileToParse.Substring(PosLine + 1);
                    if (SymbolName != "GenericTLV")
                    {
                        TLV NewTlvElement = TLV.Create(DocFile[SymbolName], NewValue);

                        NewTlvElement.setParent(Parent);
                        DebugEvent(ref NewTlvElement);
                        NewTlvList.Add(NewTlvElement);
                    }
                    else
                    {
                        TLV NewTlvElement = new TLVGeneric(SymbolName, NewValue);
                        NewTlvElement.setParent(Parent);
                        DebugEvent(ref NewTlvElement);
                        NewTlvList.Add(NewTlvElement);
                    }
                }
            }
            return NewTlvList;
        }

        public byte[] ToEncoding()
        {
            if (EncodedFile.Count() == 0)
                return null;
            var encodeStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(encodeStream);
            binaryWriter.Write(EncodedFile.GetEncodings());
            TLV cmMIC = new TLVCMMIC(EncodedFile);
            binaryWriter.Write(cmMIC.ToEncoding());
            EncodedFile.Add(cmMIC);
            binaryWriter.Write((new TLVCMTSMIC(EncodedFile, Secret).ToEncoding()));
            TLV EndOfBataMkr = new TLVSpecial();
            // Add EndOfDataMkr before making the MD5
            binaryWriter.Write(EndOfBataMkr.ToEncoding());
            return encodeStream.ToArray();
        }

        public byte[] ToEncodingMTA()
        {
            if (EncodedFile.Count() == 0)
                return null;
            var encodeStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(encodeStream);
            TLVUchar mtaDelimiterBegin = new TLVUchar("MtaConfigDelimiter", "1");

            binaryWriter.Write(EncodedFile.GetEncodings());
            //TODO: add Signature SNMP
            //SnmpMibObject  HexString calculate new Hesh Value
            TLVSnmp signature = new TLVSnmp("SnmpMibObject", "enterprises.7432.1.1.2.9.0 HexString 0x9b5cd26c31ae7818d23dd417553bd9c26768fe0f");
            TLVUchar mtaDelimiterEnd = new TLVUchar("MtaConfigDelimiter", "255");
            TLV EndOfBataMkr = new TLVSpecial();
            // Add EndOfDataMkr before making the MD5
            binaryWriter.Write(EndOfBataMkr.ToEncoding());
            return encodeStream.ToArray();
        }



        public string Split(string Value)
        {
            int position = 0;
            int txtPos = 0;
            foreach (char chValue in Value)
            {
                txtPos++;
                if (chValue == '{')
                    position++;
                if (chValue == '}')
                    position--;
                if (position == 0)
                    break;
            }

            return Value.Substring(0, txtPos);
        }
    }
}
