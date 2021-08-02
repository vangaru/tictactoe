using Microsoft.EntityFrameworkCore;

namespace TicTacToeOnlineGame.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=app.db");
        }
    }
}