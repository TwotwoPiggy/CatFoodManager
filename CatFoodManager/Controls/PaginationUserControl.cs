using CatFoodManager.Commands;
using System.ComponentModel;
using System.Windows.Forms;

namespace CatFoodManager.Controls;

public partial class PaginationUserControl : UserControl
{
    private int _currentPage = 1;
    private int _pageCount = 1;
    private int _totalCount;

    public event EventHandler? FirstPageClicked;
    public event EventHandler? PreviousPageClicked;
    public event EventHandler? NextPageClicked;
    public event EventHandler? LastPageClicked;
    public event EventHandler<int>? GoToPageClicked;
    public event EventHandler<int>? PageSizeChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            UpdateLabels();
            UpdateButtonStates();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PageCount
    {
        get => _pageCount;
        set
        {
            _pageCount = value;
            UpdateLabels();
            UpdateButtonStates();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            _totalCount = value;
            UpdateLabels();
        }
    }

    public int PageSize => int.TryParse(pageSizeComboBox.SelectedItem?.ToString(), out int pageSize) ? pageSize : 50;

    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < PageCount;

    public RelayCommand FirstPageCommand { get; }
    public RelayCommand PreviousPageCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand LastPageCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand<int> GoToPageCommand { get; }

    public PaginationUserControl()
    {
        InitializeComponent();
        FirstPageCommand = new RelayCommand(_ => OnFirstPageClicked(), _ => CanGoPrevious);
        PreviousPageCommand = new RelayCommand(_ => OnPreviousPageClicked(), _ => CanGoPrevious);
        NextPageCommand = new RelayCommand(_ => OnNextPageClicked(), _ => CanGoNext);
        LastPageCommand = new RelayCommand(_ => OnLastPageClicked(), _ => CanGoNext);
        ResetCommand = new RelayCommand(_ => Reset());
        GoToPageCommand = new RelayCommand<int>(page => OnGoToPageClicked(page));
    }

    public void Initialize(int pageSizeIndex = 2)
    {
        pageSizeComboBox.SelectedIndex = pageSizeIndex;
    }

    private void UpdateLabels()
    {
        totalLabel.Text = $"共 {_totalCount} 条记录";
        pageInfoLabel.Text = $"当前页 {_currentPage}/{_pageCount}";
    }

    private void UpdateButtonStates()
    {
        firstButton.Enabled = previousButton.Enabled = CanGoPrevious;
        lastButton.Enabled = nextButton.Enabled = CanGoNext;
        FirstPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }

    protected virtual void OnFirstPageClicked()
    {
        if (!CanGoPrevious) return;
        CurrentPage = 1;
        FirstPageClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnPreviousPageClicked()
    {
        if (!CanGoPrevious) return;
        CurrentPage--;
        PreviousPageClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnNextPageClicked()
    {
        if (!CanGoNext) return;
        CurrentPage++;
        NextPageClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnLastPageClicked()
    {
        if (!CanGoNext) return;
        CurrentPage = PageCount;
        LastPageClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnGoToPageClicked(int page)
    {
        if (page < 1 || page > PageCount)
        {
            MessageBox.Show($"待跳转的页数超出范围, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        CurrentPage = page;
        GoToPageClicked?.Invoke(this, page);
    }

    public void Reset()
    {
        CurrentPage = 1;
        jumpPageTextBox.Text = string.Empty;
    }

    private void firstButton_Click(object sender, EventArgs e)
    {
        OnFirstPageClicked();
    }

    private void previousButton_Click(object sender, EventArgs e)
    {
        OnPreviousPageClicked();
    }

    private void nextButton_Click(object sender, EventArgs e)
    {
        OnNextPageClicked();
    }

    private void lastButton_Click(object sender, EventArgs e)
    {
        OnLastPageClicked();
    }

    private void goToPageButton_Click(object sender, EventArgs e)
    {
        if (int.TryParse(jumpPageTextBox.Text, out int page))
        {
            OnGoToPageClicked(page);
        }
        else
        {
            MessageBox.Show("待跳转的页数不合规, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void pageSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        PageSizeChanged?.Invoke(this, PageSize);
    }
}
