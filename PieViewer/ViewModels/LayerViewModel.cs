using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieViewer.ViewModels
{
    internal sealed partial class LayerViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageViewModel _bitmap;

        [ObservableProperty]
        private ImageViewModel? _mask;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private bool _visible = true;

        [ObservableProperty]
        private bool _maskVisible = true;

        [ObservableProperty]
        private bool _locked;

        public LayerViewModel(string name)
        {
            Name = name;
        }
    }
}
