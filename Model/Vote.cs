namespace Model
{
    public class Vote
    {
        public int Id { get; set; }
        public int ElectorId { get; set; }
        public Person? Elector { get; set; }
        public int CandidateId { get; set; }
        public Person? Candidate { get; set; }
    }
}
