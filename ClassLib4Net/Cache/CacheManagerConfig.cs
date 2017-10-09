using System;

namespace ClassLib4Net.Cache
{
	/// <summary>
	/// 缓存配置
	/// </summary>
	public class CacheManagerConfig
	{
		public static string CacheManagerType { get { return ConfigHelper.GetAppSetting("CacheManagerType"); } }
	}
}
