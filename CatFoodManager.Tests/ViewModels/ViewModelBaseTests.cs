using CatFoodManager.ViewModels;
using Xunit;

namespace CatFoodManager.Tests.ViewModels;

public class ViewModelBaseTests
{
    [Fact]
    public void OnPropertyChanged_ShouldRaisePropertyChangedEvent()
    {
        var viewModel = new TestableViewModel();
        var eventRaised = false;
        string? propertyName = null;

        viewModel.PropertyChanged += (sender, e) =>
        {
            eventRaised = true;
            propertyName = e.PropertyName;
        };

        viewModel.TestProperty = "TestValue";

        Assert.True(eventRaised);
        Assert.Equal(nameof(TestableViewModel.TestProperty), propertyName);
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldReturnTrue()
    {
        var viewModel = new TestableViewModel();
        var result = viewModel.SetTestProperty("NewValue");

        Assert.True(result);
        Assert.Equal("NewValue", viewModel.TestProperty);
    }

    [Fact]
    public void SetProperty_WhenValueIsSame_ShouldReturnFalse()
    {
        var viewModel = new TestableViewModel();
        viewModel.SetTestProperty("SameValue");

        var result = viewModel.SetTestProperty("SameValue");

        Assert.False(result);
    }

    [Fact]
    public void SetProperty_ShouldRaisePropertyChangedEvent()
    {
        var viewModel = new TestableViewModel();
        var eventRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(TestableViewModel.TestProperty))
            {
                eventRaised = true;
            }
        };

        viewModel.TestProperty = "TestValue";

        Assert.True(eventRaised);
    }

    private class TestableViewModel : ViewModelBase
    {
        private string _testProperty = string.Empty;

        public string TestProperty
        {
            get => _testProperty;
            set => SetProperty(ref _testProperty, value);
        }

        public bool SetTestProperty(string value) => SetProperty(ref _testProperty, value);
    }
}
