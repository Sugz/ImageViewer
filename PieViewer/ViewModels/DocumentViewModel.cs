using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PieViewer.ViewModels
{
    internal sealed partial class DocumentViewModel : ObservableObject
    {
        public ObservableCollection<LayerViewModel> Layers { get; } = new();

        [ObservableProperty]
        private string _name;

        public DocumentViewModel(string name)
        {
            Name = _name;
        }
    }
}
