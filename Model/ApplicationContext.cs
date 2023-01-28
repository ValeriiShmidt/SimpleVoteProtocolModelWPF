using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;
        public DbSet<PermissionToVote> PermissionsToVote { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite(@"Data Source=C:\Users\Valerii\source\repos\My\SimpleVoteProtocolModel\Model\VoteModeling.db");
            optionsBuilder.UseSqlite(@"Data Source=VoteModeling.db");
        }
    }
}
