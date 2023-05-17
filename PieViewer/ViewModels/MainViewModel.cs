using CommunityToolkit.Mvvm.ComponentModel;
using ImageViewer.Services;
using Microsoft.Extensions.Configuration;
using PieViewer.Services;
using PieViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ISerializable = ImageViewer.Services.ISerializable;

namespace ImageViewer.ViewModels;

internal sealed partial class MainViewModel : ObservableObject, ISerializable
{
    [JsonIgnore]
    public WindowStartupLocation WindowStartupLocation = WindowStartupLocation.CenterScreen;

    [JsonIgnore]
    public static string WindowTitle => "Pie Viewer";

    [ObservableProperty]
    private double windowWidth = 1200;

    [ObservableProperty]
    private double windowHeight = 800;

    [ObservableProperty]
    private double windowLeft;

    [ObservableProperty]
    private double windowTop;


    [ObservableProperty]
    private ImageViewModel? _currentImage;



    public string GetSerializedName() => "MainViewModel";

    public void Deserialize(JsonNode jsonObject)
    {
        WindowStartupLocation = WindowStartupLocation.Manual;
        WindowWidth = jsonObject[nameof(WindowWidth)]!.GetValue<double>();
        WindowHeight = jsonObject[nameof(WindowHeight)]!.GetValue<double>();
        WindowLeft = jsonObject[nameof(WindowLeft)]!.GetValue<double>();
        WindowTop = jsonObject[nameof(WindowTop)]!.GetValue<double>();
    }

    public MainViewModel(SettingsManager settingsManager, PrintScreenListener printScreenListener)
    {
        settingsManager.Deserialize(this);

        printScreenListener.Install();
        printScreenListener.PrintScreen += (async () => await Task.Run(OnGlobalPrintScreen));
    }


    private void OnGlobalPrintScreen()
    {
        Debug.WriteLine("PrintScreen");
    }
}
