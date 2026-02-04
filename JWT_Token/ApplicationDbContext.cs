using JWT_Token.Model;
using Microsoft.EntityFrameworkCore;

namespace JWT_Token
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
//15 -10 user
