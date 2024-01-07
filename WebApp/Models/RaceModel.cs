namespace Landmark.WebApp.Models
{
    public class RaceModel
    {
        public int Number { get; set; }

        public required string SerialTag { get; set; }

        public string? StartTime { get; set; }

        public string? FinishTime { get; set; }

        public int CountPoints { get; set; }

        public required string SexType { get; set; }

        public List<string>? Participants { get; set; }
    }
}