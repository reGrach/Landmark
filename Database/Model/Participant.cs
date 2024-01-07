namespace Landmark.Database.Model
{
    public class Participant
    {
        public long Id { get; set; }

        public required string Name { get; set; }

        public long? RaceId { get; set; }
    }
}

