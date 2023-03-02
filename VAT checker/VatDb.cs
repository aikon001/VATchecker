using Microsoft.EntityFrameworkCore;
namespace VAT_checker
{
    class VatDb : DbContext
    {
        public VatDb(DbContextOptions<VatDb> options)
            : base(options) { }

        public DbSet<Vat> Vats => Set<Vat>();
    }
}