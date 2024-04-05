using Microsoft.EntityFrameworkCore;
using WebLoginPortal.Models;

namespace WebLoginPortal.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }
        public DbSet<UserInfo> UsersTable { get; set; }
    }
}
