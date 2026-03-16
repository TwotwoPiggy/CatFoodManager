using CatFoodManager.Controls;
using System.Windows.Forms;
using Xunit;

namespace CatFoodManager.Tests.Controls;

public class PaginationUserControlTests
{
    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        var control = new PaginationUserControl();

        Assert.NotNull(control.FirstPageCommand);
        Assert.NotNull(control.PreviousPageCommand);
        Assert.NotNull(control.NextPageCommand);
        Assert.NotNull(control.LastPageCommand);
        Assert.NotNull(control.ResetCommand);
        Assert.NotNull(control.GoToPageCommand);
    }

    [Fact]
    public void CurrentPage_DefaultValue_ShouldBeOne()
    {
        var control = new PaginationUserControl();

        Assert.Equal(1, control.CurrentPage);
    }

    [Fact]
    public void PageCount_DefaultValue_ShouldBeOne()
    {
        var control = new PaginationUserControl();

        Assert.Equal(1, control.PageCount);
    }

    [Fact]
    public void TotalCount_DefaultValue_ShouldBeZero()
    {
        var control = new PaginationUserControl();

        Assert.Equal(0, control.TotalCount);
    }

    [Fact]
    public void CanGoPrevious_WhenCurrentPageIsOne_ShouldReturnFalse()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 1;
        control.PageCount = 10;

        Assert.False(control.CanGoPrevious);
    }

    [Fact]
    public void CanGoPrevious_WhenCurrentPageIsGreaterThanOne_ShouldReturnTrue()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 2;
        control.PageCount = 10;

        Assert.True(control.CanGoPrevious);
    }

    [Fact]
    public void CanGoNext_WhenCurrentPageEqualsPageCount_ShouldReturnFalse()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 10;
        control.PageCount = 10;

        Assert.False(control.CanGoNext);
    }

    [Fact]
    public void CanGoNext_WhenCurrentPageLessThanPageCount_ShouldReturnTrue()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 5;
        control.PageCount = 10;

        Assert.True(control.CanGoNext);
    }

    [Fact]
    public void FirstPageCommand_Execute_ShouldRaiseFirstPageClicked()
    {
        var control = new PaginationUserControl();
        control.PageCount = 10;
        control.CurrentPage = 5;
        var eventRaised = false;
        control.FirstPageClicked += (s, e) => eventRaised = true;

        control.FirstPageCommand.Execute(null);

        Assert.True(eventRaised);
        Assert.Equal(1, control.CurrentPage);
    }

    [Fact]
    public void PreviousPageCommand_Execute_ShouldRaisePreviousPageClicked()
    {
        var control = new PaginationUserControl();
        control.PageCount = 10;
        control.CurrentPage = 5;
        var eventRaised = false;
        control.PreviousPageClicked += (s, e) => eventRaised = true;

        control.PreviousPageCommand.Execute(null);

        Assert.True(eventRaised);
        Assert.Equal(4, control.CurrentPage);
    }

    [Fact]
    public void NextPageCommand_Execute_ShouldRaiseNextPageClicked()
    {
        var control = new PaginationUserControl();
        control.PageCount = 10;
        control.CurrentPage = 5;
        var eventRaised = false;
        control.NextPageClicked += (s, e) => eventRaised = true;

        control.NextPageCommand.Execute(null);

        Assert.True(eventRaised);
        Assert.Equal(6, control.CurrentPage);
    }

    [Fact]
    public void LastPageCommand_Execute_ShouldRaiseLastPageClicked()
    {
        var control = new PaginationUserControl();
        control.PageCount = 10;
        control.CurrentPage = 5;
        var eventRaised = false;
        control.LastPageClicked += (s, e) => eventRaised = true;

        control.LastPageCommand.Execute(null);

        Assert.True(eventRaised);
        Assert.Equal(10, control.CurrentPage);
    }

    [Fact]
    public void GoToPageCommand_Execute_ShouldRaiseGoToPageClicked()
    {
        var control = new PaginationUserControl();
        control.PageCount = 10;
        control.CurrentPage = 1;
        var eventRaised = false;
        var targetPage = 0;
        control.GoToPageClicked += (s, page) =>
        {
            eventRaised = true;
            targetPage = page;
        };

        control.GoToPageCommand.Execute(5);

        Assert.True(eventRaised);
        Assert.Equal(5, targetPage);
        Assert.Equal(5, control.CurrentPage);
    }

    [Fact]
    public void Reset_ShouldSetCurrentPageToOne()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 10;

        control.Reset();

        Assert.Equal(1, control.CurrentPage);
    }

    [Fact]
    public void Initialize_ShouldSetPageSizeIndex()
    {
        var control = new PaginationUserControl();

        control.Initialize(1);

        Assert.Equal("20", control.PageSize.ToString());
    }

    [Fact]
    public void PageSize_Default_ShouldReturn50()
    {
        var control = new PaginationUserControl();

        Assert.Equal(50, control.PageSize);
    }

    [Fact]
    public void FirstPageCommand_CanExecute_WhenCanGoPreviousIsFalse_ShouldReturnFalse()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 1;
        control.PageCount = 10;

        Assert.False(control.FirstPageCommand.CanExecute(null));
    }

    [Fact]
    public void FirstPageCommand_CanExecute_WhenCanGoPreviousIsTrue_ShouldReturnTrue()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 5;
        control.PageCount = 10;

        Assert.True(control.FirstPageCommand.CanExecute(null));
    }

    [Fact]
    public void NextPageCommand_CanExecute_WhenCanGoNextIsFalse_ShouldReturnFalse()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 10;
        control.PageCount = 10;

        Assert.False(control.NextPageCommand.CanExecute(null));
    }

    [Fact]
    public void NextPageCommand_CanExecute_WhenCanGoNextIsTrue_ShouldReturnTrue()
    {
        var control = new PaginationUserControl();
        control.CurrentPage = 5;
        control.PageCount = 10;

        Assert.True(control.NextPageCommand.CanExecute(null));
    }
}
