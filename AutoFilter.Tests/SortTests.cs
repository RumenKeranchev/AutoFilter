using AutoFilter.Core;
using AutoFilter.DemoDb;
using System.Linq.Expressions;

namespace AutoFilter.Tests
{
    class SortTests : BaseTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase(" ")]
        [TestCase(" ")] // alt+255
        public void Empty_Column_Should_Throw(string field)
        {
            var invalidFilter = new Sort(field, Dir.Asc);
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
            var invalidFilter = new Sort(field, Dir.Asc);
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
        public void Valid_Sort_Should_Work()
        {
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Total = x.Total };
            var sort = new Sort("Number", Dir.Asc);

            var query = Db.Invoices
                .Select(castExp)
                .Apply(sort)
                .ToList();

            Assert.That(query, Has.Count.EqualTo(6));
            Assert.Multiple(() =>
            {
                Assert.That(query[0].Number, Is.EqualTo("CRN-1001"));
                Assert.That(query[1].Number, Is.EqualTo("CRN-1002"));
                Assert.That(query[2].Number, Is.EqualTo("INV-1001"));
                Assert.That(query[3].Number, Is.EqualTo("INV-1002"));
                Assert.That(query[4].Number, Is.EqualTo("INV-1003"));
                Assert.That(query[5].Number, Is.EqualTo("INV-1004"));
            });

            sort = new Sort("Number", Dir.Desc);
            query = [.. Db.Invoices
                .Select(castExp)
                .Apply(sort)];

            Assert.That(query, Has.Count.EqualTo(6));
            Assert.Multiple(() =>
            {
                Assert.That(query[0].Number, Is.EqualTo("INV-1004"));
                Assert.That(query[1].Number, Is.EqualTo("INV-1003"));
                Assert.That(query[2].Number, Is.EqualTo("INV-1002"));
                Assert.That(query[3].Number, Is.EqualTo("INV-1001"));
                Assert.That(query[4].Number, Is.EqualTo("CRN-1002"));
                Assert.That(query[5].Number, Is.EqualTo("CRN-1001"));
            });
        }

        [Test]
        public void Multiple_Valid_Sorts_Should_Work()
        {
            Expression<Func<Invoice, Invoice>> castExp = x => new() { Number = x.Number, Type = x.Type, Total = x.Total };
            var sort1 = new Sort("Type", Dir.Asc);
            var sort2 = new Sort("Number", Dir.Desc);

            var query = Db.Invoices
                .Select(castExp)
                .Apply(sort1)
                .Apply(sort2)
                .ToList();

            Assert.That(query, Has.Count.EqualTo(6));
            Assert.Multiple(() =>
            {
                Assert.That(query[0].Type, Is.EqualTo("Credit Note"));
                Assert.That(query[0].Number, Is.EqualTo("CRN-1002"));
                Assert.That(query[1].Type, Is.EqualTo("Credit Note"));
                Assert.That(query[1].Number, Is.EqualTo("CRN-1001"));
                Assert.That(query[2].Type, Is.EqualTo("Invoice"));
                Assert.That(query[2].Number, Is.EqualTo("INV-1004"));
                Assert.That(query[3].Type, Is.EqualTo("Invoice"));
                Assert.That(query[3].Number, Is.EqualTo("INV-1003"));
                Assert.That(query[4].Type, Is.EqualTo("Invoice"));
                Assert.That(query[4].Number, Is.EqualTo("INV-1002"));
                Assert.That(query[5].Type, Is.EqualTo("Invoice"));
                Assert.That(query[5].Number, Is.EqualTo("INV-1001"));
            });
        }
    }
}
