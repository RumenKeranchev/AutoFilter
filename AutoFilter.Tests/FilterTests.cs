using AutoFilter.Core;
using AutoFilter.DemoDb;
using System.Linq.Expressions;

namespace AutoFilter.Tests
{
    class FilterTests : BaseTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase(" ")]
        [TestCase(" ")] // alt+255
        public void Empty_Column_Should_Throw(string field)
        {
            var invalidFilter = new Filter(field, Operator.Equal, "Value");
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total };

            Assert.Throws<InvalidOperationException>(() =>
            {
                var query = Db.Invoices
                    .Select(castExp)
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        [TestCase("gibberish")]
        [TestCase("nUmBer ")]
        [TestCase(" nUmBer")]
        public void Invalid_Column_Should_Throw(string field)
        {
            var invalidFilter = new Filter(field, Operator.Equal, "Value");
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total };

            Assert.Throws<ArgumentException>(() =>
            {
                var query = Db.Invoices
                    .Select(castExp)
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        public void Missing_Query_Cast_Should_Throw()
        {
            var invalidFilter = new Filter("Number", Operator.Equal, "Value");

            Assert.Throws<InvalidOperationException>(() =>
            {
                var query = Db.Invoices
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        [TestCase("SentDate", "2025-13-13")]
        [TestCase("SentDate", "2025-03-33")]
        [TestCase("SentDate", "20250333")]
        [TestCase("SentDate", "random")]
        [TestCase("SentDate", "random")]
        [TestCase("IsPaid", "random")]
        [TestCase("Total", "random")]
        public void Invalid_Value_Should_Throw(string field, string value)
        {
            var invalidFilter = new Filter(field, Operator.Equal, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Total = x.Total, SentDate = x.SentDate, IsPaid = x.IsPaid };

            Assert.Throws<FormatException>(() =>
            {
                var query = Db.Invoices
                    .Select(castExp)
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        public void Null_Value_Should_Throw()
        {
            var invalidFilter = new Filter("SentDate", Operator.Equal, null);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, SentDate = x.SentDate };

            Assert.Throws<NullReferenceException>(() =>
            {
                var query = Db.Invoices
                    .Select(castExp)
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        [TestCase(Operator.NotContains, "INV", "CRN")]
        [TestCase(Operator.NotContains, "inv", "CRN")]
        [TestCase(Operator.NotContains, "IN", "CRN")]
        [TestCase(Operator.NotContains, "in", "CRN")]
        [TestCase(Operator.NotContains, "DEP", "INV")]
        [TestCase(Operator.NotEqual, "DEP-1001", "INV-1001")]
        [TestCase(Operator.NotEqual, "INV-1001", "INV-1002")]
        [TestCase(Operator.NotEqual, "inv-1001", "INV-1002")]
        [TestCase(Operator.Contains, "DEP", null)]
        [TestCase(Operator.Contains, "INV", "INV")]
        [TestCase(Operator.Contains, "inv", "INV")]
        [TestCase(Operator.Contains, "IN", "INV")]
        [TestCase(Operator.Contains, "in", "INV")]
        [TestCase(Operator.Equal, "INV-1001", "INV")]
        [TestCase(Operator.Equal, "inv-1001", "INV")]
        [TestCase(Operator.Equal, "DEP-1001", null)]
        public void Valid_String_Filter_Should_Work(Operator op, string value, string expectedPrefix)
        {
            var filter = new Filter("Number", op, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total };

            var invoice = Db.Invoices
                .OrderBy(x => x.Id)
                .Select(castExp)
                .Apply(filter)
                .FirstOrDefault();

            if (expectedPrefix is null)
            {
                Assert.That(invoice, Is.Null);
            }
            else
            {
                Assert.That(invoice, Is.Not.Null);
                Assert.That(invoice?.Number, Does.StartWith(expectedPrefix));
            }
        }

        [Test]
        [TestCase("number", Operator.GreaterThan, "INV")]
        [TestCase("total", Operator.Contains, "24")]
        [TestCase("DueDate", Operator.Contains, "2025-09-02")]
        [TestCase("SentDate", Operator.Contains, "2025-09-02")]
        [TestCase("ispaid", Operator.LessThanOrEqual, "true")]
        public void Invalid_Operator_Should_Throw(string field, Operator op, string value)
        {
            var invalidFilter = new Filter(field, op, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total, DueDate = x.DueDate, SentDate = x.SentDate, IsPaid = x.IsPaid };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var query = Db.Invoices
                    .Select(castExp)
                    .Apply(invalidFilter)
                    .FirstOrDefault();
            });
        }

        [Test]
        [TestCase(Operator.GreaterThan, "18", 24)]
        [TestCase(Operator.GreaterThan, "24", null)]
        [TestCase(Operator.GreaterThan, "-9.6", 24)]
        [TestCase(Operator.GreaterThan, "-9,6", 24)]
        [TestCase(Operator.GreaterThanOrEqual, "24", 24)]
        [TestCase(Operator.GreaterThanOrEqual, "25", null)]
        [TestCase(Operator.GreaterThanOrEqual, "-9.6", 24)]
        [TestCase(Operator.GreaterThanOrEqual, "-9,6", 24)]
        [TestCase(Operator.LessThan, "24", 12)]
        [TestCase(Operator.LessThan, "-10", null)]
        [TestCase(Operator.LessThan, "-9.6", null)]
        [TestCase(Operator.LessThan, "-9,6", null)]
        [TestCase(Operator.LessThanOrEqual, "24", 24)]
        [TestCase(Operator.LessThanOrEqual, "-10", null)]
        [TestCase(Operator.LessThanOrEqual, "-9.6", -9.6)]
        [TestCase(Operator.LessThanOrEqual, "-9,6", -9.6)]
        [TestCase(Operator.Equal, "12", 12)]
        [TestCase(Operator.Equal, "35", null)]
        [TestCase(Operator.Equal, "-9.6", -9.6)]
        [TestCase(Operator.Equal, "-9,6", -9.6)]
        [TestCase(Operator.NotEqual, "24", 12)]
        [TestCase(Operator.NotEqual, "65", 24)]
        [TestCase(Operator.NotEqual, "-9.6", 24)]
        [TestCase(Operator.NotEqual, "-9,6", 24)]
        public void Valid_Number_Filter_Should_Work(Operator op, string value, decimal? expected)
        {
            var filter = new Filter("total", op, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total };

            var invoice = Db.Invoices
                .OrderBy(x => x.Id)
                .Select(castExp)
                .Apply(filter)
                .FirstOrDefault();

            Assert.That(invoice?.Total, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Operator.Equal, "2025-09-27 00:00", "2025-09-27 00:00")]
        [TestCase(Operator.Equal, "2025-09-29 21:00", null)]
        [TestCase(Operator.NotEqual, "2025-10-02 16:00", "2025-09-17 07:30")]
        [TestCase(Operator.NotEqual, "2025-10-02 15:00", "2025-09-17 07:30")]
        [TestCase(Operator.GreaterThan, "2025-09-17 07:30", "2025-09-17 09:00")]
        [TestCase(Operator.GreaterThan, "2025-10-10 07:30", null)]
        [TestCase(Operator.GreaterThanOrEqual, "2025-09-17 07:30", "2025-09-17 07:30")]
        [TestCase(Operator.GreaterThanOrEqual, "2025-09-17 08:00", "2025-09-17 09:00")]
        [TestCase(Operator.GreaterThanOrEqual, "2025-10-17 08:00", null)]
        [TestCase(Operator.LessThan, "2025-09-22 11:00", "2025-09-17 07:30")]
        [TestCase(Operator.LessThan, "2025-09-10 11:00", null)]
        [TestCase(Operator.LessThanOrEqual, "2025-09-22 11:00", "2025-09-17 07:30")]
        [TestCase(Operator.LessThanOrEqual, "2025-09-22 10:00", "2025-09-17 07:30")]
        [TestCase(Operator.LessThanOrEqual, "2025-09-01 10:00", null)]
        public void Valid_DateTime_Filter_Should_Work(Operator op, string value, string expected)
        {
            var filter = new Filter("duedate", op, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { DueDate = x.DueDate };

            var invoice = Db.Invoices
                .OrderBy(x => x.DueDate)
                .Select(castExp)
                .Apply(filter)
                .FirstOrDefault();

            if (expected is null)
            {
                Assert.That(invoice, Is.Null);
            }
            else
            {
                Assert.That(invoice.DueDate, Is.EqualTo(DateTime.Parse(expected)));
            }
        }

        [Test]
        [TestCase(Operator.Equal, "true", true)]
        [TestCase(Operator.Equal, "false", false)]
        [TestCase(Operator.NotEqual, "false", true)]
        [TestCase(Operator.NotEqual, "true", false)]
        public void Valid_Bool_Filter_Should_Work(Operator op, string value, bool expected)
        {
            var filter = new Filter("ispaid", op, value);
            Expression<Func<Invoice, Invoice>> castExp = x => new() { IsPaid = x.IsPaid };

            var invoice = Db.Invoices
                .OrderBy(x => x.Id)
                .Select(castExp)
                .Apply(filter)
                .FirstOrDefault();

            Assert.That(invoice, Is.Not.Null);
            Assert.That(invoice.IsPaid, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("status", Operator.Equal, "sent", "duedate", Operator.GreaterThanOrEqual, "2025-10-02 15:00", "INV-1001")]
        [TestCase("duedate", Operator.GreaterThan, "2025-09-17 07:30", "duedate", Operator.LessThanOrEqual, "2025-09-17 09:00", "CRN-1002")]
        [TestCase("type", Operator.Equal, "invoice", "ispaid", Operator.NotEqual, "false", "INV-1001")]
        public void Multiple_Valid_Filters_Should_Work(string field1, Operator op1, string value1, string field2, Operator op2, string value2, string expected)
        {
            var filter1 = new Filter(field1, op1, value1);
            var filter2 = new Filter(field2, op2, value2);

            Expression<Func<Invoice, Invoice>> castExp = x => x;

            var invoice = Db.Invoices
                .Select(castExp)
                .Apply(filter1)
                .Apply(filter2)
                .FirstOrDefault();

            Assert.That(invoice, Is.Not.Null);
            Assert.That(invoice.Number, Is.EqualTo(expected));
        }
    }
}
