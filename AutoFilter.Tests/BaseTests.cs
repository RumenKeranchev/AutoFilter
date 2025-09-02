using AutoFilter.DemoDb;

namespace AutoFilter.Tests
{
    public abstract class BaseTests
    {
        protected AppDbContext Db { get; set; }

        [SetUp]
        public void Setup() => Db = new AppDbContext();

        [TearDown]
        public void Teardown() => Db.Dispose();
    }
}
