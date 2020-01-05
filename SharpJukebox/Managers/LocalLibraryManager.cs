using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJukebox
{
    public class LocalLibraryManager
    {
        private ITrackLocater _fileLocater;
        private MetadataExtractor _metadataExtractor;

        public ReadOnlyCollection<AudioFile> Tracks { get; private set; }

        public LocalLibraryManager(ITrackLocater FileLocater, MetadataExtractor MetadataExtractor)
        {
            _fileLocater = FileLocater;
            _metadataExtractor = MetadataExtractor;
        }

        public void UpdateFiles()
        {
            _fileLocater.LocateFiles();
            _metadataExtractor.Extract(_fileLocater.GetFiles());
            Tracks = _metadataExtractor.Files;
        }

        public ReadOnlyCollection<AudioFile> Search(string Query)
        {
            var result = Tracks.Where(file =>
            file.Title.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
            file.Artist.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
            file.Album.Contains(Query, StringComparison.OrdinalIgnoreCase)
            );
            return new ReadOnlyCollection<AudioFile>(result.ToList());
        }

        public AudioFile FindByFilename(string Filename)
        {
            foreach(AudioFile track in Tracks)
                if (track.Filename == Filename)
                    return track;

            return null;
        }

        public ReadOnlyCollection<AudioFile> FindByArtist(string ArtistName)
        {
            var result = Tracks.Where(file => file.Artist == ArtistName);
            return new ReadOnlyCollection<AudioFile>(result.ToList());
        }
    }
}
