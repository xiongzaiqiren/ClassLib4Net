using System;
using System.Configuration;

namespace ClassLib4Net.MsmqRount
{
    public class MsmqConfig : ConfigurationElement
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        [ConfigurationProperty("Enabled", DefaultValue = "True", IsRequired = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["Enabled"];
            }
            set
            {
                this["Enabled"] = value;
            }
        }

        /// <summary>
        /// 是否启用消息分流机制
        /// </summary>
        [ConfigurationProperty("AllowMultiSendMessage", DefaultValue = "False")]
        public bool AllowMultiMessage
        {
            get
            {
                return (bool)this["AllowMultiSendMessage"];
            }
            set
            {
                this["AllowMultiSendMessage"] = value;
            }
        }
        /// <summary>
        /// 默认发送的地址
        /// </summary>
        [ConfigurationProperty("DefaultMessageSendArrdess", DefaultValue = "")]
        public string DefaultMessageSendArrdess
        {
            get
            {
                return (string)this["DefaultMessageSendArrdess"];
            }
            set
            {
                this["DefaultMessageSendArrdess"] = value;
            }
        }
        /// <summary>
        /// 分流地址
        /// </summary>
        [ConfigurationProperty("MultiMessageAddress", IsRequired = true)]
        public MultiMessageAddress MultiMessageAddress
        {
            get
            {
                return (MultiMessageAddress)this["MultiMessageAddress"];
            }

            set
            {
                this["MultiMessageAddress"] = value;
            }
        }

        [ConfigurationProperty("Group", DefaultValue = "Default", IsRequired = false)]
        public string Group
        {
            get
            {
                return (string)this["Group"];
            }
            set
            {
                this["Group"] = value;
            }
        }
    }

    /// <summary>
    /// 队列地址信息
    /// </summary>
    public class MessageAddress : ConfigurationElement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageAddress()
        {
        }
        public MessageAddress(string messageAddressName)
        {
            MessageAddressName = messageAddressName;
        }

        /// <summary>
        /// 队列地址名称
        /// </summary>
        [ConfigurationProperty("MessageAddressName", IsRequired = true, IsKey = true)]
        [StringValidator(MinLength = 0, MaxLength = 600)]
        public string MessageAddressName
        {
            get
            {
                return (string)this["MessageAddressName"];
            }
            set
            {
                this["MessageAddressName"] = value;
            }
        }

        /// <summary>
        /// 分组
        /// </summary>
        [ConfigurationProperty("Group", DefaultValue = "Default", IsRequired = false)]
        [StringValidator(MinLength = 0, MaxLength = 100)]
        public string Group
        {
            get
            {
                return (string)this["Group"];
            }
            set
            {
                this["Group"] = value;
            }
        }

        /// <summary>
        /// 该队列地址是否可用
        /// </summary>
        [ConfigurationProperty("Enabled", IsRequired = true, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["Enabled"];
            }
            set
            {
                this["Enabled"] = value;
            }
        }
    }

    /// <summary>
    /// 队列地址集合
    /// </summary>
    public class MultiMessageAddress : ConfigurationElementCollection
    {
        public MultiMessageAddress()
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
            return new MessageAddress();
        }


        protected override ConfigurationElement CreateNewElement(
            string elementName)
        {
            return new MessageAddress(elementName);
        }


        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((MessageAddress)element).MessageAddressName;
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


        public MessageAddress this[int index]
        {
            get
            {
                return (MessageAddress)BaseGet(index);
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

        new public MessageAddress this[string Name]
        {
            get
            {
                return (MessageAddress)BaseGet(Name);
            }
        }

        public int IndexOf(MessageAddress assembly)
        {
            return BaseIndexOf(assembly);
        }

        public void Add(MessageAddress assembly)
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

        public void Remove(MessageAddress assembly)
        {
            if (BaseIndexOf(assembly) >= 0)
                BaseRemove(assembly.MessageAddressName);
        }

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