namespace Landmark.ER301Driver.Enums
{
    public enum ResponseReader : byte
    {
        OK = 0,
        ERR_BAUD_RATE = 1,
        ERR_PORT_DISCONNECT = 2,
        ERR_GENERAL = 10,
        ERR_UNDEFINED = 11,
        ERR_COMMAND_PARAMETER = 12,
        ERR_NO_CARD = 13,
        ERR_REQUEST_FAILURE = 20, // 0x14, no card present
        ERR_RESET_FAILURE = 21,
        ERR_AUTH_FAILURE = 22,
        ERR_READ_BLOCK_FAILURE = 23,
        ERR_WRITE_BLOCK_FAILURE = 24,
        ERR_WRITE_ADDRESS_FAILURE = 25,
        ERR_WRITE_ADDRESS_FAILURE2 = 26
    };
}

