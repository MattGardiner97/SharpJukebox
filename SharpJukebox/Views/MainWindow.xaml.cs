using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CONFIG_FILENAME = "config.json";
        private const string SEARCH_PLACEHOLDER = "Search...";

        private Configuration _config;

        private MMDevice _audioDevice;
        private AudioDeviceLocater _audioDeviceLocater;
        private ITrackLocater _fileLocater;
        private IPlaylistReader _playlistReader;
        private MetadataExtractor _metadataExtractor;
        private LocalLibraryManager _localLibraryManager;
        private PlaylistManager _playlistManager;
        private NavigationManager _navigationManager;
        private MusicPlayer _musicPlayer;
        private MusicShuffler _shuffler;
        private LibraryPage _searchPage;

        //Holds the current page before beginning a search
        private Page _previousPage;

        public MainWindow()
        {
            _audioDeviceLocater = new AudioDeviceLocater();

            string configFilename = System.IO.Path.Join(AppContext.BaseDirectory, CONFIG_FILENAME);
            ConfigurationLoader configLoader = new ConfigurationLoader();
            configLoader.LoadFromFile(configFilename);
            _config = configLoader.Configuration;

            List<string> searchLocations = new List<string>();
            searchLocations.Add(System.IO.Path.Join(AppContext.BaseDirectory, "Music"));
            searchLocations.AddRange(_config.SearchLocations);
            _fileLocater = new LocalTrackLocater(searchLocations.ToArray());

            _metadataExtractor = new MetadataExtractor();
            _localLibraryManager = new LocalLibraryManager(_fileLocater, _metadataExtractor);
            _navigationManager = new NavigationManager();
            _musicPlayer = new MusicPlayer();
            _shuffler = new MusicShuffler();
            _playlistReader = new LocalPlaylistReader(_localLibraryManager, System.IO.Path.Join(AppContext.BaseDirectory, "Playlists"));
            _playlistManager = new PlaylistManager(_playlistReader);

            _searchPage = new LibraryPage(_playlistManager.Playlists);



            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialise audio devices
            _audioDeviceLocater.LocateDevices();
            _audioDevice = _audioDeviceLocater.Devices[0];
            _musicPlayer.SetAudioDevice(_audioDevice);

            //Load local files
            _localLibraryManager.UpdateFiles();
            _playlistManager.Load();

            //Set event handlers
            //_searchPage.TrackSelected += LibraryPage_TrackSelected;
            playPauseButton.Pressed += playPauseButton_Clicked;

            //Display initial page
            ShowTracksPage("All", _localLibraryManager.Tracks);
            lbiAll.IsSelected = true;
            lbiAll.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _musicPlayer.Dispose();
            _audioDevice.Dispose();
            _audioDevice = null;
        }

        private void playPauseButton_Clicked()
        {
            if (_musicPlayer.PlayState == PlayState.Playing)
            {
                _musicPlayer.Pause();
                playPauseButton.DisplayState = PlayButtonDisplayState.Play;
            }
            else if (_musicPlayer.PlayState == PlayState.Paused)
            {
                _musicPlayer.Resume();
                playPauseButton.DisplayState = PlayButtonDisplayState.Pause;

            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            listboxNavigation.SelectedItem = null;

            UpdateSearchBoxPlaceholder();
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSearchBoxPlaceholder();
        }

        private void UpdateSearchBoxPlaceholder()
        {
            if (txtSearch.Text == SEARCH_PLACEHOLDER && txtSearch.IsFocused == true)
            {
                txtSearch.Foreground = Brushes.Black;
                txtSearch.Text = "";
            }
            if (string.IsNullOrEmpty(txtSearch.Text) && txtSearch.IsFocused == false)
            {
                txtSearch.Foreground = Brushes.Gray;
                txtSearch.Text = SEARCH_PLACEHOLDER;
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateSearchBoxPlaceholder();

            if (txtSearch.IsFocused == false)
                return;

            string query = txtSearch.Text;
            if (query == SEARCH_PLACEHOLDER || string.IsNullOrEmpty(query))
            {
                if (_previousPage != null)
                {
                    LibraryFrame.Content = _previousPage;
                    _previousPage = null;
                }
                return;
            }
            else
            {
                if (_previousPage == null)
                    _previousPage = (Page)LibraryFrame.Content;

                var searchResults = _localLibraryManager.Search(query);
                _searchPage.SetDataGridItems(searchResults);
                _searchPage.SetPageHeader($"Search results for '{query}'");
                if (LibraryFrame.Content != _searchPage)
                    LibraryFrame.Content = _searchPage;
            }
        }

        private void lbiAll_Selected(object sender, RoutedEventArgs e)
        {
            ShowTracksPage("All", _localLibraryManager.Tracks);
        }

        private void LibraryPage_TrackSelected(IEnumerable<AudioFile> Selected, IEnumerable<AudioFile> AllTracks)
        {
            IEnumerable<AudioFile> tracksToPlay = null;
            if(_musicPlayer.Shuffle == true)
            {
                if(Selected.Count() > 1)
                {
                    tracksToPlay = _shuffler.Shuffle(Selected);
                }
                else
                {
                    tracksToPlay = _shuffler.Shuffle(AllTracks,Selected.First());
                }
            }
            else
            {

                var result = new List<AudioFile>(AllTracks);
                result.Remove(Selected.First());
                result.Insert(0, Selected.First());
                tracksToPlay = result;
            }

            _musicPlayer.ClearQueue();
            _musicPlayer.AddToQueue(tracksToPlay);
            _musicPlayer.Play();
            playPauseButton.DisplayState = PlayButtonDisplayState.Pause;
        }

        private void Librarypage_PlaylistSelected(Playlist Playlist)
        {
            Playlist selectedPlaylist = Playlist;
            ShowTracksPage(selectedPlaylist.Name, selectedPlaylist.Tracks);
        }

        private void LibraryPage_ArtistSelected(string Artist)
        {
            string artistName = Artist;
            ShowTracksPage(artistName, _localLibraryManager.FindByArtist(artistName));
        }

        public void ShowTracksPage(string PageHeader, ReadOnlyCollection<AudioFile> Tracks, bool ClearSearch = true)
        {
            TrackPage newPage = new TrackPage();
            newPage.SetPageHeader(PageHeader);
            newPage.SetDataContext(Tracks);
            newPage.TracksSelected += LibraryPage_TrackSelected;
            LibraryFrame.Content = newPage;
            if (ClearSearch == true)
                txtSearch.Clear();
        }

        private void ShowPlaylistPage()
        {
            LibraryPage newPage = new LibraryPage(_playlistManager.Playlists);
            newPage.SetPageHeader("Playlists");
            newPage.SetDataGridItems(_playlistManager.Playlists);
            newPage.PlaylistSelected += Librarypage_PlaylistSelected;
            LibraryFrame.Content = newPage;
            txtSearch.Clear();
        }

        private void ShowArtistPage()
        {
            string[] artists = _localLibraryManager.Tracks.Select(track => track.Artist).Distinct().ToArray();

            LibraryPage newPage = new LibraryPage(_playlistManager.Playlists);
            newPage.SetPageHeader("Artists");
            newPage.SetDataGridItems(artists);
            newPage.ArtistSelected += LibraryPage_ArtistSelected;
            LibraryFrame.Content = newPage;
            txtSearch.Clear();
        }

        private void lbiQueue_Selected(object sender, RoutedEventArgs e)
        {
            var queue = _musicPlayer.Queue;
            ShowTracksPage("Queue", queue);
        }

        private void lbiPlaylists_Selected(object sender, RoutedEventArgs e)
        {
            ShowPlaylistPage();
        }

        private void lbiArtists_Selected(object sender, RoutedEventArgs e)
        {
            ShowArtistPage();
        }
    }
}