using CatFoodManager.Core.Repositories;
using CommonTools.Database;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Core.Repositories;

public class RepositoryBaseTests
{
    [Fact]
    public void Constructor_ShouldSetSQLiteHelper()
    {
        var sqliteHelperMock = new Mock<SQLiteHelper>();
        var repository = new TestRepository(sqliteHelperMock.Object);

        Assert.NotNull(repository);
    }

    [Fact]
    public void Migrate_ShouldCallCreateTable()
    {
        var sqliteHelperMock = new Mock<SQLiteHelper>();
        var repository = new TestRepository(sqliteHelperMock.Object);

        repository.Migrate<TestEntity>();

        sqliteHelperMock.Verify(x => x.Db, Times.Once);
    }

    private class TestRepository : RepositoryBase
    {
        public TestRepository(SQLiteHelper sqliteHelper) : base(sqliteHelper) { }
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
