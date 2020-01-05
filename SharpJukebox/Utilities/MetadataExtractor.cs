using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TagLib;

namespace SharpJukebox
{
    public class MetadataExtractor
    {
        private List<AudioFile> _resultList;

        public ReadOnlyCollection<AudioFile> Files { get; private set; }

        public MetadataExtractor()
        {
            _resultList = new List<AudioFile>();
            Files = new ReadOnlyCollection<AudioFile>(_resultList);
        }

        public void Extract(IEnumerable<string> Filenames)
        {
            foreach(string filename in Filenames)
            {
                File file  = File.Create(filename);

                AudioFile aFile = CreateAudioFile(file);
                aFile.Filename = filename;

                _resultList.Add(aFile);
            }
        }

        private AudioFile CreateAudioFile(File TagFile)
        {
            string filename = new System.IO.FileInfo(TagFile.Name).Name;

            AudioFile result = new AudioFile();
            result.Title = TagFile.Tag.Title == null ? filename : TagFile.Tag.Title;
            result.Artist = TagFile.Tag.FirstArtist == null ? "Unknown Artist" : TagFile.Tag.FirstArtist;
            result.Album = TagFile.Tag.Album == null ? "Unknown Album" : TagFile.Tag.Album;
            result.Duration = TagFile.Properties.Duration;
            return result;
        }

    }
}
