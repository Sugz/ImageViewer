using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PieViewer.ViewModels
{
    internal partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageSource? _source;
    }
}
