using IPR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Json;
using Windows.Storage;

namespace IPR.Control
{
    /// <summary>
    /// TODO: Add more queries.
    /// </summary>
    class HighscoreReader
    {
        private static JsonObject highscoreCompilation;
        private static bool isInitialised = false;

        public HighscoreReader()
        { 
            //Can't init asynchronously, init is checked in get method.
        }

        private async Task initAsync()
        {
            string rawText = String.Empty;
            //Three steps neccesary to subvert access shenanigans
            var installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var subfolder = await installFolder.GetFolderAsync("Assets\\");
            var file = await subfolder.GetFileAsync("Highscores.json");

            rawText = await FileIO.ReadTextAsync(file);                
            highscoreCompilation = JsonObject.Parse(rawText)["stringRef"].GetObject();
            isInitialised = true;
        }
        /// <summary>
        /// TODO: Test it
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static HighscoreObj findByRank(int rank)
        {
            HighscoreObj retVal = null;
            JsonObject highscore = highscoreCompilation[rank.ToString()].GetObject();
            try
            {
                string name = highscore["Name"].GetString();
                float distance = (float) highscore["Distance"].GetNumber();
                string[] time = highscore["TimeTaken"].GetString().Split(':');
                int hours = int.Parse(time[0]);
                int minutes = int.Parse(time[1]);
                TimeSpan timeTaken = new TimeSpan(hours, minutes, 0);
            }
            catch (KeyNotFoundException)
            {
                retVal = new HighscoreObj();
            }
            return retVal;
        }
    }
}
