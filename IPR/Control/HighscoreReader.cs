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
    /// JSon
    /// </summary>
    class HighscoreReader
    {
        private static bool isInitialised = false;
        private static IHighscoreReaderAdapter reader;

        public HighscoreReader()
        {
            reader = new XMLReader();
        }

        public static async Task initAsync()
        {
            await reader.initAsync();
            isInitialised = true;
        }

        public async static Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            return await reader.GetHighscoresAsync();
        }

        public async static Task SaveHighscoreObj(HighscoreObj obj)
        {
            await reader.SaveHighscoreObj(obj);
        }

        public async static Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            await reader.SaveHighscoreObjs(objs);
        }
    }

    public interface IHighscoreReaderAdapter
    {
        Task initAsync();

        Task<List<HighscoreObj>> GetHighscoresAsync();

        Task SaveHighscoreObj(HighscoreObj obj);

        Task SaveHighscoreObjs(List<HighscoreObj> objs);
    }

    public class JSonReader : IHighscoreReaderAdapter
    {
        private static JsonObject highscoreCompilation;
        private static bool isInitialised = false;

        public async Task initAsync()
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

        public async Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            if (!isInitialised)
                await initAsync();
            HighscoreObj retVal = null;
            JsonObject highscore = highscoreCompilation["hoi"].GetObject();
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
            return null;
        }


        public Task SaveHighscoreObj(HighscoreObj obj)
        {
            throw new NotImplementedException();
        }

        public Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            throw new NotImplementedException();
        }
    }
    class XMLReader : IHighscoreReaderAdapter
    {

        public Task initAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveHighscoreObj(HighscoreObj obj)
        {
            throw new NotImplementedException();
        }

        public Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            throw new NotImplementedException();
        }
    }
}
