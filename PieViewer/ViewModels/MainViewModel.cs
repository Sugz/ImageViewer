using CommunityToolkit.Mvvm.ComponentModel;
using PieViewer.Services;
using Microsoft.Extensions.Configuration;
using PieViewer.Core;
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
using ISerializable = PieViewer.Services.ISerializable;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace PieViewer.ViewModels;

internal sealed partial class MainViewModel : ObservableObject, ISerializable, IDisposable
{
    private readonly PrintScreenListener _printScreenListener;
    private readonly ScreenCapture _screenCapture;


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
    [property: JsonIgnore]
    private ImageViewModel? _currentImage;

    [ObservableProperty]
    [property: JsonIgnore]
    private ImageViewModel? _cursorImage;


    [property: JsonIgnore]
    public ObservableCollection<LayerViewModel> Layers { get; } = new();


    
    private RelayCommand _myCommand = new RelayCommand(onMyBtnClick);
    public RelayCommand MyCommand => _myCommand;

    private static void onMyBtnClick()
    {
        Debug.WriteLine("The buton was clicked");
    }

    public string GetSerializedName() => "MainViewModel";

    public void Deserialize(JsonNode jsonObject)
    {
        WindowStartupLocation = WindowStartupLocation.Manual;
        WindowWidth = jsonObject[nameof(WindowWidth)]!.GetValue<double>();
        WindowHeight = jsonObject[nameof(WindowHeight)]!.GetValue<double>();
        WindowLeft = jsonObject[nameof(WindowLeft)]!.GetValue<double>();
        WindowTop = jsonObject[nameof(WindowTop)]!.GetValue<double>();
    }

    public MainViewModel(SettingsManager settingsManager)
    {
        settingsManager.Deserialize(this);



        _printScreenListener = new PrintScreenListener();
        _printScreenListener.Install();
        //_printScreenListener.PrintScreenAsync += OnPrintScreen;
        _printScreenListener.PrintScreen += OnPrintScreen;

        _screenCapture = new ScreenCapture();
    }

    private void OnPrintScreen()
    {
        //CurrentImage = new(_screenCapture.CaptureDesktop());
        //CursorImage = new(_screenCapture.CaptureImageCursor());

        LayerViewModel capture = new("Screen Capture");
        capture.Bitmap = new ImageViewModel(_screenCapture.CaptureDesktop());
        capture.Locked = true;
        Layers.Add(capture);

        System.Drawing.Point point = new();
        LayerViewModel cursor = new("Cursor");
        cursor.Bitmap = new ImageViewModel(ScreenCapture.CaptureImageCursor(ref point));
        cursor.Bitmap.X = point.X;
        cursor.Bitmap.Y = point.Y;
        Layers.Add(cursor);
    }

    /*
    private async Task OnPrintScreen()
    {
        // The BitmapSource source is created on a background thread (non ui) since it is async
        // Hence it must be freeze before assigning it to avoid the error:
        // "Must create DependencySource on same Thread as the DependencyObject"
        // https://stackoverflow.com/questions/4690781/must-create-dependencysource-on-same-thread-as-the-dependencyobject
        await Task.Run(() =>
        {
            BitmapSource bitmapSource = _screenCapture.CaptureDesktop();
            bitmapSource.Freeze();
            CurrentImage = new(bitmapSource);

            //SharedBitmapSource bitmapSource = new(_screenCapture.CaptureDesktopAsBitmap());
            //bitmapSource.Freeze();
            //CurrentImage = new(bitmapSource);
        });
    }
    */

    public void Dispose()
    {
        _printScreenListener.Uninstall();
    }


    
}
