
using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;

namespace JJ.SecureTokenService.Configuration
{
    public class ServiceLocatorSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ServiceInstanceCollection Instances
        {
            get { return (ServiceInstanceCollection)this[""]; }
            set { this[""] = value; }
        }
    }
    public class ServiceInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((ServiceInstanceElement)element).Name;
        }
    }

    public class ServiceInstanceElement : ConfigurationElement
    {
        public override bool IsReadOnly()
        {
            return false;
        }

        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        [ConfigurationProperty("providerType", IsRequired = true)]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }
}
