using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AutoFilter.DemoDb
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options)
        //public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>()
        //    .UseSqlServer("Server=localhost;Database=AutoFilter;Trusted_Connection=True;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True").Options)
        {
            Database.EnsureCreated();
            Seed();
        }

        // All DateTime values are stored in UTC and inputs in Seed() are in UTC.
        void Seed()
        {
            var invoices = new List<Invoice>
            {
                new() {
                    Number = "INV-1001",
                    Type = "Invoice",
                    Status = "Sent",
                    SentDate =DateTime.Parse("2026-01-05 08:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    DueDate = DateTime.Parse("2026-01-10 13:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    IsPaid = true,
                    Total = 24,
                },
                new()
                {
                    Number = "INV-1002",
                    Type = "Invoice",
                    Status = "Draft",
                    DueDate = DateTime.Parse("2026-01-31 16:30", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    Total = 12,
                },
                new()
                {
                    Number = "CRN-1001",
                    Type = "Credit Note",
                    Status = "Sent",
                    SentDate =DateTime.Parse("2026-01-12 14:30", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    DueDate = DateTime.Parse("2026-01-20 18:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    IsPaid = true,
                    Total = -6,
                },
                new()
                {
                    Number="CRN-1002",
                    Type = "Credit Note",
                    Status = "Draft",
                    DueDate = DateTime.Parse("2026-01-21 15:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    Total = -9.6m,
                },
                new()
                {
                    Number = "INV-1003",
                    Type = "Invoice",
                    Status = "Sent",
                    DueDate = DateTime.Parse("2026-01-07 10:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    Total = 18,
                },
                new()
                {
                    Number = "INV-1004",
                    Type = "Invoice",
                    Status = "Draft",
                    DueDate = DateTime.Parse("2026-01-28 09:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    Total = 18,
                },
            };

            AddRange(invoices);
            SaveChanges();
        }

        public DbSet<Invoice> Invoices { get; set; }
    }
}
