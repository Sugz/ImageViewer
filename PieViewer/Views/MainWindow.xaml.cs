using CommunityToolkit.Mvvm.DependencyInjection;
using PieViewer.ViewModels;
using System.Windows;

namespace PieViewer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainViewModel? mainViewModel = Ioc.Default.GetService<MainViewModel>();
        if (mainViewModel is not null)
        {
            DataContext = mainViewModel;
            WindowStartupLocation = mainViewModel.WindowStartupLocation;
            Left = mainViewModel.WindowLeft;
            Top = mainViewModel.WindowTop;
        }
    }
}
