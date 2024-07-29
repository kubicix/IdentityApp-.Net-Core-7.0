using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext : IdentityDbContext<IdentityUser>
    {
        //connection string dışarıdan vermek için base(options) kullandık eğer direk buraya yazmak istiyorsak ta yazabilirdik
        public IdentityContext(DbContextOptions<IdentityContext> options):base(options)
        {

        }
    }
}