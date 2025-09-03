using AutoFilter.DemoDb;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        var db = new AppDbContext();

        var invoice = await db.Invoices
            .OrderBy(x => x.Number)
            //.ThenByDescending(x => x.Number)
            .Select(x => new { x.Number, x.Total })            
            .ToListAsync();

        invoice.ForEach(Console.WriteLine);
    }
}