using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Models;

namespace PetBuddies_API.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

    }
}
