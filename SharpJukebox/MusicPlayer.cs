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
        public AudioFile CurrentTrack { get; private set; }

        private List<AudioFile> _queue;

        public PlayState PlayState { get; private set; } = PlayState.Stopped;
        public bool Shuffle { get; set; } = true;
        public IEnumerable<AudioFile> Queue { get { return _queue; } }

        public event Action<AudioFile> Started;
        public event Action Paused;
        public event Action Resumed;
        public event Action Stopped;
        public event Action<double> Seeked;

        public MusicPlayer()
        {
            _queue = new List<AudioFile>();
        }

        public void Load()
        {
            

            Cleanup();

            CurrentTrack = _queue[_currentTrackIndex];
            CurrentTrack.CurrentlyPlaying = true;

            _waveSource = CodecFactory.Instance.GetCodec(CurrentTrack.Filename)
                .ToSampleSource()
                .ToMono()
                .ToWaveSource();
            _soundOut = new WasapiOut() { Latency = 100, Device = _currentDevice };
            _soundOut.Initialize(_waveSource);

            _soundOut.Stopped += _soundOut_Stopped;
        }

        private void _soundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (_currentTrackIndex + 1 >= _queue.Count)
            {
                PlayState = PlayState.Stopped;
                Stopped?.Invoke();
                return;
            }

            NextTrack();
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
            Started?.Invoke(CurrentTrack);
        }

        public void Resume()
        {
            if (PlayState != PlayState.Paused)
                return;

            _soundOut.Play();
            this.PlayState = PlayState.Playing;
            Resumed?.Invoke();
        }

        public void Pause()
        {
            _soundOut.Pause();

            this.PlayState = PlayState.Paused;
            Paused?.Invoke();
        }

        public void Seek(double TrackPercent)
        {
            if (CurrentTrack == null)
                return;

            if (TrackPercent < 0 || TrackPercent > 1)
                return;

            TimeSpan targetTime = _waveSource.GetLength() * TrackPercent;
            _waveSource.SetPosition(targetTime);
            Seeked?.Invoke(TrackPercent);
        }

        public void NextTrack()
        {
            if (_queue.Count == 0 || _currentTrackIndex + 1 >= _queue.Count)
                return;

            _currentTrackIndex++;
            Play();
        }

        public void PreviousTrack()
        {
            if (_queue.Count == 0 || _currentTrackIndex - 1 < 0)
                return;

            _currentTrackIndex--;
            Play();

        }

        public void Cleanup()
        {
            this.PlayState = PlayState.Stopped;

            if (CurrentTrack != null)
                CurrentTrack.CurrentlyPlaying = false;


            if (_soundOut != null)
            {
                _soundOut.Stopped -= _soundOut_Stopped;
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
            CurrentTrack = null;
            _currentTrackIndex = 0;
            _queue.Clear();
        }

        public void SetAudioDevice(MMDevice Device)
        {
            _currentDevice = Device;
        }

        public TimeSpan GetCurrentTrackTimePosition()
        {
            if (CurrentTrack == null || _waveSource == null)
                return TimeSpan.Zero;

            return CurrentTrack.Duration * GetCurrentTrackCompletion();
        }

        public TimeSpan GetCurrentTrackLength()
        {
            if (CurrentTrack == null || _waveSource == null)
                return TimeSpan.Zero;

            return CurrentTrack.Duration;
        }

        public double GetCurrentTrackCompletion()
        {
            if (CurrentTrack == null || _waveSource == null)
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
