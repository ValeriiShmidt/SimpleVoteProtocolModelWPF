namespace Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int PermissionToVoteId { get; set; }
        public PermissionToVote? PermissionToVote { get; set; }
    }
}
