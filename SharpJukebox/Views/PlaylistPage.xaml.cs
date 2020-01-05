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
        public event Action<Playlist> PlaylistSelected;

        public PlaylistPage()
        {
            InitializeComponent();
        }

        public PlaylistPage(string Header, IEnumerable<Playlist> Playlists)
        {
            InitializeComponent();

            SetPageHeader(Header);
            SetDataContext(Playlists);
        }

        public void SetPageHeader(string Header)
        {
            lblHeader.Content = Header;
        }

        public void SetDataContext(IEnumerable<Playlist> Playlists)
        {
            dgPlaylists.DataContext = Playlists;
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
    }
}
