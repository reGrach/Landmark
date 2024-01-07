namespace Landmark.ER301Driver.Enums
{
    public enum CommandReader : ushort
    {
        INIT_PORT = 0x0101,
        SET_DEVICE_NODE_NO = 0x0102,
        GET_DEVICE_NODE_NO = 0x0103,
        READ_DEVICE_MODE = 0x0104,
        SET_BUZZER_BEEP = 0x0106,
        SET_LED_COLOR = 0x0107,
        RFU = 0x0108,
        SET_ANTENNA_STATUS = 0x010c,
        MIFARE_REQUEST = 0x0201,
        MIFARE_ANTICOLLISION = 0x0202,
        MIFARE_SELECT = 0x0203,
        MIFARE_HLTA = 0x0204,
        MIFARE_AUTHENTICATION2 = 0x0207,
        MIFARE_READ = 0x0208,
        MIFARE_WRITE = 0x0209,
        MIFARE_INITVAL = 0x020A,
        MIFARE_READBALANCE = 0x020B,
        MIFARE_DECREMENT = 0x020C,
        MIFARE_INCREMENT = 0x020D
    };
}

