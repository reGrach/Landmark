namespace Landmark.ER301Driver.Extensions;

class InvalidChecksumException : Exception
{
    public InvalidChecksumException(string message)
        : base(message) { }
}