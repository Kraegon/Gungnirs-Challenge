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
    /// Reads & writes highscores to and from a file.
    /// Uses a bridge pattern, reader options are XML or JSon(Not implemented)
    /// </summary>
    class HighscoreIO
    {
        /// <summary>
        /// The highscores.xxx file. Set by file interfacer.
        /// </summary>
        public static StorageFile HighscoreFile;
        private static IHighscoreIOBridge ioHandler = new XMLIO();

        public static bool IsInitialised = false;
        public static bool IsFileEmpty = true;

        public delegate void HighscoreUpdatedHandler();
        public static event HighscoreUpdatedHandler HighscoreUpdatedEvent;
        
        /// <summary>
        /// Relay initialise command
        /// </summary>
        public static async Task InitAsync()
        {
            await ioHandler.InitAsync();
            HighscoreIO.IsInitialised = true;
        }

        /// <returns>Gets all highscores in the file unsorted.</returns>
        public async static Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            var retVal = await ioHandler.GetHighscoresAsync();
            return retVal;
        }

        /// <summary>
        /// Save a single highscore object.
        /// </summary>
        /// <param name="obj">Contains data on a throw to store</param>
        public async static void SaveHighscoreObj(HighscoreObj obj)
        {
            await ioHandler.SaveHighscoreObj(obj);
            HighscoreUpdatedEvent();
        }

        /// <summary>
        /// Plainly emoves the file.
        /// </summary>
        public async static void ClearHighscores()
        {
            await ioHandler.ClearHighscores();
            HighscoreUpdatedEvent();
        }

        public async static void SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            await ioHandler.SaveHighscoreObjs(objs);
            HighscoreUpdatedEvent();
        }

        /// <summary>
        /// Uses LINQ to sort a list of highscores by their distances.
        /// </summary>
        /// <param name="objs">Any list of highscores</param>
        /// <returns>Distance first highscores</returns>
        public async static Task<List<HighscoreObj>> SortHighestScoreFirst(List<HighscoreObj> objs)
        {
            if (!IsInitialised)
                await InitAsync();
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

    /// <summary>
    /// File interfacing interface.
    /// Required for bridging between different filetypes.
    /// </summary>
    public interface IHighscoreIOBridge
    {
        Task InitAsync();

        Task<List<HighscoreObj>> GetHighscoresAsync();

        Task SaveHighscoreObj(HighscoreObj obj);

        Task SaveHighscoreObjs(List<HighscoreObj> objs);

        Task ClearHighscores();
    }

    /// <summary>
    /// XML version
    /// </summary>
    class XMLIO : IHighscoreIOBridge
    {
        /// <summary>
        /// Makes the app aware of the state and existance of the Highscores XML file.
        /// Initialises booleans and creates file, if neccesary.
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            try
            {
                HighscoreIO.HighscoreFile = await ApplicationData.Current.RoamingFolder.GetFileAsync("Highscores.xml");
                HighscoreIO.IsInitialised = true;
                HighscoreIO.IsFileEmpty = await checkFileEmpty();
            }
            catch (FileNotFoundException)
            {
                CreateXMLfile();
            }
        }

        /// <summary>
        /// Create the file and rerun/continue initialisation.
        /// </summary>
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

        /// <summary>
        /// Check whether or not the .xml file starts with a node. If not label it as empty.
        /// </summary>
        private async Task<bool> checkFileEmpty()
        {
            System.Xml.XmlReader XmlReader = System.Xml.XmlReader.Create(await HighscoreIO.HighscoreFile.OpenStreamForReadAsync());
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

        /// <summary>
        /// Get all highscores unsorted.
        /// </summary>
        public async Task<List<HighscoreObj>> GetHighscoresAsync()
        {
            if (!HighscoreIO.IsInitialised)
                await InitAsync();
            if (HighscoreIO.IsFileEmpty)
                return new List<HighscoreObj>();
            List<HighscoreObj> retVal = new List<HighscoreObj>();           
            try
            {
                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(HighscoreIO.HighscoreFile);
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

        /// <summary>
        /// Saves a highscore object.
        /// If the file doesn't contain any highscore objects it will initialise it.
        /// If the file does contain any highscoreobject(s) it will append it.
        /// </summary>
        public async Task SaveHighscoreObj(HighscoreObj obj)
        {
            if (!HighscoreIO.IsInitialised)
                await InitAsync();
            if (HighscoreIO.IsFileEmpty)
            {
                //Create
                using (XmlWriter xmlWriter = XmlWriter.Create(await HighscoreIO.HighscoreFile.OpenStreamForWriteAsync()))
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
                HighscoreIO.IsFileEmpty = false;
            }
            else
            {
                //Append
                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(HighscoreIO.HighscoreFile);
                var node = xmlDoc.SelectSingleNode("Highscores");
                var newElement = xmlDoc.CreateElement("Highscore");
                newElement.SetAttribute("Name", obj.Name);
                newElement.SetAttribute("Distance", obj.Distance.ToString());
                newElement.SetAttribute("Time", obj.TimeTaken.ToString());
                node.AppendChild(newElement);
                await xmlDoc.SaveToFileAsync(HighscoreIO.HighscoreFile); 
            }
        }

        /// <summary>
        /// Cheap method.
        /// </summary>
        public async Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            foreach (var obj in objs)
            {
                await SaveHighscoreObj(obj);
            }
        }

        /// <summary>
        /// Deletes entire file.
        /// Warning: requires reinitialisation.
        /// </summary>
        public async Task ClearHighscores()
        {
            await HighscoreIO.HighscoreFile.DeleteAsync();
        }
    }

    /// <summary>
    /// Deprecated Json version
    /// </summary>
    public class JsonReader : IHighscoreIOBridge
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
                HighscoreIO.HighscoreFile = await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFileAsync("Highscore.Json");
                rawText = await FileIO.ReadTextAsync(HighscoreIO.HighscoreFile);
                highscoreCompilation = JsonObject.Parse(rawText)["stringRef"].GetObject();
                HighscoreIO.IsInitialised = true;
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
            if (!HighscoreIO.IsInitialised)
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
            if (!HighscoreIO.IsInitialised)
                await InitAsync();
            throw new NotImplementedException();
        }

        public async Task SaveHighscoreObjs(List<HighscoreObj> objs)
        {
            if (!HighscoreIO.IsInitialised)
                await InitAsync();
            throw new NotImplementedException();
        }


        public Task ClearHighscores()
        {
            throw new NotImplementedException();
        }
    }
}