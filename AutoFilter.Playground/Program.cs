using AutoFilter.DemoDb;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

class Program
{
    static async Task Main(string[] args)
    {
        var db = new AppDbContext();

        Expression<Func<Invoice, Invoice>> castExpr = x => new() { DueDate = x.DueDate };

        var invoice = await db.Invoices
            .OrderBy(x => x.DueDate)
            .Select(castExpr)
            .ToListAsync();

        invoice.ForEach(i => Console.WriteLine(i.DueDate));
    }

    record Dto
    {
        //public required string Number { get; set; }

        //public required int Id { get; set; }

        public decimal Total { get; set; }
    }
}