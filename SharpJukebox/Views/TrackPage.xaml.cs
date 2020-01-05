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

        public TrackPage()
        {
            InitializeComponent();
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
    }
}
