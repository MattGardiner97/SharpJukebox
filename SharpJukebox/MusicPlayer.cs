using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace SharpJukebox
{
    public class MusicPlayer
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;
        private MMDevice _currentDevice;

        private int _currentTrackIndex = -1;
        private AudioFile _currentTrack;

        private List<AudioFile> _queue;

        public PlayState PlayState { get; private set; } = PlayState.Stopped;
        public bool Shuffle { get; set; } = true;
        public IEnumerable<AudioFile> Queue { get { return _queue; } }

        public event Action TrackFinished;

        public MusicPlayer()
        {
            _queue = new List<AudioFile>();
        }

        public void Load()
        {
            Cleanup();

            _currentTrack = _queue[_currentTrackIndex];

            _waveSource = CodecFactory.Instance.GetCodec(_currentTrack.Filename)
                .ToSampleSource()
                .ToMono()
                .ToWaveSource();
            _soundOut = new WasapiOut() { Latency = 100, Device = _currentDevice };
            _soundOut.Initialize(_waveSource);

            _soundOut.Stopped += _soundOut_Stopped;
        }

        private void _soundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            if(TrackFinished != null)
             TrackFinished();
        }

        public void Play()
        {
            if (_queue.Count == 0)
                return;

            if (_currentTrackIndex == -1)
                _currentTrackIndex++;

            Load();
            _soundOut.Play();
            this.PlayState = PlayState.Playing;
        }

        public void Resume()
        {
            if (PlayState != PlayState.Paused)
                return;

            _soundOut.Play();
            this.PlayState = PlayState.Playing;

        }

        public void Pause()
        {
            _soundOut.Pause();

            this.PlayState = PlayState.Paused;
        }

        public void Seek(int Time)
        {

        }

        public void NextTrack()
        {
            if (_queue.Count == 0 || _currentTrackIndex + 1 >= _queue.Count)
                return;

            _currentTrackIndex++;
        }

        public void PreviousTrack()
        {
            if (_queue.Count == 0 || _currentTrackIndex - 1 <= _queue.Count)
                return;

            _currentTrackIndex--;
        }

        public void Cleanup()
        {
            this.PlayState = PlayState.Stopped;

            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        public void Dispose()
        {
            Cleanup();
        }

        public void AddToQueue(AudioFile File)
        {
            _queue.Add(File);
        }

        public void AddToQueue(IEnumerable<AudioFile> Files)
        {
            _queue.AddRange(Files);
        }

        public void ClearQueue()
        {
            Cleanup();
            _currentTrack = null;
            _queue.Clear();
        }

        public void SetAudioDevice(MMDevice Device)
        {
            _currentDevice = Device;
        }

        public TimeSpan GetCurrentTrackTimePosition()
        {
            if (_currentTrack == null || _waveSource == null)
                return TimeSpan.Zero;

            return _currentTrack.Duration * GetCurrentTrackCompletion();
        }

        public TimeSpan GetCurrentTrackLength()
        {
            if (_currentTrack == null || _waveSource == null)
                return TimeSpan.Zero;

            return _currentTrack.Duration;
        }

        public double GetCurrentTrackCompletion()
        {
            if (_currentTrack == null || _waveSource == null)
                return 0;

            var current = _waveSource.Position;
            var total = _waveSource.Length;
            var result = (double)current / total;

            return result;
        }
    }

    public enum PlayState
    {
        Playing,
        Paused,
        Stopped
    }
}
