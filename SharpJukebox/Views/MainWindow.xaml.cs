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
        private MusicPlayer _musicPlayer;
        private QueueBuilder _queueBuilder;
        private TrackPage _searchPage;
        private DispatcherTimer _trackProgressTimer;

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
            _musicPlayer = new MusicPlayer();
            _queueBuilder = new QueueBuilder();
            _playlistReader = new LocalPlaylistReader(_localLibraryManager, System.IO.Path.Join(AppContext.BaseDirectory, "Playlists"));
            _playlistWriter = new LocalPlaylistWriter(System.IO.Path.Join(AppContext.BaseDirectory, "Playlists"));
            _playlistManager = new PlaylistManager(_playlistReader, _playlistWriter);
            _trackProgressTimer = new DispatcherTimer();
            _trackProgressTimer.Interval = TimeSpan.FromSeconds(0.5);
            _trackProgressTimer.Tick += (_, __) =>
            {
                lblCurrentTime.Content = _musicPlayer.GetCurrentTrackTimePosition().ToString(@"mm\:ss");
            };

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
            nextButton.Pressed += nextButton_Clicked;
            prevButton.Pressed += prevButton_Clicked;
            seekBar.Seeked += seekBar_Seeked;
            _musicPlayer.Started += musicPlayer_Started;
            _musicPlayer.Paused += musicPlayer_Paused;
            _musicPlayer.Seeked += musicPlayer_Seeked;
            _musicPlayer.Resumed += musicPlayer_Resumed;
            _musicPlayer.Stopped += musicPlayer_Stopped;

            //Load the required style
            _sidebarSelectedStyle = (Style)Application.Current.Resources["SidebarLabelSelectedStyle"];

            //Display initial page
            ShowTracksPage("All", _localLibraryManager.Tracks);
            lblSidebarAll.Style = _sidebarSelectedStyle;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _musicPlayer.Dispose();
            _audioDevice.Dispose();
            _audioDevice = null;
        }

        /////////////////////////
        //Player Event Handlers//
        /////////////////////////
        private void musicPlayer_Started(AudioFile NewTrack)
        {
            lblTrackName.Content = $"{NewTrack.Title} - {NewTrack.Artist}";
            playPauseButton.DisplayState = PlayButtonDisplayState.Pause;
            seekBar.StartAnimation(_musicPlayer.GetCurrentTrackLength());
            _trackProgressTimer.Start();
            lblLength.Content = _musicPlayer.GetCurrentTrackLength().ToString(@"mm\:ss");
        }

        private void musicPlayer_Resumed()
        {
            playPauseButton.DisplayState = PlayButtonDisplayState.Pause;
            seekBar.ResumeAnimation();
        }

        private void musicPlayer_Paused()
        {
            playPauseButton.DisplayState = PlayButtonDisplayState.Play;
            seekBar.PauseAnimation();
        }

        private void musicPlayer_Seeked(double NewPercentage)
        {
            seekBar.StartAnimation(NewPercentage, _musicPlayer.GetCurrentTrackLength());
            if (_musicPlayer.PlayState == PlayState.Paused)
                seekBar.PauseAnimation();
        }

        private void musicPlayer_Stopped()
        {
            _trackProgressTimer.Stop();
            lblTrackName.Content = null;
            playPauseButton.DisplayState = PlayButtonDisplayState.Play;
        }

        /////////////////////////////////
        //Player Control Event Handlers//
        /////////////////////////////////
        private void playPauseButton_Clicked()
        {
            if (_musicPlayer.PlayState == PlayState.Playing)
                _musicPlayer.Pause();
            else if (_musicPlayer.PlayState == PlayState.Paused)
                _musicPlayer.Resume();
        }

        private void nextButton_Clicked()
        {
            _musicPlayer.NextTrack();
        }

        private void prevButton_Clicked()
        {
            var x = _musicPlayer.GetCurrentTrackTimePosition();

            if (_musicPlayer.GetCurrentTrackTimePosition() < TimeSpan.FromSeconds(1))
                _musicPlayer.PreviousTrack();
            else
                _musicPlayer.Play();
        }

        private void seekBar_Seeked(double TargetPercent)
        {
            _musicPlayer.Seek(TargetPercent);
        }

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
            if (Selected.Count() == 1)
                PlayTracks(AllTracks, Selected.First());
            else
                PlayTracks(Selected, null);

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
            ShowTracksPage(Playlist.Name, Playlist.Tracks, true, Playlist);
        }
        private void NewPage_PlayPlaylistSelected(Playlist Playlist)
        {
            PlayTracks(Playlist.Tracks, null);
        }

        //////////////////////////////
        //Artist Page Event Handlers//
        //////////////////////////////
        public void ArtistPage_ArtistSelected(string ArtistName)
        {
            IEnumerable<AudioFile> tracks = _localLibraryManager.FindByArtist(ArtistName);
            ShowTracksPage(ArtistName, tracks);
        }

        public void ArtistPage_PlayArtistSelected(string ArtistName)
        {
            IEnumerable<AudioFile> tracks = _localLibraryManager.FindByArtist(ArtistName);
            PlayTracks(tracks, null);
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
            newPage.PlayPlaylistSelected += NewPage_PlayPlaylistSelected;
            LibraryFrame.Content = newPage;
            txtSearch.Clear();
        }

        private void ShowArtistPage()
        {
            ClearSidebarSelection();
            IEnumerable<string> artists = _localLibraryManager.Tracks.Select(track => track.Artist).Distinct();
            ArtistPage newPage = new ArtistPage("Artists", artists);
            newPage.ArtistSelected += ArtistPage_ArtistSelected;
            LibraryFrame.Content = newPage;
            txtSearch.Clear();
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

        private void PlayTracks(IEnumerable<AudioFile> Tracks, AudioFile First)
        {
            IEnumerable<AudioFile> queue = null;
            queue = _queueBuilder.BuildQueue(Tracks, First, _musicPlayer.Shuffle);

            _musicPlayer.ClearQueue();
            _musicPlayer.AddToQueue(queue);
            _musicPlayer.Play();
        }


    }
}