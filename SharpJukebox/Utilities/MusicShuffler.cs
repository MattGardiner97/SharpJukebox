using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpJukebox
{
    public class MusicShuffler
    {
        private const int SHUFFLE_ITERATIONS = 4;

        private Random _random = new Random();

        public IEnumerable<AudioFile> Shuffle(IEnumerable<AudioFile> Tracks, AudioFile First)
        {
            List<AudioFile> resultList = new List<AudioFile>(Tracks);

            int trackCount = resultList.Count;

            for (int shufflerIteration = 0; shufflerIteration < SHUFFLE_ITERATIONS; shufflerIteration++)
            {
                for(int i = 0; i < trackCount;i++)
                {
                    int firstIndex = _random.Next(0, trackCount);
                    int secondIndex = _random.Next(0, trackCount);
                    Swap(resultList, firstIndex, secondIndex);
                }
            }

            if(First != null)
            {
                int index = GetIndexOfTrack(resultList, First);
                Swap(resultList, 0, index);
            }

            return resultList;
        }

        private int GetIndexOfTrack(List<AudioFile> Files, AudioFile Target)
        {
            for (int i = 0; i < Files.Count; i++)
                if (Files[i] == Target)
                    return i;

            return -1;
        }

        private void Swap(List<AudioFile> Files, int Index1, int Index2)
        {
            AudioFile tmp = Files[Index1];
            Files[Index1] = Files[Index2];
            Files[Index2] = tmp;
        }

    }
}
