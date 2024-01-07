namespace Landmark.ER301Driver.Enums
{
    public enum TagRequestCode : byte
    {
        IDLE_CARD = 0x26,   // If the card is halted, it won't respond
        ALL_TYPE_A = 0x52  // Will activate even a halted card
    };
}

