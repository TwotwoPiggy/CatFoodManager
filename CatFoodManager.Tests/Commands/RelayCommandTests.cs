using CatFoodManager.Commands;
using Xunit;

namespace CatFoodManager.Tests.Commands;

public class RelayCommandTests
{
    [Fact]
    public void Execute_ShouldInvokeExecuteAction()
    {
        var executed = false;
        var command = new RelayCommand(_ => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void Execute_WithParameter_ShouldPassParameter()
    {
        object? receivedParameter = null;
        var command = new RelayCommand(param => receivedParameter = param);

        command.Execute("TestParameter");

        Assert.Equal("TestParameter", receivedParameter);
    }

    [Fact]
    public void CanExecute_WhenNoCanExecuteFunc_ShouldReturnTrue()
    {
        var command = new RelayCommand(_ => { });

        var result = command.CanExecute(null);

        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsTrue_ShouldReturnTrue()
    {
        var command = new RelayCommand(_ => { }, _ => true);

        var result = command.CanExecute(null);

        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsFalse_ShouldReturnFalse()
    {
        var command = new RelayCommand(_ => { }, _ => false);

        var result = command.CanExecute(null);

        Assert.False(result);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseEvent()
    {
        var command = new RelayCommand(_ => { });
        var eventRaised = false;

        command.CanExecuteChanged += (sender, e) => eventRaised = true;
        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }

    [Fact]
    public void Constructor_WithNullExecuteAction_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }
}

public class RelayCommandGenericTests
{
    [Fact]
    public void Execute_ShouldInvokeExecuteAction()
    {
        var executed = false;
        var command = new RelayCommand<string>(_ => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void Execute_WithParameter_ShouldPassTypedParameter()
    {
        string? receivedParameter = null;
        var command = new RelayCommand<string>(param => receivedParameter = param);

        command.Execute("TestParameter");

        Assert.Equal("TestParameter", receivedParameter);
    }

    [Fact]
    public void CanExecute_WhenNoCanExecuteFunc_ShouldReturnTrue()
    {
        var command = new RelayCommand<int>(_ => { });

        var result = command.CanExecute(42);

        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsTrue_ShouldReturnTrue()
    {
        var command = new RelayCommand<int>(_ => { }, _ => true);

        var result = command.CanExecute(42);

        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsFalse_ShouldReturnFalse()
    {
        var command = new RelayCommand<int>(_ => { }, _ => false);

        var result = command.CanExecute(42);

        Assert.False(result);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseEvent()
    {
        var command = new RelayCommand<string>(_ => { });
        var eventRaised = false;

        command.CanExecuteChanged += (sender, e) => eventRaised = true;
        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }

    [Fact]
    public void Constructor_WithNullExecuteAction_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand<string>(null!));
    }
}
