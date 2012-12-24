using System;

namespace DocsisLibrary
{
    [Serializable]
    public enum SnmpType // RFC1213 subset of ASN.1
    {
        EndMarker = 0x00,
        Boolean = 0x01,
        Integer32 = 0x02, // X690.Int
        BitString = 0x03, // X690.BitSet // TODO: verify if this is BITS.
        OctetString = 0x04, // X690.OctetString
        Null = 0x05,
        ObjectIdentifier = 0x06, // uint[]
        ObjectDescriptor = 0x07,
        ExternalInstance = 0x08,
        Real = 0x09, // X690.Real
        Enumerated = 0x0a,
        EmbeddedPDV = 0x0b,
        UTF8String = 0x0c,
        RelativeOID = 0x0d,
        Reserved1 = 0x0e,
        Reserved2 = 0x0f,
        SequenceTagNumber = 0x10, // X690.Sequence (this is in fact the tag number for SEQUENCE)
        Set = 0x11,
        NumericString = 0x12,
        PrintableString = 0x13,
        T61String = 0x14,
        VideoTextString = 0x15,
        IA5String = 0x16,
        UTCTime = 0x17,
        GeneralizedTime = 0x18,
        GraphicString = 0x19,
        VisibleString = 0x1a,
        GeneralString = 0x1b,
        UniversalString = 0x1c,
        CharacterString = 0x1d,
        BMPString = 0x1e,
        Sequence = 0x30, // RFC1213 sequence for whole SNMP packet beginning
        IPAddress = 0x40,
        Counter32 = 0x41,
        Gauge32 = 0x42,
        TimeTicks = 0x43,
        Opaque = 0x44,
        NetAddress = 0x45,
        Counter64 = 0x46,
        UInt32 = 0x47,
        NoSuchObject = 0x80,
        NoSuchInstance = 0x81,
        EndOfMibView = 0x82,
        GetRequestPdu = 0xA0,
        GetNextRequestPdu = 0xA1,
        GetResponsePdu = 0xA2,
        SetRequestPdu = 0xA3,
        TrapV1Pdu = 0xA4,
        GetBulkRequestPdu = 0xA5,
        InformRequestPdu = 0xA6,
        TrapV2Pdu = 0xA7,
        ReportPdu = 0xA8
    }
}
