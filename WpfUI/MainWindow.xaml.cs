using ContactBook.ViewModels;
using System.Windows;

namespace ContactBook;

public partial class MainWindow : Window
{
    public MainWindow(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}
