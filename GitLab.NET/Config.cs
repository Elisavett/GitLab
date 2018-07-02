using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;

namespace GitLabDotNet
{
    public class Config
    {
        private string _Name = "";
        private string _GitLabURI = "";
        private string _APIKey = "";
        private string _DefaultProjectLocation = "";
        private bool _PreferHTTPPush = false;
        private string RegKey;
        private Guid _Identity;

        public Guid Identity
        {
            get { return _Identity; }
        }

        public bool PreferHTTPPush
        {
            get { return _PreferHTTPPush; }
            set
            {
                _PreferHTTPPush = value;
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        public string GitLabURI
        {
            get { return _GitLabURI; }
            set
            {
                _GitLabURI = value;
            }
        }

        public int apiVersion = 3;

        string apiPath
        {
            get
            {
                return "api/v" + apiVersion.ToString() + "/";
            }
        }

        public string APIUrl
        {
            get
            {
                string RetVal = GitLabURI;

                if (RetVal != null)
                {
                    if (!RetVal.EndsWith("/"))
                        RetVal += "/";

                    RetVal += apiPath;
                }
                return RetVal;
            }
        }


        public string DefaultProjectLocation
        {
            get { return _DefaultProjectLocation; }
            set
            {

                _DefaultProjectLocation = value;
            }
        }

        public string APIKey
        {
            get { return _APIKey; }
            set
            {
                _APIKey = value;
            }
        }

        public Config(Guid _identity = new Guid())
        {
            if (_identity == Guid.Empty)
                _identity = Guid.NewGuid();

            _Identity = _identity;
            RegKey = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
            Load();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Save()
        {
            Registry.Write(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "PreferHTTPPush", _PreferHTTPPush.ToString(), RegistryValueKind.String);
            Registry.Write(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "Name", _Name, RegistryValueKind.String);
            Registry.Write(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "GitLabURI", _GitLabURI, RegistryValueKind.String);
            Registry.Write(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "DefaultProjectLocation", _DefaultProjectLocation, RegistryValueKind.String);
            Registry.Write(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "APIKey", _APIKey, RegistryValueKind.String);
        }

        public void Load()
        {
            if (Registry.Check(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "GitLabURI"))
                _GitLabURI = Registry.Read(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "GitLabURI").ToString();

            if (Registry.Check(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "APIKey"))
                _APIKey = Registry.Read(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "APIKey").ToString();

            if (Registry.Check(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "DefaultProjectLocation"))
                _DefaultProjectLocation = Registry.Read(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "DefaultProjectLocation").ToString();

            if (Registry.Check(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "PreferHTTPPush"))
                _PreferHTTPPush = Convert.ToBoolean(Registry.Read(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "PreferHTTPPush").ToString());

            if (Registry.Check(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "Name"))
                Name = Registry.Read(RegistryHive.CurrentUser, "SOFTWARE\\"+RegKey+"\\" + _Identity.ToString(), "Name").ToString();
        }

        public static Collection<Config> GetAllConfigs()
        {
            Collection<Config> retVal = new Collection<Config>();
            string RegKey = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
            string[] keys = Registry.GetAllSubkeys(RegistryHive.CurrentUser, "SOFTWARE\\" + RegKey );

            if (keys != null)
                foreach (string key in keys)
                {
                    Guid outGuid = new Guid();

                    if (Guid.TryParse(key, out outGuid))
                    {
                        retVal.Add(new Config(outGuid));
                    }
                }

            return retVal;
        }        
    }
}

