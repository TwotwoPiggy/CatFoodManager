using CatFoodManager.Controls;
using System.Windows.Forms;
using Xunit;

namespace CatFoodManager.Tests.Controls;

public class SearchUserControlTests
{
    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        var control = new SearchUserControl();

        Assert.NotNull(control.SearchCommand);
        Assert.NotNull(control.ResetCommand);
    }

    [Fact]
    public void SearchText_GetSet_ShouldWorkCorrectly()
    {
        var control = new SearchUserControl();

        control.SearchText = "test search";

        Assert.Equal("test search", control.SearchText);
    }

    [Fact]
    public void SearchText_Set_ShouldUpdateTextBox()
    {
        var control = new SearchUserControl();

        control.SearchText = "test value";

        Assert.Equal("test value", control.SearchText);
    }

    [Fact]
    public void SearchCommand_Execute_ShouldRaiseSearchClicked()
    {
        var control = new SearchUserControl();
        var eventRaised = false;
        control.SearchClicked += (s, e) => eventRaised = true;

        control.SearchCommand.Execute(null);

        Assert.True(eventRaised);
    }

    [Fact]
    public void ResetCommand_Execute_ShouldRaiseResetClicked()
    {
        var control = new SearchUserControl();
        var eventRaised = false;
        control.ResetClicked += (s, e) => eventRaised = true;

        control.ResetCommand.Execute(null);

        Assert.True(eventRaised);
    }

    [Fact]
    public void ResetCommand_Execute_ShouldClearSearchText()
    {
        var control = new SearchUserControl();
        control.SearchText = "test";

        control.ResetCommand.Execute(null);

        Assert.Equal(string.Empty, control.SearchText);
    }

    [Fact]
    public void SearchClicked_Event_ShouldProvideCorrectSender()
    {
        var control = new SearchUserControl();
        object? sender = null;
        control.SearchClicked += (s, e) => sender = s;

        control.SearchCommand.Execute(null);

        Assert.Equal(control, sender);
    }

    [Fact]
    public void ResetClicked_Event_ShouldProvideCorrectSender()
    {
        var control = new SearchUserControl();
        object? sender = null;
        control.ResetClicked += (s, e) => sender = s;

        control.ResetCommand.Execute(null);

        Assert.Equal(control, sender);
    }

    [Fact]
    public void SearchText_DefaultValue_ShouldBeEmpty()
    {
        var control = new SearchUserControl();

        Assert.Equal(string.Empty, control.SearchText);
    }

    [Fact]
    public void SearchCommand_CanExecute_ShouldAlwaysReturnTrue()
    {
        var control = new SearchUserControl();

        Assert.True(control.SearchCommand.CanExecute(null));
    }

    [Fact]
    public void ResetCommand_CanExecute_ShouldAlwaysReturnTrue()
    {
        var control = new SearchUserControl();

        Assert.True(control.ResetCommand.CanExecute(null));
    }
}
