using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieViewer.ViewModels
{
    internal sealed partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageSource? _source;

        [ObservableProperty]
        private double _x = 0;

        [ObservableProperty]
        private double _y = 0;


        public ImageViewModel()
        {
            
        }

        public ImageViewModel(ImageSource source)
        {
            Source = source;
        }
    }
}
