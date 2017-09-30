using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClassLib4Net.MsmqRount
{
    /*eg:
      add config at  configSections bleow
      <section name="ClassLib4Net.MSMQConfig" type="ClassLib4Net.MsmqRount.CommonPlatformConfiurationSectionHandler, ClassLib4Net" allowDefinition="MachineToApplication" restartOnExternalChanges="true"/>
      add MSMQConfig:
      <ClassLib4Net.MSMQConfig>
		<CommonConfig>
			<MsmqConfig Enabled="True" AllowMultiSendMessage="False" DefaultMessageSendArrdess=".\Private$\message">
				<MultiMessageAddress>
					<add MessageAddressName=".\Private$\message" Enabled="True"></add>
					<add MessageAddressName=".\Private$\message2" Enabled="True"></add>
				</MultiMessageAddress>
			</MsmqConfig>
		</CommonConfig>
	</ClassLib4Net.MSMQConfig>
     */
    /// <summary>
    /// 本类是一个多路由消息分发，路由控制类
    /// 对消息路由进行hash散列分发，
    /// 站点web.config应当做如下配置：
    /// <![CDATA[
    ///  add config at  configSections bleow
    /// <section name="ClassLib4Net.MSMQConfig" type="ClassLib4Net.MsmqRount.CommonPlatformConfiurationSectionHandler, ClassLib4Net" allowDefinition="MachineToApplication" restartOnExternalChanges="true"/>
    /// add MSMQConfig:
    ///<ClassLib4Net.MSMQConfig>
    ///<CommonConfig>
    ///<MsmqConfig Enabled="True" AllowMultiSendMessage="False" DefaultMessageSendArrdess=".\Private$\message">
    ///<MultiMessageAddress>
    ///<add MessageAddressName=".\Private$\message" Enabled="True"></add>
    ///<add MessageAddressName=".\Private$\message2" Enabled="True"></add>
    ///	</MultiMessageAddress>
    ///</MsmqConfig>
    ///	</CommonConfig>
    ///	</ClassLib4Net.MSMQConfig>
    /// ]]>
    /// </summary>
    public class MultiMessageAddressRount
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        internal static MsmqConfig GetConfig()
        {
            string webconfigPath = HttpContext.Current.Server.MapPath("~/Web.Config");

            //读取配置
            MsmqConfig config = null;
            try
            {
                config = CommonPlatformConfiguration.GetMsmqConfig();
            }
            catch
            {
                throw new Exception("从Web.Config读取配置MsmqConfig错误!");
            }
            return config;
        }

        /// <summary>
        /// 获取发送消息地址
        /// </summary>
        /// <returns></returns>
        public static string GetHashMessageAdress(object obj)
        {
            return GetHashMessageAdress(obj, "Default");
        }

        /// <summary>
        /// 获取发送消息地址
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string GetHashMessageAdress(object obj, string group)
        {
            MsmqConfig config = GetConfig();
            if (config == null || !config.Enabled)
            {
                return string.Empty;
            }
            var arrdessList = new List<string>();
            for (var i = 0; i < config.MultiMessageAddress.Count; i++)
            {
                if (config.MultiMessageAddress[i].Enabled && config.MultiMessageAddress[i].Group.Equals(group))
                {
                    arrdessList.Add(config.MultiMessageAddress[i].MessageAddressName);
                }
            }
            //启用分流
            if (arrdessList.Count > 0 && config.AllowMultiMessage)
            {
                if (config.MultiMessageAddress.Count == 1)
                {
                    return config.MultiMessageAddress[0].MessageAddressName;
                }
                else
                {
                    var hsIndex = Math.Abs(obj.GetHashCode()) % arrdessList.Count;
                    return arrdessList[hsIndex];
                }
            }
            else
            {
                return config.DefaultMessageSendArrdess;
            }
        }
    }
}