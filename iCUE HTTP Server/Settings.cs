using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace iCUE_HTTP_Server
{
    class Settings
    {
        public static string settingsDir = "./settings.json";

        // Key: process name, Value: profile object
        public static Dictionary<string, Profile> processControllers;

        public static string defaultSettingsJSON = "{\n\t\"Profiles\":\n\t[\n\t\t{\n\t\t\t\"Process\": \"iCUE\",\n\t\t\t\"AutoSetGame\": \"iCUE\"\n\t\t},\n\t\t{\n\t\t\t\"Process\": \"ExampleProcess\",\n\t\t\t\"Controller\": \"./controllers/example/example_controller.exe\",\n\t\t\t\"CloseWithProcess\": true,\n\t\t\t\"CloseOnProfileSwitch\": false,\n\t\t\t\"EmbedController\": false,\n\t\t\t\"CommandLineArgs\": \"\",\n\t\t\t\"AutoSetGame\": \"\",\n\t\t\t\"AutoSetGameOnClose\": \"\",\n\t\t\t\"LockSetGame\": \"\"\n\t\t}\n\t]\n}";

        private static string pre { get { return string.Format("{0}   Settings -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        // Load the settings from file
        public static void LoadSettings ()
        {
            // If no settings file exists, create a blank one
            if (!File.Exists(settingsDir))
            {
                File.WriteAllText(settingsDir, defaultSettingsJSON);
            }

            // Reset variables
            processControllers = null;
            
            // Read text from file
            string contents;
            using (StreamReader sr = new StreamReader(settingsDir, Encoding.UTF8))
            {
                contents = sr.ReadToEnd();
            }

            // Read data from JSON
            SettingsJSON jsonSettings = JsonConvert.DeserializeObject<SettingsJSON>(contents);

            processControllers = new Dictionary<string, Profile>();
            foreach (ProfileJSON profile in jsonSettings.Profiles)
            {
                processControllers[profile.Process] = profile.ToRegularProfile();
            }
        }

        public struct Profile
        {
            // Process name is stored as the key in the stored dictionary
            public string controller;
            public bool closeWithProcess;
            public bool closeOnProfileSwitch;
            public bool embedController;
            public string commandLineArgs;
            public string autoSetGame;
            public string autoSetGameOnClose;
            public string lockSetGame;
        }

        // Classes used for easy deserialise JSON
        private class SettingsJSON
        {
            public IList<ProfileJSON> Profiles { get; set; }
        }

        private class ProfileJSON
        {
            public string Process { get; set; }
            public string Controller { get; set; }
            public bool CloseWithProcess { get; set; }
            public bool CloseOnProfileSwitch { get; set; }
            public bool EmbedController { get; set; }
            public string CommandLineArgs { get; set; }
            public string AutoSetGame { get; set; }
            public string AutoSetGameOnClose { get; set; }
            public string LockSetGame { get; set; }

            public Profile ToRegularProfile ()
            {
                return new Profile() { controller = Controller, closeWithProcess = CloseWithProcess, closeOnProfileSwitch = CloseOnProfileSwitch, embedController = EmbedController, commandLineArgs = CommandLineArgs, autoSetGame = AutoSetGame, autoSetGameOnClose = AutoSetGameOnClose, lockSetGame = LockSetGame };
            }
        }
    }  
}
