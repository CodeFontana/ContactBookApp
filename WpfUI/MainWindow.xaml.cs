using WpfUI.ViewModels;
using System.Windows;

namespace WpfUI;

public partial class MainWindow : Window
{
    public MainWindow(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}
