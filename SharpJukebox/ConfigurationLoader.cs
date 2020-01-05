using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace SharpJukebox
{
    public class ConfigurationLoader
    {
        public Configuration Configuration { get; private set; }

        public void LoadFromFile(string Filename)
        {
            if (File.Exists(Filename) == false)
            {
                Configuration = null;
                return;
            }

            string fileContents = File.ReadAllText(Filename);
            this.Configuration = JsonConvert.DeserializeObject<Configuration>(fileContents);
        }
    }
}
