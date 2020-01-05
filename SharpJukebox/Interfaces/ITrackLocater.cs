using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJukebox
{
    public interface ITrackLocater
    {
        public void LocateFiles();
        public ReadOnlyCollection<string> GetFiles();
    }
}
