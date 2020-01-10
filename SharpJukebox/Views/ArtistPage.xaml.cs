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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for ArtistPage.xaml
    /// </summary>
    public partial class ArtistPage : Page
    {
        public event Action<string> ArtistSelected;
        public event Action<string> PlayArtistSelected;

        public ArtistPage(String Header, IEnumerable<string> Artists)
        {
            InitializeComponent();

            dgArtists.DataContext = Artists;
            lblHeader.Content = Header;
        }

        private void dgArtists_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ArtistSelected == null)
                return;

            string selectedArtist = (string)dgArtists.SelectedItems.Cast<string>().First();
            ArtistSelected(selectedArtist);

        }

        private void ContextMenuPlay_Clicked(object sender, RoutedEventArgs e)
        {
            if (PlayArtistSelected == null)
                return;

            string selectedArtist = (string)dgArtists.SelectedItems.Cast<string>().First();
            PlayArtistSelected(selectedArtist);
        }

        private void ContextMenuView_Clicked(object sender, RoutedEventArgs e)
        {
            if (ArtistSelected == null)
                return;

            string selectedArtist = (string)dgArtists.SelectedItems.Cast<string>().First();
            ArtistSelected(selectedArtist);
        }
    }
}
