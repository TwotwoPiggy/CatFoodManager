using CatFoodManager.Commands;
using System.ComponentModel;
using System.Windows.Forms;

namespace CatFoodManager.Controls;

public partial class SearchUserControl : UserControl
{
    private string _searchText = string.Empty;

    public event EventHandler? SearchClicked;
    public event EventHandler? ResetClicked;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            searchTextBox.Text = value;
        }
    }

    public RelayCommand SearchCommand { get; }
    public RelayCommand ResetCommand { get; }

    public SearchUserControl()
    {
        InitializeComponent();
        SearchCommand = new RelayCommand(_ => OnSearchClicked());
        ResetCommand = new RelayCommand(_ => OnResetClicked());
    }

    protected virtual void OnSearchClicked()
    {
        _searchText = searchTextBox.Text;
        SearchClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnResetClicked()
    {
        searchTextBox.Text = string.Empty;
        _searchText = string.Empty;
        ResetClicked?.Invoke(this, EventArgs.Empty);
    }

    private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            OnSearchClicked();
            e.Handled = true;
        }
    }

    private void searchTextBox_TextChanged(object sender, EventArgs e)
    {
        _searchText = searchTextBox.Text;
    }

    private void searchButton_Click(object sender, EventArgs e)
    {
        OnSearchClicked();
    }

    private void resetButton_Click(object sender, EventArgs e)
    {
        OnResetClicked();
    }
}
