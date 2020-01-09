using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpJukebox
{
    public class QueueBuilder
    {
        private MusicShuffler _shuffler;

        public QueueBuilder()
        {
            _shuffler = new MusicShuffler();
        }

        public IEnumerable<AudioFile> BuildQueue(IEnumerable<AudioFile> Tracks, AudioFile FirstTrack, bool Shuffle )
        {
            if (Tracks.Count() == 1)
                return Tracks;

            IEnumerable<AudioFile> result = null;
            if (Shuffle == true)
            {
                result = _shuffler.Shuffle(Tracks,FirstTrack);
            }
            else
            {
                if (FirstTrack == null || Tracks.First() == FirstTrack)
                    return Tracks;

                List<AudioFile> resultList = new List<AudioFile>(Tracks);

                int startIndex = GetIndexOf(resultList, FirstTrack);
                var elementsBeforeTarget = resultList.GetRange(0, startIndex);
                resultList.RemoveRange(0, startIndex);
                resultList.AddRange(elementsBeforeTarget);
                result = resultList;
            }

            return result;
        }

        private int GetIndexOf(List<AudioFile> Tracks, AudioFile Target)
        {
            for(int i = 0; i < Tracks.Count; i++)
            {
                if (Tracks[i] == Target)
                    return i;
            }

            return -1;
        }

    }
}
