using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace DocsisLibrary
{
    public class DecodeFile
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

        public static string MIBFilePath = Environment.GetEnvironmentVariable("mibs");

        public static MibParser MibFileParese = new MibParser(MIBFilePath);
        private readonly SymbolTable itemSymbolTable;
        private byte[] BinaryFile;
        private TLVList EncodedFile;
        //   public string Secret = "DOCSIS";

        public DecodeFile()
        {
            itemSymbolTable = LoadSymbolTable();
        }

        public DecodeFile(string FileName)
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
                    var formatter = new XmlSerializer(typeof(SymbolTable));
                    try
                    {
                        return (SymbolTable)formatter.Deserialize(fileStream);
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
                    var formatter = new XmlSerializer(typeof(SymbolTable));
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

        public string Decode2File(string FileName)
        {
            var MS = new FileStream(FileName, FileMode.Open);
            BinaryFile = new byte[MS.Length];
            MS.Read(BinaryFile, 0, (int) MS.Length);
            EncodedFile = Decode(null, BinaryFile, itemSymbolTable);
            return ToString();
        }

        public void Decode2File(string FileName, string ToFileName)
        {
            try
            {
                Decode2File(FileName);
                byte[] DecFiles = (new UTF8Encoding()).GetBytes(ToString());
                var MS = new FileStream(ToFileName, FileMode.Create);
                BinaryFile = new byte[MS.Length];
                MS.Write(DecFiles, 0, DecFiles.Length);
                MS.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Nesto Nije uredu" + e.Message);
            }
        }

        public string Decoding(byte[] DecodeString)
        {
            //try
            //{
            EncodedFile = null;
            BinaryFile = null;
            EncodedFile = Decode(null, DecodeString, itemSymbolTable);
            return ToString();
            //}
            //catch (Exception e)
            // {
            //    throw new Exception("Error ocurred at position : \n" + e.Message);
            //  }
        }

        private Param getTLVCode(SymbolTable symbolName, byte Ident)
        {
            foreach (Param paramValue in symbolName.Param)
                if (paramValue.TLV == Ident)
                    return paramValue;
            return null;
        }

        private TLVList Decode(TLV Parent, byte[] binaryStructureFile, SymbolTable DocFile)
        {
            var newTlvList = new TLVList();
            int i = 0;
            while (i < binaryStructureFile.Length - 1)
            {
                byte getItentification = binaryStructureFile[i++];
                int sizeOfElement = 0;
                if (getItentification == 64)
                    sizeOfElement = (binaryStructureFile[i++] << 8) | binaryStructureFile[i++];
                else
                    sizeOfElement = binaryStructureFile[i++];
                Param findParamElement = getTLVCode(DocFile, getItentification);
                var getContextToDecode = new byte[sizeOfElement];

                Array.Copy(binaryStructureFile, i, getContextToDecode, 0, sizeOfElement);
                i += sizeOfElement;
                if (findParamElement != null)
                {
                    bool itsGroup = findParamElement.Group != null ? true : false;
                    if (itsGroup)
                    {
                        //Get some solution for format activating difrent type of class
                        TLV newTlvElement = TLV.Create(findParamElement, getContextToDecode);
                        newTlvElement.setParent(Parent);
                        newTlvElement.children = Decode(newTlvElement, getContextToDecode, findParamElement.Group);
                        DebugEvent(ref newTlvElement);
                        newTlvList.Add(newTlvElement);
                    }
                    else
                    {
                        TLV newTlvElement = TLV.Create(findParamElement, getContextToDecode);
                        newTlvElement.setParent(Parent);
                        DebugEvent(ref newTlvElement);
                        newTlvList.Add(newTlvElement);
                    }
                }
                if (findParamElement == null)
                {
                    TLV newTlvElement = new TLVGeneric("GenericTLV", getItentification, getContextToDecode);
                    newTlvElement.setParent(Parent);
                    DebugEvent(ref newTlvElement);
                    newTlvList.Add(newTlvElement);
                }
            }
            return newTlvList;
        }

        public string Split(string Value)
        {
            int position = 0;
            int txtPosition = 0;
            foreach (char ch in Value)
            {
                txtPosition++;
                if (ch == '{')
                    position++;
                if (ch == '}')
                    position--;
                if (position == 0)
                    break;
            }

            return Value.Substring(0, txtPosition);
        }

        public static Byte[] ConvertStringToByteArray(String s)
        {
            return (new UTF8Encoding()).GetBytes(s);
        }

        public override string ToString()
        {
            string DecodedFile = "Main\n{\n";
            foreach (TLV tlvValue in EncodedFile)
                DecodedFile += "\t"+tlvValue.ToString()+"\n";
            DecodedFile = DecodedFile + "}\n";
            return DecodedFile;
        }
    }
}
