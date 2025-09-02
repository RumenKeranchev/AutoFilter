IQueryable extension methods for filtering and sorting data on an already projected (.Select) query.  
Both sorting and filtering extension methods are called ".Apply" where each takes corresponding class as a parameter.  
  
**Filter** class accepts 3 parameters: field, operand and value.  
**Sort** class accepts 2 parameters: field and direction.  

Example usage:  
```
static async Task Main(string[] args)
{
    var db = new AppDbContext();
    Expression<Func<InvoiceDetail, Dto>> castExpr =  
      x => new() { Number = x.Invoice!.Number, Id = x.Invoice_Id, Total = x.Invoice.Total };

    var invoice = await db.InvoiceDetails
         .Select(castExpr)
         .Apply(new Filter("number", Operator.NotContains, "02"))
         .Apply(new Sort("number", Dir.Desc))
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
```
