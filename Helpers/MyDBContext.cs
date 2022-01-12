using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Helpers
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options) {
            //DataGenerator dg = new DataGenerator();
            //dg.
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        //public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conversation>()
                .HasMany(b => b.Messages)
                .WithOne();
        }
    }
}