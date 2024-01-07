using Landmark.Database.Enums;

namespace Landmark.Database.Model
{
    public class Race
    {
        public long Id { get; set; }

        public required string SerialTag { get; set; }

        public int TeamNumber { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? FinishTime { get; set; }

        public int CountPoints { get; set; }

        public SexType SexType { get; set; }

        public virtual List<Participant> Participants { get; set; } = new List<Participant>();
    }
}