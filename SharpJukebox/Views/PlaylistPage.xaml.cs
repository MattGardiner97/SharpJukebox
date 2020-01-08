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

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for PlaylistPage.xaml
    /// </summary>
    public partial class PlaylistPage : Page
    {
        private const string PAGE_HEADER = "Playlists";

        private PlaylistManager _playlistManager;

        public event Action<Playlist> PlaylistSelected;

        public PlaylistPage(PlaylistManager PlaylistManager)
        {
            _playlistManager = PlaylistManager;

            InitializeComponent();

            lblHeader.Content = PAGE_HEADER;
            dgPlaylists.DataContext = _playlistManager.Playlists;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Playlist selected = (Playlist)dgPlaylists.SelectedItems[0];
            PlaylistSelected(selected);
        }

        private void dgPlaylists_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void NewPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TextInputDialog dialog = new TextInputDialog()
            {
                Owner = Window.GetWindow(this)
            };
            dialog.ShowDialog("Enter Playlist Name");

            if (dialog.DialogResult == true)
            {
                CreatePlaylist(dialog.Result);
            }
        }

        private void CreatePlaylist(string PlaylistName)
        {
            Playlist newPlaylist = _playlistManager.CreatePlaylist(PlaylistName);
            dgPlaylists.DataContext = _playlistManager.Playlists;
        }
    }
}
