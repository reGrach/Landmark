namespace Landmark.ER301Driver.Enums
{
    public enum TagType : ushort
    {
        ULTRALIGHT = 0x0044,
        MIFARE_CLASSIC_1K_S50 = 0x0004,

        // The Mifare Classic 4k has 4096 bytes across 40 sectors: Sector 0-31 are divided into 4 discrete 16-byte 
        // blocks (2Kb). Sector 32-39  are divided into 16 discrete 16-byte blocks (2Kb). Block 0 is accessable 
        // without authorization, but all other remaining blocks require authorization per sector basis. Last block 
        // in a sector (sector trailer) is reserved for the storage of the auth info. This means there are 41 
        // non-writable blocks (656 bytes), leaving 215 writable block (3440 bytes). The auth blocks are divided 
        // into 6 bytes for key A, 4 bytes of read/write configuration and 6 bytes for key B.
        // Accessing a block is a two step process. First you must authenticate to the sector with either the 
        // A or the B key, then you can read or write one of the blocks in that sector.
        //
        MIFARE_CLASSIC_4K_S70 = 0x0002,
        DESFIRE = 0x0344,
        PRO = 0x0008,
        PRO_X = 0x0304
    };
}

