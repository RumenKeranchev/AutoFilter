using AutoFilter.Core;
using AutoFilter.DemoDb;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

class Program
{
    static async Task Main(string[] args)
    {
        var db = new AppDbContext();

        Expression<Func<InvoiceDetail, Dto>> castExpr = x => new() { Number = x.Invoice!.Number, Id = x.Invoice_Id, Total = x.Invoice.Total };

        var invoice = await db.InvoiceDetails
            .Select(castExpr)
            .Apply(new Sort("number", Dir.Desc))
            .Apply(new Filter("number", Operator.NotContains, "02"))
            .Apply(new Filter("number", Operator.NotContains, "in"))
            .FirstOrDefaultAsync();

        Console.WriteLine(invoice);
    }

    record Dto
    {
        public required string Number { get; set; }

        public required int Id { get; set; }

        public decimal Total { get; set; }
    }
}