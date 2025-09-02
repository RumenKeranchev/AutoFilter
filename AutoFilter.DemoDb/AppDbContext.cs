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
                    SentDate = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(30),
                    VatBase = 20,
                    Vat = 4,
                    Total = 24,
                    Details =
                    [
                        new InvoiceDetail { UnitPrice = 100, Quantity = 2, Goods = "Goods 1" },
                        new InvoiceDetail { UnitPrice = 50, Quantity = 1, Goods = "Goods 2" }
                    ],
                },
                new()
                {
                    Number = "INV-1002",
                    Type = "Invoice",
                    Status = "Draft",
                    DueDate = DateTime.UtcNow.AddDays(15),
                    VatBase = 10,
                    Vat = 2,
                    Total = 12,
                    Details =
                    [
                        new InvoiceDetail { UnitPrice = 200, Quantity = 1, Goods = "Goods 3" },
                        new InvoiceDetail { UnitPrice = 20, Quantity = 3, Goods = "Goods 4" }
                    ],
                },
                new()
                {
                    Number = "CRN-1001",
                    Type = "Credit Note",
                    Status = "Sent",
                    SentDate = DateTime.UtcNow.AddDays(-10),
                    DueDate = DateTime.UtcNow.AddDays(20),
                    VatBase = -5,
                    Vat = -1,
                    Total = -6,
                    Details =
                    [
                        new InvoiceDetail { UnitPrice = 50, Quantity = -1, Goods = "Goods 2" },
                        new InvoiceDetail { UnitPrice = 25, Quantity = -1, Goods = "Goods 5" }
                    ],
                },
                new()
                {
                    Number="CRN-1002",
                    Type = "Credit Note",
                    Status = "Draft",
                    DueDate = DateTime.UtcNow.AddDays(10),
                    VatBase = -8,
                    Vat = -1.6m,
                    Total = -9.6m,
                    Details =
                    [
                        new InvoiceDetail { UnitPrice = 80, Quantity = -1, Goods = "Goods 6" }
                    ],
                },
                new()
                {
                    Number = "INV-1003",
                    Type = "Invoice",
                    Status = "Sent",
                    SentDate = DateTime.UtcNow.AddDays(-5),
                    DueDate = DateTime.UtcNow.AddDays(25),
                    VatBase = 15,
                    Vat = 3,
                    Total = 18,
                    Details =
                    [
                        new InvoiceDetail { UnitPrice = 150, Quantity = 1, Goods = "Goods 7" },
                        new InvoiceDetail { UnitPrice = 30, Quantity = 1, Goods = "Goods 8" }
                    ],
                },
            };

            AddRange(invoices);
            SaveChanges();
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
