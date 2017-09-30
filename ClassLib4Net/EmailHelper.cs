using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace ClassLib4Net
{
	/// <summary>
	/// 电子邮件助手
	/// </summary>
    public class EmailHelper
    {
        static Regex mailRegex = new Regex(@"\A(?:(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\]))\Z");
        
		/// <summary>
		/// 发送邮件的方法
        /// </summary>
		/// <param name="from">发件箱地址</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="displayName">发件人显示名</param>
		/// <param name="smtp">smtp服务器地址</param>
		/// <param name="smtpUsername">发件箱地址</param>
        /// <param name="smtpPassword">发件箱密码</param>
        /// <param name="isAysnc">异步</param>
        /// <param name="tos">密件抄送多人</param>
		public static void Send(string from, string title, string body, string displayName, string smtp = "mail.itv.cn", string smtpUsername = "lichun@itv.cn", string smtpPassword = "!QAZ2wsx", bool isAysnc = false, params string[] tos)
        {
            MailAddress sender = new MailAddress(from, displayName, Encoding.UTF8);

            MailMessage message = new MailMessage
            {
                From = sender,
                Body = body,
                Subject = title,
                BodyEncoding = Encoding.UTF8,
                Priority = MailPriority.High,
                IsBodyHtml = true
            };

            foreach (string bcc in tos)
            {
                if (mailRegex.IsMatch(bcc))
                {
                    message.Bcc.Add(bcc);
                }
            }

            SmtpClient client = new SmtpClient(smtp);

            client.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (isAysnc) client.SendAsync(message, null);
            else client.Send(message);
        }

		#region 发送邮件的方法
		/// <summary>
		/// 发送邮件的方法
		/// </summary>
		/// <param name="strSmtpServer">邮件服务器地址</param>
		/// <param name="strFrom">发送地址</param>
		/// <param name="strFromPass">发送密码</param>
		/// <param name="strto">接收地址</param>
		/// <param name="strSubject">邮件主题</param>
		/// <param name="strBody">邮件内容</param>
		/// <param name="isHtmlFormat">邮件内容是否以html格式发送</param>
		public static void SendEmail(string strSmtpServer, string strFrom, string strFromPass, string strto, string strSubject, string strBody, bool isHtmlFormat)
		{
			SendEmail(strSmtpServer, strFrom, strFromPass, strto, strSubject, strBody, isHtmlFormat, null);
		}

		/// <summary>
		/// 发送邮件的方法
		/// </summary>
		/// <param name="strSmtpServer">邮件服务器地址</param>
		/// <param name="strFrom">发送地址</param>
		/// <param name="strFromPass">发送密码</param>
		/// <param name="strto">接收地址</param>
		/// <param name="strSubject">邮件主题</param>
		/// <param name="strBody">邮件内容</param>
		/// <param name="isHtmlFormat">邮件内容是否以html格式发送</param>
		/// <param name="files">附件文件的集合</param>
		public static void SendEmail(string strSmtpServer, string strFrom, string strFromPass, string strto, string strSubject, string strBody, bool isHtmlFormat, string[] files)
		{
			try
			{
				SmtpClient client = new SmtpClient(strSmtpServer);
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(strFrom, strFromPass);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;

				MailMessage message = new MailMessage(strFrom, strto, strSubject, strBody);
				message.BodyEncoding = Encoding.UTF8;
				message.IsBodyHtml = isHtmlFormat;

				if (files != null)
				{
					for (int i = 0; i < files.Length; i++)
					{
						if (File.Exists(files[i]))
						{
							message.Attachments.Add(new Attachment(files[i]));
						}
					}
				}

				client.Send(message);
			}
			catch (Exception ex)
			{
				//throw new Exception("发送邮件失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 异步发送邮件的方法
		/// <summary>
		/// 异步发送邮件的方法
		/// </summary>
		/// <param name="strSmtpServer">邮件服务器地址</param>
		/// <param name="strFrom">发送地址</param>
		/// <param name="strFromPass">发送密码</param>
		/// <param name="strto">接收地址</param>
		/// <param name="strSubject">邮件主题</param>
		/// <param name="strBody">邮件内容</param>
		/// <param name="isHtmlFormat">邮件内容是否以html格式发送</param>
		/// <param name="files">附件文件的集合</param>
		/// <param name="userToken">一个用户定义对象，此对象将被传递给完成异步操作时所调用的方法。</param>
		/// <param name="onComplete">发送结束后的回调函数</param>
		public static void SendAsyncEmail(string strSmtpServer, string strFrom, string strFromPass, string strto, string strSubject, string strBody, bool isHtmlFormat, string[] files, object userToken, SendCompletedEventHandler onComplete)
		{
			try
			{
				SmtpClient client = new SmtpClient(strSmtpServer);
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(strFrom, strFromPass);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;

				MailMessage message = new MailMessage(strFrom, strto, strSubject, strBody);
				message.BodyEncoding = Encoding.UTF8;
				message.IsBodyHtml = isHtmlFormat;

				if (files != null)
				{
					for (int i = 0; i < files.Length; i++)
					{
						if (File.Exists(files[i]))
						{
							message.Attachments.Add(new Attachment(files[i]));
						}
					}
				}

				//绑定邮件发送完成事件
				client.SendCompleted += new SendCompletedEventHandler(onComplete);

				//异步发送
				client.SendAsync(message, userToken);
			}
			catch (Exception ex)
			{
				throw new Exception("发送邮件失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		/// <summary>
		/// 发送邮件的方法
		/// </summary>
		/// <param name="strSmtpServer">邮件服务器地址</param>
		/// <param name="strFrom">发送地址</param>
		/// <param name="strFromPass">发送密码</param>
		/// <param name="strto">接收地址(多个接收地址用逗号分隔)</param>
		/// <param name="strSubject">邮件主题</param>
		/// <param name="strBody">邮件内容</param>
		/// <param name="isHtmlFormat">邮件内容是否以html格式发送</param>
		/// <param name="files">附件文件的集合</param>
		public static int SendEmail2(string strSmtpServer, string strFrom, string strFromPass, string strto, string strSubject, string strBody, bool isHtmlFormat, System.Collections.ArrayList files)
		{
			int flag = 0;

			SmtpClient client = new SmtpClient(strSmtpServer);
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(strFrom, strFromPass);
			client.DeliveryMethod = SmtpDeliveryMethod.Network;

			string[] strtos = strto.Split(new char[] { ',' });
			for (int i = 0; i < strtos.Length; i++)
			{
				try
				{
					if (strtos[i].Trim().Length > 0)
					{
						MailMessage message = new MailMessage(strFrom, strtos[i].Trim(), strSubject, strBody);
						message.BodyEncoding = Encoding.Default;
						message.IsBodyHtml = isHtmlFormat;

						for (int j = 0; j < files.Count; j++)
						{
							if (File.Exists(files[j].ToString()))
							{
								message.Attachments.Add(new Attachment(files[j].ToString()));
							}
						}

						try
						{
							client.Send(message);
							flag++;
						}
						catch (System.Net.Mail.SmtpException ex)
						{
							continue;
						}
					}
				}
				catch (Exception err)
				{
					continue;
				}
			}
			return flag;
		}
    }
}
