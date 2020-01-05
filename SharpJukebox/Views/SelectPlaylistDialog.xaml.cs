using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for SelectPlaylistDialog.xaml
    /// </summary>
    public partial class SelectPlaylistDialog : Window
    {
        private IEnumerable<Playlist> _playlists;

        public Playlist SelectedPlaylist { get; private set; }


        public SelectPlaylistDialog(IEnumerable<Playlist> Playlists)
        {
            _playlists = Playlists;

            InitializeComponent();

            playlistListbox.ItemsSource = Playlists.Select(Playlist => Playlist.Name);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (SelectedPlaylist != null)
                return;

            Close();
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            SelectedPlaylist = _playlists.FirstOrDefault(playlist => playlist.Name == (string)((ListBoxItem)sender).Content);
            Close();
        }
    }
}
