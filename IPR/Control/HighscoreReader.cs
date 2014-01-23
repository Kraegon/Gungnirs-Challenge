using IPR.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using System.Xml;
using Windows.Data.Xml.Dom;
using System.Xml.Linq;

namespace IPR.Control
{
    /// <summary>
    /// Uses bridge pattern, reader options are XML or JSon(Not implemented)
    /// </summary>
    class HighscoreReader
    {
        public static StorageFile HighscoreFile;
        private static IHighscoreReaderBridge reader = new XMLReader();

        public static bool IsInitialised = false;
        public static bool IsFileEmpty = true;

        public delegate void HighscoreUpdatedHandler();
        public static event HighscoreUpdatedHandler HighscoreUpdatedEvent;

        public static async Task initAsync()
        {
            await reader.InitAsync();
            HighscoreReader.IsInitialised = true;
        }

        public async static Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            var retVal = await reader.GetHighscoresAsync();
            return retVal;
        }

        public async static void SaveHighscoreObj(HighscoreObj obj)
        {
            await reader.SaveHighscoreObj(obj);
            HighscoreUpdatedEvent();
        }

        public async static void ClearHighscores()
        {
            await reader.ClearHighscores();
            HighscoreUpdatedEvent();
        }

        public async static void SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            await reader.SaveHighscoreObjs(objs);
            HighscoreUpdatedEvent();
        }

        public async static Task<List<HighscoreObj>> SortHighestScoreFirst(List<HighscoreObj> objs)
        {
            if (!IsInitialised)
                await initAsync();
            if (IsFileEmpty)
                return new List<HighscoreObj>();
            List<HighscoreObj> unsortedHighscores = objs;
            var sortedVar = from highscore in unsortedHighscores
                            orderby highscore.Distance descending
                            select highscore;
            List<HighscoreObj> sortedList = new List<HighscoreObj>(sortedVar.AsEnumerable<HighscoreObj>());
            return sortedList;
        }
    }

    public interface IHighscoreReaderBridge
    {
        Task InitAsync();

        Task<List<HighscoreObj>> GetHighscoresAsync();

        Task SaveHighscoreObj(HighscoreObj obj);

        Task SaveHighscoreObjs(List<HighscoreObj> objs);

        Task ClearHighscores();
    }
    /// <summary>
    /// Json version
    /// </summary>
    public class JsonReader : IHighscoreReaderBridge
    {
        private static JsonObject highscoreCompilation;

        public async Task InitAsync()
        {
            string rawText = String.Empty;
            //Three steps neccesary to subvert access shenanigans
            /* Use for access to ASSETS folder
            var installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var subfolder = await installFolder.GetFolderAsync("Assets\\");
            var file = await subfolder.GetFileAsync("Highscores.json");
            /**/
            try
            {
                HighscoreReader.HighscoreFile = await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFileAsync("Highscore.Json");
                rawText = await FileIO.ReadTextAsync(HighscoreReader.HighscoreFile);
                highscoreCompilation = JsonObject.Parse(rawText)["stringRef"].GetObject();
                HighscoreReader.IsInitialised = true;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }
        private async Task CreateJsonFileAsync()
        {
            try
            {
                await ApplicationData.Current.RoamingFolder.CreateFileAsync("Highscores.Json");
                await InitAsync();
            }
            catch
            {
                //If highscores init unsuccesful, we're doomed. Making a decent catch means filling in an error on the GUI.
                //Let's make a healthy assumption this'll just work.
            }
        }
        public async Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            if (!HighscoreReader.IsInitialised)
                await InitAsync();
            HighscoreObj retVal = null;
            JsonObject highscore = highscoreCompilation["hoi"].GetObject();
            try
            {
                string name = highscore["Name"].GetString();
                float distance = (float)highscore["Distance"].GetNumber();
                string[] time = highscore["TimeTaken"].GetString().Split(':');
                int hours = int.Parse(time[0]);
                int minutes = int.Parse(time[1]);
                TimeSpan timeTaken = new TimeSpan(hours, minutes, 0);
            }
            catch (KeyNotFoundException)
            {
                retVal = new HighscoreObj();
            }
            catch
            {
                return GetHighscoresAsync().Result; 
            }
            return null;
        }


        public async Task SaveHighscoreObj(HighscoreObj obj)
        {
            if (!HighscoreReader.IsInitialised)
                await InitAsync();
            throw new NotImplementedException();
        }

        public async Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            if (!HighscoreReader.IsInitialised)
                await InitAsync();
            throw new NotImplementedException();
        }


        public Task ClearHighscores()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// XML version
    /// </summary>
    class XMLReader : IHighscoreReaderBridge
    {
        public async Task InitAsync()
        {
            try
            {
                HighscoreReader.HighscoreFile = await ApplicationData.Current.RoamingFolder.GetFileAsync("Highscores.xml");
                HighscoreReader.IsInitialised = true;
                HighscoreReader.IsFileEmpty = await checkFileEmpty();
            }
            catch (FileNotFoundException)
            {
                CreateXMLfile();
            }
        }

        private async void CreateXMLfile()
        {
            try
            {
                await ApplicationData.Current.RoamingFolder.CreateFileAsync("Highscores.XML");
                await InitAsync();
            }
            catch
            {
                //If highscores init unsuccesful, we're doomed. Making a decent catch means filling in an error report on the GUI.
                //Let's make a healthy assumption this'll just work.
            }
        }

        private async Task<bool> checkFileEmpty()
        {
            System.Xml.XmlReader XmlReader = System.Xml.XmlReader.Create(await HighscoreReader.HighscoreFile.OpenStreamForReadAsync());
            try
            {
                XmlReader.Read();
            }
            catch (XmlException e)
            {
                if (e.Message.StartsWith("Root element"))
                {
                    XmlReader.Dispose();
                    return true;
                }
                else
                    System.Diagnostics.Debugger.Break();
            }
            XmlReader.Dispose();
            return false;
        }

        public async Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            if (!HighscoreReader.IsInitialised)
                await InitAsync();
            if (HighscoreReader.IsFileEmpty)
                return new List<HighscoreObj>();
            List<HighscoreObj> retVal = new List<HighscoreObj>();           
            try
            {
                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(HighscoreReader.HighscoreFile);
                var node = xmlDoc.SelectSingleNode("Highscores");
                var nodeList = node.ChildNodes;
                foreach (var e in nodeList)
                {
                    HighscoreObj highscore = new HighscoreObj();
                    highscore.Name = (string)e.Attributes[0].NodeValue;
                    highscore.Distance = float.Parse((string)e.Attributes[1].NodeValue);
                    highscore.TimeTaken = TimeSpan.Parse((string)e.Attributes[2].NodeValue);
                    retVal.Add(highscore);
                }
            }
            catch (UnauthorizedAccessException)
            {
                retVal = null;
            }
            return retVal;
        }

        public async Task SaveHighscoreObj(HighscoreObj obj)
        {
            if (!HighscoreReader.IsInitialised)
                await InitAsync();
            if (HighscoreReader.IsFileEmpty)
            {
                //Create
                using (XmlWriter xmlWriter = XmlWriter.Create(await HighscoreReader.HighscoreFile.OpenStreamForWriteAsync()))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Highscores");
                    xmlWriter.WriteStartElement("Highscore");
                    xmlWriter.WriteAttributeString("Name", obj.Name);
                    xmlWriter.WriteAttributeString("Distance", obj.Distance.ToString());
                    xmlWriter.WriteAttributeString("Time", obj.TimeTaken.ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                }
                HighscoreReader.IsFileEmpty = false;
            }
            else
            {
                //Append
                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(HighscoreReader.HighscoreFile);
                var node = xmlDoc.SelectSingleNode("Highscores");
                var newElement = xmlDoc.CreateElement("Highscore");
                newElement.SetAttribute("Name", obj.Name);
                newElement.SetAttribute("Distance", obj.Distance.ToString());
                newElement.SetAttribute("Time", obj.TimeTaken.ToString());
                node.AppendChild(newElement);
                await xmlDoc.SaveToFileAsync(HighscoreReader.HighscoreFile); 
            }
        }

        public async Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            foreach (var obj in objs)
            {
                await SaveHighscoreObj(obj);
            }
        }


        public async Task ClearHighscores()
        {
            await HighscoreReader.HighscoreFile.DeleteAsync();
        }
    }
}