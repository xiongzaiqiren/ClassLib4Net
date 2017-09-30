using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace ClassLib4Net
{
	public class WindowsAuthorityMock
	{
		#region 权限模拟
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		public static extern int LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
		[DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		/// <summary>
		/// 验证用户，并生成 WindowsIdentity 实例
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="domain"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private static WindowsIdentity GetIdentity(String userName, String domain, String password)
		{
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if (LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) == 0)
				return null;
			else if (DuplicateToken(token, 2, ref tokenDuplicate) == 0)
				return null;
			else
				return new WindowsIdentity(tokenDuplicate);
		}

		public static WindowsImpersonationContext GetContext(string identityString)
		{
			WindowsIdentity identity = null;
			WindowsImpersonationContext impersonationContext = null;
			if (identityString != null && identityString.Length != 0)
			{
				string[] s = identityString.Split(',');
				string user = s[0], pwd = s[1];
				identity = string.IsNullOrEmpty(user) ? null : GetIdentity(user, null, pwd);
				// 使用用户凭证进行用户模拟
				impersonationContext = (identity == null) ? null : identity.Impersonate();
			}
			return impersonationContext;
		}

		#endregion
	}
}
