using System;
using System.Collections.Generic;
using System.Text;

namespace SharpJukebox
{
    public class MusicShuffler
    {
        private const int SHUFFLE_ITERATIONS = 4;

        private Random _random = new Random();

        public AudioFile[] Shuffle(AudioFile[] Files)
        {
            AudioFile[] result = new AudioFile[Files.Length];
            Files.CopyTo(result, 0);

            for(int iteration = 0; iteration < SHUFFLE_ITERATIONS;iteration++)
            {
                for(int index = 0; index < result.Length;index++)
                {
                    int randomIndex = _random.Next(0, result.Length);
                    Swap(result, index, randomIndex);
                }
            }

            return result;
        }

        public AudioFile[] Shuffle(AudioFile[] Files, AudioFile FirstFile)
        {
            var shuffled = Shuffle(Files);
            int targetIndex = GetIndexOf(shuffled, FirstFile);
            Swap(shuffled, 0, targetIndex);
            return shuffled;
        }

        private int GetIndexOf(AudioFile[] Files, AudioFile Target)
        {
            for (int i = 0; i < Files.Length; i++)
                if (Files[i] == Target)
                    return i;

            return -1;
        }

        private void Swap(AudioFile[] Files, int Index1, int Index2)
        {
            AudioFile tmp = Files[Index1];
            Files[Index1] = Files[Index2];
            Files[Index2] = tmp;
        }

    }
}
