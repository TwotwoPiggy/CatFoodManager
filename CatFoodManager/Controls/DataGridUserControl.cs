using System.ComponentModel;
using System.Windows.Forms;

namespace CatFoodManager.Controls;

public partial class DataGridUserControl : UserControl
{
    private BindingSource _bindingSource = [];

    public event DataGridViewCellEventHandler? CellClicked;
    public event DataGridViewCellEventHandler? CellValueChanged;
    public event EventHandler? CurrentCellDirtyStateChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? DataSource
    {
        get => _bindingSource.DataSource;
        set
        {
            _bindingSource.DataSource = value;
            dataGridView.DataSource = _bindingSource;
        }
    }

    public DataGridView DataGridView => dataGridView;

    public DataGridUserControl()
    {
        InitializeComponent();
    }

    public void Initialize()
    {
        dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
        dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridView.AutoGenerateColumns = false;
    }

    public void ConfigureColumns(IEnumerable<DataGridViewColumn> columns, bool autoSizeColumnsMode = true)
    {
        dataGridView.Columns.Clear();
        dataGridView.AutoSizeColumnsMode = autoSizeColumnsMode ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;

        foreach (var column in columns)
        {
            if (column == null) continue;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dataGridView.Columns.Add(column);
        }
    }

    public void ClearColumns()
    {
        dataGridView.Columns.Clear();
    }

    public void AddColumn(DataGridViewColumn column)
    {
        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
        {
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        dataGridView.Columns.Add(column);
    }

    public T? GetCellValue<T>(int rowIndex, string columnName) where T : class
    {
        if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count) return default;
        var row = dataGridView.Rows[rowIndex];
        if (row == null) return default;
        var cell = row.Cells[columnName];
        if (cell == null || cell.Value == null) return default;
        return (T)cell.Value;
    }

    public long? GetIdInRow(int rowIndex, string idColumnName = "Id")
    {
        if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count) return default;
        var row = dataGridView.Rows[rowIndex];
        if (row == null) return default;
        var cell = row.Cells[idColumnName];
        if (cell == null || cell.Value == null) return default;
        return (long)cell.Value;
    }

    public void EndEdit()
    {
        dataGridView.EndEdit();
    }

    public void RefreshData()
    {
        dataGridView.Refresh();
    }

    private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        CellClicked?.Invoke(this, e);
    }

    private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        CellValueChanged?.Invoke(this, e);
    }

    private void dataGridView_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        CurrentCellDirtyStateChanged?.Invoke(this, e);
    }

    private void dataGridView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            dataGridView.EndEdit();
            e.Handled = true;
        }
    }

    private void dataGridView_Leave(object? sender, EventArgs e)
    {
        dataGridView.EndEdit();
    }
}
