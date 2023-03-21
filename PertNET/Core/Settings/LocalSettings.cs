namespace PertNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using EasyPrototypingNET.Core;

    public class LocalSettings
    {
        private List<SettingsContent> settings;

        public LocalSettings()
        {
            settings = new List<SettingsContent>();
        }

        public LocalSettings(IEnumerable<SettingsContent> settings) : this()
        {
            foreach (SettingsContent item in settings)
            {
                this.AddOrSet(item.Key, Type.GetType(item.Type), item.Value);
            }
        }

        ~LocalSettings()
        {
            settings.Clear();
        }

        public string Filename
        {
            get
            {
                string settingsPath = this.CurrentSettingsPath();
                string settingsName = this.UserSettingsName();
                string settingsFile = Path.Combine(settingsPath, settingsName);
                return settingsFile;
            }
        }

        public string Pathname
        {
            get
            {
                return $"{this.CurrentSettingsPath()}\\";
            }
        }

        public void AddOrSet(string key, Type type, object valueContent)
        {
            if (this.Exists(key) == true)
            {
                int keyIndex = this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                this.settings[keyIndex].Type = type.Name;
                this.settings[keyIndex].Value = valueContent;
            }
            else
            {
                this.settings.Add(new SettingsContent() { Key = key, Type = type.Name, Value = valueContent});
            }
        }

        public void AddOrSet(string key, object valueContent)
        {
            if (this.Exists(key) == true)
            {
                int keyIndex = this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                this.settings[keyIndex].Type = typeof(string).Name;
                this.settings[keyIndex].Value = valueContent;
            }
            else
            {
                this.settings.Add(new SettingsContent() { Key = key, Type = typeof(string).Name, Value = valueContent });
            }
        }

        public object Get(string key)
        {
            lock (this.settings)
            {
                if (this.Exists(key) == true)
                {
                    int keyIndex = this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    return this.settings[keyIndex].Value; 
                }
                else
                {
                    return default;
                }
            }
        }

        public SettingsContent GetContent(string key)
        {
            lock (this.settings)
            {
                if (this.Exists(key) == true)
                {
                    int keyIndex = this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    return this.settings[keyIndex];
                }
                else
                {
                    return default;
                }
            }
        }

        public object this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }

        public void Remove(string key)
        {
            lock (this.settings)
            {
                if (this.Exists(key) == true)
                {
                    int keyIndex = this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    this.settings.RemoveAt(keyIndex);
                }
            }
        }

        public void RemoveAll()
        {
            if (this.settings != null)
            {
                lock (this.settings)
                {
                    this.settings.Clear();
                }
            }
        }

        public bool Exists(string key)
        {
            lock (this.settings)
            {
                return this.settings.FindIndex(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) != -1;

            }
        }

        public int Count()
        {
            int count = -1;
            if (this.settings != null)
            {
                lock (this.settings)
                {
                    count = this.settings.Count();
                }
            }

            return (count);
        }

        public bool IsExitSettings()
        {
            if (File.Exists(this.Filename) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Save()
        {
            try
            {
                SerializeHelper<List<SettingsContent>>.Serialize(settings, SerializeFormatter.Xml, this.Filename);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public void Load()
        {
            try
            {
                if (this.settings != null)
                {
                    this.settings.Clear();
                    this.settings = SerializeHelper<List<SettingsContent>>.DeSerialize(SerializeFormatter.Xml, this.Filename);
                }
                else
                {
                    this.settings = new List<SettingsContent>();
                    this.settings = SerializeHelper<List<SettingsContent>>.DeSerialize(SerializeFormatter.Xml, this.Filename);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private string CurrentSettingsPath()
        {
            string settingsPath = string.Empty;

            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            settingsPath = $"{rootPath}\\{this.ApplicationName()}\\Settings";

            if (string.IsNullOrEmpty(settingsPath) == false)
            {
                try
                {
                    if (Directory.Exists(settingsPath) == false)
                    {
                        Directory.CreateDirectory(settingsPath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return settingsPath;
        }

        private string ApplicationName()
        {
            string result = string.Empty;

            Assembly assm = Assembly.GetEntryAssembly();
            result = assm.GetName().Name;
            return result;
        }

        private string UserSettingsName()
        {
            string result = string.Empty;
            string username = Environment.UserName;

            result = $"{username}.Settings";

            return result;
        }
    }
}