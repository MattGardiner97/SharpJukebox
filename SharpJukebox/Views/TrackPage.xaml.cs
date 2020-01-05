using System;
using System.Collections.Generic;
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
    /// Interaction logic for TrackPage.xaml
    /// </summary>
    public partial class TrackPage : Page
    {
        private IEnumerable<AudioFile> _tracks;

        //Arg1: Selected tracks
        //Arg2: All Tracks
        public event Action<IEnumerable<AudioFile>, IEnumerable<AudioFile>> TracksSelected;
        public event Action<string> ArtistSelected;
        public event Action<string, string> AlbumSelected; //Arg1: Artist, Arg2: Album
        public event Action<IEnumerable<AudioFile>> AddToPlaylistSelected;

        public TrackPage()
        {
            InitializeComponent();
        }

        public TrackPage(string Header, IEnumerable<AudioFile> Tracks)
        {
            InitializeComponent();

            SetPageHeader(Header);
            SetDataContext(Tracks);
        }
        
        public void SetDataContext(IEnumerable<AudioFile> Tracks)
        {
            _tracks = Tracks;
            dgTracks.DataContext = Tracks;
        }

        public void SetPageHeader(string Header)
        {
            lblHeader.Content = Header;
        }

        private void dgTracks_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AudioFile[] selected = new AudioFile[dgTracks.SelectedItems.Count];
            dgTracks.SelectedItems.CopyTo(selected,0);
            TracksSelected(selected, _tracks);
        }

        private void MenuItemPlay_Clicked(object sender, RoutedEventArgs e)
        {
            IEnumerable<AudioFile> selected = dgTracks.SelectedItems.Cast<AudioFile>();
            TracksSelected(selected, _tracks);
        }

        private void MenuItemArist_Clicked(object sender, RoutedEventArgs e)
        {
            AudioFile firstSelected = (AudioFile)dgTracks.SelectedItems[0];
            ArtistSelected(firstSelected.Artist);
        }

        private void MenuItemAlbum_Clicked(object sender, RoutedEventArgs e)
        {
            AudioFile firstSelected = (AudioFile)dgTracks.SelectedItems[0];
            AlbumSelected(firstSelected.Artist, firstSelected.Album);
        }

        private void MenuItemAddPlaylist_Clicked(object sender, RoutedEventArgs e)
        {
            IEnumerable<AudioFile> tracks = dgTracks.SelectedItems.Cast<AudioFile>();
            AddToPlaylistSelected(tracks);
        }
    }
}
