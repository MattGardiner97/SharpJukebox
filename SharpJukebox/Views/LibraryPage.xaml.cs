using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        private LibraryPageDisplayType _currentDisplayState = LibraryPageDisplayType.None;

        public object Data { get; private set; }

        public event Action<LibraryPage, AudioFile> TrackSelected;
        public event Action<Playlist> PlaylistSelected;
        public event Action<string> ArtistSelected;

        public LibraryPage()
        {
            InitializeComponent();
        }

        public T GetDataGridItems<T>()
        {
            return (T)Data;
        }

        public void SetDataGridItems(IEnumerable<AudioFile> Tracks)
        {
            this.Data = Tracks;
            dgTracks.DataContext = Data;
            ShowDataGrid(dgTracks);
            _currentDisplayState = LibraryPageDisplayType.Tracks;
        }

        public void SetDataGridItems(IEnumerable<Playlist> Playlists)
        {
            this.Data = Playlists;
            dgPlaylists.DataContext = Data;
            ShowDataGrid(dgPlaylists);
            _currentDisplayState = LibraryPageDisplayType.Playlists;
        }

        public void SetDataGridItems(IEnumerable<string> Artists)
        {
            this.Data = Artists;
            dgArtists.DataContext = Data;
            ShowDataGrid(dgArtists);
            _currentDisplayState = LibraryPageDisplayType.Artists;
        }

        public void SetPageHeader(string Header)
        {
            lblHeader.Content = Header;
        }

        private void ShowDataGrid(DataGrid SelectedGrid)
        {
            dgTracks.Visibility = Visibility.Collapsed;
            dgPlaylists.Visibility = Visibility.Collapsed;
            dgArtists.Visibility = Visibility.Collapsed;

            SelectedGrid.Visibility = Visibility.Visible;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object selectedObject = ((DataGridRow)sender).Item;
            switch (_currentDisplayState)
            {
                case LibraryPageDisplayType.Tracks:
                    TrackSelected(this, (AudioFile)selectedObject);
                    break;
                case LibraryPageDisplayType.Playlists:
                    PlaylistSelected((Playlist)selectedObject);
                    break;
                case LibraryPageDisplayType.Artists:
                    ArtistSelected((string)selectedObject);
                    break;
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }

    public enum LibraryPageDisplayType
    {
       None,Tracks,Playlists,Artists
    }
}
