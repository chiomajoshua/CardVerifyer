using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CardVerifyer.Data.Persistence
{
    public class CardVerifyerDbContext : IdentityDbContext
    {
        public CardVerifyerDbContext(DbContextOptions<CardVerifyerDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }
    }
}