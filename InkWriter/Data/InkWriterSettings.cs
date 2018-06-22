using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace InkWriter.Data
{
    /// <summary>Settings class for InkWriter application.</summary>
    [XmlRoot()]
    public class InkWriterSettings
    {
        public List<MailboxSettings> MailboxSettings { get; private set; }

        public static InkWriterSettings Load()
        {
            if (File.Exists(SettingsPath))
            {
                try
                {
                    using (TextReader reader = new StreamReader(SettingsPath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(InkWriterSettings));
                        InkWriterSettings loadedSettings = (InkWriterSettings)serializer.Deserialize(reader);
                        return loadedSettings;
                    }
                }
                catch
                {
                    return new InkWriterSettings();
                }
            }
            else
            {
                return new InkWriterSettings();
            }
        }

        public void Save()
        {
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            using (TextWriter writer = new StreamWriter(SettingsPath, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InkWriterSettings));
                serializer.Serialize(writer, this);
            }
        }

        private static string SettingsDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            }
        }

        private static string SettingsPath
        {
            get
            {
                return Path.Combine(SettingsDirectory, "settings.xml");
            }
        }
    }
}
