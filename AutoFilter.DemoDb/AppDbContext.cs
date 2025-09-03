using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoFilter.DemoDb
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("AutoFilter").Options)
        {
            Database.EnsureCreated();
            Seed();
        }

        void Seed()
        {
            var invoices = new List<Invoice>
            {
                new() {
                    Number = "INV-1001",
                    Type = "Invoice",
                    Status = "Sent",
                    SentDate = DateTime.UtcNow.Date.AddDays(30).AddHours(15),
                    DueDate = new DateTime(2025, 10, 2, 15, 0, 0, DateTimeKind.Local),
                    IsPaid = true,
                    Total = 24,
                },
                new()
                {
                    Number = "INV-1002",
                    Type = "Invoice",
                    Status = "Draft",
                    DueDate = new DateTime(2025, 09, 17, 07, 30, 0, DateTimeKind.Local),
                    Total = 12,
                },
                new()
                {
                    Number = "CRN-1001",
                    Type = "Credit Note",
                    Status = "Sent",
                    DueDate = new DateTime(2025, 09, 22, 11, 0, 0, DateTimeKind.Local),
                    SentDate = DateTime.UtcNow.Date.AddDays(20).AddHours(11),
                    IsPaid = true,
                    Total = -6,
                },
                new()
                {
                    Number="CRN-1002",
                    Type = "Credit Note",
                    Status = "Draft",
                    DueDate = new DateTime(2025, 09, 17, 09, 0, 0, DateTimeKind.Local),
                    Total = -9.6m,
                },
                new()
                {
                    Number = "INV-1003",
                    Type = "Invoice",
                    Status = "Sent",
                    DueDate = new DateTime(2025, 09, 27, 0, 0, 0, DateTimeKind.Local),
                    Total = 18,
                },
            };

            AddRange(invoices);
            SaveChanges();
        }

        public DbSet<Invoice> Invoices { get; set; }
    }
}
