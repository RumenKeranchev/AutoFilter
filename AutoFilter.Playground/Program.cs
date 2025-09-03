using AutoFilter.Core;
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
            .Select(x => new { x.DueDate })
            .Apply(new Sort("DueDate", Dir.Asc))
            .ToListAsync();

        invoice.ForEach(i => Console.WriteLine(i));
    }
}