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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

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
        private IPlaylistWriter _playlistWriter;
        private MetadataExtractor _metadataExtractor;
        private LocalLibraryManager _localLibraryManager;
        private PlaylistManager _playlistManager;
        private NavigationManager _navigationManager;
        private MusicPlayer _musicPlayer;
        private MusicShuffler _shuffler;
        private TrackPage _searchPage;

        //Animation variables
        private Storyboard _trackProgressStoryboard;

        private Style _sidebarSelectedStyle;

        //Holds the current page before beginning a search
        private Page _previousPage;

        public MainWindow()
        {
            _audioDeviceLocater = new AudioDeviceLocater();

            //Load app configuration
            string configFilename = System.IO.Path.Join(AppContext.BaseDirectory, CONFIG_FILENAME);
            ConfigurationLoader configLoader = new ConfigurationLoader();
            configLoader.LoadFromFile(configFilename);
            _config = configLoader.Configuration;
            
            //Get list of Music search locations
            List<string> searchLocations = new List<string>();
            searchLocations.Add(System.IO.Path.Join(AppContext.BaseDirectory, "Music"));
            searchLocations.AddRange(_config.SearchLocations);
            _fileLocater = new LocalTrackLocater(searchLocations.ToArray());

            //Instantiate requires classes
            _metadataExtractor = new MetadataExtractor();
            _localLibraryManager = new LocalLibraryManager(_fileLocater, _metadataExtractor);
            _navigationManager = new NavigationManager();
            _musicPlayer = new MusicPlayer();
            _shuffler = new MusicShuffler();
            _playlistReader = new LocalPlaylistReader(_localLibraryManager, System.IO.Path.Join(AppContext.BaseDirectory, "Playlists"));
            _playlistWriter = new LocalPlaylistWriter(System.IO.Path.Join(AppContext.BaseDirectory,"Playlists"));
            _playlistManager = new PlaylistManager(_playlistReader,_playlistWriter);

            //Initialise the search page
            _searchPage = new TrackPage();


            InitializeComponent();
        }

        /////////////////////////
        //Window Event Handlers//
        /////////////////////////
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialise audio devices
            _audioDeviceLocater.LocateDevices();
            _audioDevice = _audioDeviceLocater.Devices.First();
            _musicPlayer.SetAudioDevice(_audioDevice);

            //Load local files
            _localLibraryManager.UpdateFiles();
            _playlistManager.Load();

            //Set event handlers
            _searchPage.TracksSelected += TrackPage_TracksSelected;
            playPauseButton.Pressed += playPauseButton_Clicked;


            //Load the required style
            _sidebarSelectedStyle = (Style)Application.Current.Resources["SidebarLabelSelectedStyle"];

            //Display initial page
            ShowTracksPage("All", _localLibraryManager.Tracks);
            lblSidebarAll.Style = _sidebarSelectedStyle;

            //Setup animations
            _trackProgressStoryboard = (Storyboard)Resources["progTrackProgress_AnimStoryboard"];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _musicPlayer.Dispose();
            _audioDevice.Dispose();
            _audioDevice = null;
        }

        /////////////////////////////////
        //Player Control Event Handlers//
        /////////////////////////////////
        private void playPauseButton_Clicked()
        {
            if (_musicPlayer.PlayState == PlayState.Playing)
            {
                _musicPlayer.Pause();
                _trackProgressStoryboard.Pause();
                playPauseButton.DisplayState = PlayButtonDisplayState.Play;
            }
            else if (_musicPlayer.PlayState == PlayState.Paused)
            {
                _musicPlayer.Resume();
                _trackProgressStoryboard.Resume();
                playPauseButton.DisplayState = PlayButtonDisplayState.Pause;
            }
        }

        //private void LibraryPage_ArtistSelected(string Artist)
        //{
        //    string artistName = Artist;
        //    ShowTracksPage(artistName, _localLibraryManager.FindByArtist(artistName));
        //}

        //////////////////////////
        //Sidebar Event Handlers//
        //////////////////////////
        private void lblSidebarAll_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowTracksPage("All", _localLibraryManager.Tracks);
            lblSidebarAll.Style = _sidebarSelectedStyle;
        }

        private void lblSidebarQueue_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)

        {
            var queue = _musicPlayer.Queue;
            ShowTracksPage("Queue", queue);
            lblSidebarQueue.Style = _sidebarSelectedStyle;
        }

        private void lblSidebarPlaylists_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowPlaylistPage();
            lblSidebarPlaylists.Style = _sidebarSelectedStyle;
        }

        private void lblSidebarArtists_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowArtistPage();
            lblSidebarArtists.Style = _sidebarSelectedStyle;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
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
                _searchPage.SetDataContext(searchResults);
                _searchPage.SetPageHeader($"Search results for '{query}'");
                if (LibraryFrame.Content != _searchPage)
                    LibraryFrame.Content = _searchPage;
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //listboxNavigation.SelectedItem = null;
            UpdateSearchBoxPlaceholder();
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSearchBoxPlaceholder();
        }

        /////////////////////////////
        //Track Page Event Handlers//
        /////////////////////////////
        private void TrackPage_TracksSelected(IEnumerable<AudioFile> Selected, IEnumerable<AudioFile> AllTracks)
        {
            IEnumerable<AudioFile> tracksToPlay = null;
            if (_musicPlayer.Shuffle == true)
            {
                if (Selected.Count() > 1)
                    tracksToPlay = _shuffler.Shuffle(Selected);
                else
                    tracksToPlay = _shuffler.Shuffle(AllTracks, Selected.First());
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

            //Setup progress bar animation
            DoubleAnimation anim = (DoubleAnimation)_trackProgressStoryboard.Children[0];
            anim.Duration = _musicPlayer.GetCurrentTrackLength();
            _trackProgressStoryboard.Begin();
        }

        private void TrackPage_ArtistSelected(string Artist)
        {
            var artistTracks = _localLibraryManager.FindByArtist(Artist);
            ShowTracksPage(Artist, artistTracks);
        }

        private void TrackPage_AlbumSelected(string Artist, string Album)
        {
            var albumTracks = _localLibraryManager.FindByAlbum(Artist, Album);
            ShowTracksPage(Album, albumTracks);
        }

        private void TrackPage_AddPlaylistSelected(IEnumerable<AudioFile> Tracks)
        {
            SelectPlaylistDialog spd = new SelectPlaylistDialog(_playlistManager.Playlists);
            spd.Owner = this;
            spd.ShowDialog();
            Playlist selected = spd.SelectedPlaylist;
            if (selected == null)
                return;

            selected.AddTracks(Tracks);
            _playlistManager.SavePlaylist(selected);
        }

        private void Trackpage_PlaylistUpdated(Playlist UpdatedPlaylist)
        {
            _playlistManager.SavePlaylist(UpdatedPlaylist);
            ShowTracksPage(UpdatedPlaylist.Name, UpdatedPlaylist.Tracks, true, UpdatedPlaylist);
        }

        ////////////////////////////////
        //Playlist Page Event Handlers//
        ////////////////////////////////
        private void PlaylistPage_PlaylistSelected(Playlist Playlist)
        {
            ShowTracksPage(Playlist.Name, Playlist.Tracks, true,Playlist);
        }


        //////////////////
        //User Functions//
        //////////////////
        public void ShowTracksPage(string PageHeader, IEnumerable<AudioFile> Tracks, bool ClearSearch = true, Playlist PlaylistContext = null)
        {
            ClearSidebarSelection();

            TrackPage newPage = new TrackPage(PageHeader, Tracks);
            newPage.PlaylistContext = PlaylistContext;
            newPage.TracksSelected += TrackPage_TracksSelected;
            newPage.ArtistSelected += TrackPage_ArtistSelected;
            newPage.AlbumSelected += TrackPage_AlbumSelected;
            newPage.AddToPlaylistSelected += TrackPage_AddPlaylistSelected;
            newPage.PlaylistUpdated += Trackpage_PlaylistUpdated;

            LibraryFrame.Content = newPage;
            if (ClearSearch == true)
                txtSearch.Clear();
        }

        private void ShowPlaylistPage()
        {
            ClearSidebarSelection();
            PlaylistPage newPage = new PlaylistPage(_playlistManager);
            newPage.PlaylistSelected += PlaylistPage_PlaylistSelected;
            LibraryFrame.Content = newPage;
            txtSearch.Clear();
        }

        private void ShowArtistPage()
        {
            ClearSidebarSelection();

            throw new Exception();

            //string[] artists = _localLibraryManager.Tracks.Select(track => track.Artist).Distinct().ToArray();

            //LibraryPage newPage = new LibraryPage(_playlistManager.Playlists);
            //newPage.SetPageHeader("Artists");
            //newPage.SetDataGridItems(artists);
            //newPage.ArtistSelected += LibraryPage_ArtistSelected;
            //LibraryFrame.Content = newPage;
            //txtSearch.Clear();
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

       private void ClearSidebarSelection()
        {
            var defaultStyle = (Style)Application.Current.Resources["SidebarLabelStyle"];

            lblSidebarAll.Style = defaultStyle;
            lblSidebarArtists.Style = defaultStyle;
            lblSidebarPlaylists.Style = defaultStyle;
            lblSidebarQueue.Style = defaultStyle;
        }
    }
}