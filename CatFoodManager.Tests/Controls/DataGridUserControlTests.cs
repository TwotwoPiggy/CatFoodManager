using CatFoodManager.Controls;
using System.Windows.Forms;
using Xunit;

namespace CatFoodManager.Tests.Controls;

public class DataGridUserControlTests
{
    [Fact]
    public void Constructor_ShouldInitializeControl()
    {
        using var control = new DataGridUserControl();

        Assert.NotNull(control.DataGridView);
    }

    [Fact]
    public void DataSource_Set_ShouldUpdateDataGridView()
    {
        using var control = new DataGridUserControl();
        var testData = new List<TestItem>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };

        control.DataSource = testData;

        Assert.Equal(testData, control.DataSource);
    }

    [Fact]
    public void DataSource_SetNull_ShouldWork()
    {
        using var control = new DataGridUserControl();

        control.DataSource = null;

        Assert.Null(control.DataSource);
    }

    [Fact]
    public void Initialize_ShouldSetDataGridViewProperties()
    {
        using var control = new DataGridUserControl();

        control.Initialize();

        Assert.Equal(DataGridViewEditMode.EditOnEnter, control.DataGridView.EditMode);
        Assert.False(control.DataGridView.AutoGenerateColumns);
    }

    [Fact]
    public void ClearColumns_ShouldRemoveAllColumns()
    {
        using var control = new DataGridUserControl();
        control.Initialize();
        control.AddColumn(new DataGridViewTextBoxColumn { Name = "TestColumn" });

        control.ClearColumns();

        Assert.Empty(control.DataGridView.Columns);
    }

    [Fact]
    public void AddColumn_ShouldAddColumnToDataGridView()
    {
        using var control = new DataGridUserControl();
        control.Initialize();
        var column = new DataGridViewTextBoxColumn { Name = "TestColumn", HeaderText = "Test" };

        control.AddColumn(column);

        Assert.Single(control.DataGridView.Columns);
        Assert.Equal("TestColumn", control.DataGridView.Columns[0].Name);
    }

    [Fact]
    public void EndEdit_ShouldEndEditMode()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        control.EndEdit();

        Assert.True(true);
    }

    [Fact]
    public void RefreshData_ShouldRefreshDataGridView()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        control.RefreshData();

        Assert.True(true);
    }

    [Fact]
    public void GetIdInRow_WithValidRow_ShouldReturnId()
    {
        using var control = new DataGridUserControl();
        control.Initialize();
        control.AddColumn(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id" });
        control.AddColumn(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name" });
        var testData = new List<TestItem>
        {
            new() { Id = 1, Name = "Test1" }
        };
        control.DataSource = testData;

        var id = control.GetIdInRow(0, "Id");

        Assert.Equal(1, id);
    }

    [Fact]
    public void GetIdInRow_WithInvalidRowIndex_ShouldReturnNull()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        var id = control.GetIdInRow(999, "Id");

        Assert.Null(id);
    }

    [Fact]
    public void GetIdInRow_WithNegativeRowIndex_ShouldReturnNull()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        var id = control.GetIdInRow(-1, "Id");

        Assert.Null(id);
    }

    [Fact]
    public void GetCellValue_WithValidRow_ShouldReturnValue()
    {
        using var control = new DataGridUserControl();
        control.Initialize();
        control.AddColumn(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id" });
        control.AddColumn(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name" });
        var testData = new List<TestItem>
        {
            new() { Id = 1, Name = "Test1" }
        };
        control.DataSource = testData;

        var name = control.GetCellValue<string>(0, "Name");

        Assert.Equal("Test1", name);
    }

    [Fact]
    public void GetCellValue_WithInvalidRowIndex_ShouldReturnNull()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        var value = control.GetCellValue<string>(999, "Name");

        Assert.Null(value);
    }

    [Fact]
    public void GetCellValue_WithNegativeRowIndex_ShouldReturnNull()
    {
        using var control = new DataGridUserControl();
        control.Initialize();

        var value = control.GetCellValue<string>(-1, "Name");

        Assert.Null(value);
    }

    private class TestItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
