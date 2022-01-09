using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using ZiPatch;
using ZiPatch.Chunks;
using ZiPatch_Explorer.Annotations;

namespace ZiPatch_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Reader? _patchFile;
        private List<Etry> _chunks = new();
        public List<Etry> Chunks
        {
            get => _chunks;
            set
            {
                _chunks = value;
                OnPropertyChanged();
            }
        }

        private string _currentState = "Select a ZiPatch file to begin File -> Open";
        public string CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            switch (element?.Name)
            {
                case "OpenMenuItem":
                    var ofp = new OpenFileDialog
                    {
                        Filter = "ZiPatch (*.patch)|*.patch",
                        RestoreDirectory = true,
                        Title = "Select a ZiPatch file from FFXIV 1.0",
                        Multiselect = false
                    };

                    if (ofp.ShowDialog() == true)
                    {
                        Chunks.Clear();

                        CurrentState = $"Loading: {ofp.SafeFileName}";
                        
                        _patchFile = new Reader(ofp.FileName);
                        var f = await _patchFile.GetChunksAsync();

                        var header = f.FirstOrDefault(x => x.ChunkType == ChunkType.Fhdr) as Fhdr;
                        CurrentState =
                            $"Version: 0x{header.Version:x0} Entries: {header.NumberEntryFile:N0} AddDir: {header.NumberAddDir:N0} DelDir: {header.NumberDeleteDir:N0}";

                        Chunks = f.OfType<Etry>().ToList();
                    }
                    else
                    {
                        if (_patchFile == null)
                        {
                            CurrentState = $"Select a ZiPatch file to begin File -> Open";
                        }
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}