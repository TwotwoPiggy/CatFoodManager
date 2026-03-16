using CatFoodManager.Domain.Interfaces;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Domain.Interfaces;

public class IUnitOfWorkTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public IUnitOfWorkTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnAffectedRows()
    {
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _unitOfWorkMock.Object.SaveChangesAsync();

        Assert.Equal(5, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_ShouldPassToken()
    {
        var cts = new CancellationTokenSource();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(cts.Token))
            .ReturnsAsync(1);

        var result = await _unitOfWorkMock.Object.SaveChangesAsync(cts.Token);

        Assert.Equal(1, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(cts.Token), Times.Once);
    }

    [Fact]
    public void Repository_ShouldReturnRepositoryInstance()
    {
        var mockRepository = new Mock<IRepository<TestEntityForUow>>();
        _unitOfWorkMock.Setup(u => u.Repository<TestEntityForUow>())
            .Returns(mockRepository.Object);

        var result = _unitOfWorkMock.Object.Repository<TestEntityForUow>();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IRepository<TestEntityForUow>>(result);
        _unitOfWorkMock.Verify(u => u.Repository<TestEntityForUow>(), Times.Once);
    }

    [Fact]
    public void Dispose_ShouldBeCalled()
    {
        _unitOfWorkMock.Object.Dispose();

        _unitOfWorkMock.Verify(u => u.Dispose(), Times.Once);
    }
}

public class TestEntityForUow : IEntity
{
    public long Id { get; set; }
}
