using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace SharpJukebox
{
    public class LocalTrackLocater : ITrackLocater
    {
        private static readonly string[] SUPPORTED_FILETYPES = new string[] { "mp3","wav" };

        private List<string> _locatedFilenames;
        private ReadOnlyCollection<string> _result;

        public string[] SearchDirectories { get; set; }

        public LocalTrackLocater(string[] SearchDirectories)
        {
            this.SearchDirectories = SearchDirectories;
            _locatedFilenames = new List<string>();
            _result = new ReadOnlyCollection<string>(_locatedFilenames);
        }

        public ReadOnlyCollection<string> GetFiles()
        {
            return _result;
        }

        public void LocateFiles()
        {
            foreach(string directory in SearchDirectories)
            {
                string[] filenames = Directory.GetFiles(directory,"",SearchOption.AllDirectories);
                foreach(string fName in filenames)
                {
                    if (IsSupportedFile(fName))
                        _locatedFilenames.Add(fName);
                }
            }
        }

        private bool IsSupportedFile(string Filename)
        {
            string extension = Filename.Substring(Filename.LastIndexOf(".") + 1);
            return SUPPORTED_FILETYPES.Any(ex => ex == extension.ToLower());
        }
    }
}
