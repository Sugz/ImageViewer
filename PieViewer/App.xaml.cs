using CommunityToolkit.Mvvm.DependencyInjection;
using PieViewer.Services;
using PieViewer.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Windows;

namespace PieViewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    public App()
    {
        ConfigureServices();
        InitializeComponent();
    }


    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static void ConfigureServices()
    {
        Ioc.Default.ConfigureServices(
                    new ServiceCollection()
                    .AddSingleton<SettingsManager>()
                    .AddSingleton<MainViewModel>()
                    .BuildServiceProvider());
    }

    protected override void OnExit(ExitEventArgs e)
    {
        MainViewModel? mainViewModel = Ioc.Default.GetService<MainViewModel>();
        SettingsManager? settingsManager = Ioc.Default.GetService<SettingsManager>();

        if (mainViewModel != null)
        {
            settingsManager?.Serialize(mainViewModel);
            mainViewModel.Dispose();
        }

        settingsManager?.Save();
    }
}
