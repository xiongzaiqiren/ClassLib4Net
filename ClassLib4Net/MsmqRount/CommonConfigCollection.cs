using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClassLib4Net.MsmqRount
{
    public class CommonConfigCollection : ConfigurationElementCollection
    {
        public CommonConfigCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CommonConfig();
        }


        protected override ConfigurationElement CreateNewElement(
            string key)
        {
            return new CommonConfig(key);
        }


        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((CommonConfig)element).Key;
        }


        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }


        }

        public new int Count
        {

            get { return base.Count; }

        }


        public CommonConfig this[int index]
        {
            get
            {
                return (CommonConfig)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CommonConfig this[string Name]
        {
            get
            {
                return (CommonConfig)BaseGet(Name);
            }
        }

        public int IndexOf(CommonConfig assembly)
        {
            return BaseIndexOf(assembly);
        }

        public void Add(CommonConfig assembly)
        {
            BaseAdd(assembly);

            // Add custom code here.
        }

        protected override void
            BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        //public void Remove(WebSite assembly)
        //{
        //    if (BaseIndexOf(assembly) >= 0)
        //        BaseRemove(assembly.Name);
        //}

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }

    }
}