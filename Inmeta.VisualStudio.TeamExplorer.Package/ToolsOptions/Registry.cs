using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Inmeta.VisualStudio.TeamExplorer.ToolsOptions
{
    internal abstract class RegistryBase
    {
        
        private string SubKeyName { get; set; }
        protected RegistryKey BaseKey { get; private set; }

        protected RegistryBase(RegistryHive basekey, string subkeyname)
        {
            BaseKey = RegistryKey.OpenBaseKey(basekey, RegistryView.Default); 
            SubKeyName = subkeyname;
        }

        internal T Read<T>(string property)
        {
            var key = BaseKey.OpenSubKey(SubKeyName);
            if (key == null)
                return default(T);
            var o = key.GetValue(property);
            return (T)o;
        }

        internal bool Exist
        {
            get
            {
                return BaseKey.OpenSubKey(SubKeyName) != null;
            }
        }

        internal void Write<T>(string property, T val)
        {
            var key = BaseKey.OpenSubKey(SubKeyName,true) ?? BaseKey.CreateSubKey(SubKeyName);
            key.SetValue(property, val);
            key.Close();
        }
    }

    internal class RegistryCurrentUser : RegistryBase
    {
        public RegistryCurrentUser(string subkeyname)
            : base(RegistryHive.CurrentUser, subkeyname)
        {
            
        }
    }
}
