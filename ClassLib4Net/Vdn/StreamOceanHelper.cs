using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net.Vdn
{
    public class StreamOceanHelper
    {
        public enum ConentType
        {
            Vod,
            VirtualChannel,
            Live,
        }

        public enum Platform
        {
            Stb = 1,
            Ipad = 2,
            Pc = 3
        }

        public enum Size
        {
            S1028p,
            S720p
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="platform"></param>
        /// <param name="contentId"></param>
        /// <param name="isRateAdapt"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static string GetUrl(ConentType contentType, Platform platform, Guid contentId, bool isRateAdapt = false, string fmt = null, int delay = 0)
        {

            StringBuilder sb = new StringBuilder("http://" + ConfigHelper.GetAppSetting("StreamOcean.VideoDomain." + contentType));

            sb.AppendFormat("/{0}/", contentType.ToString().ToLowerInvariant())
                //.AppendFormat(@"{0:yyyy\/MM\/dd\/}", DateTime.Today)
                .AppendFormat("{0:N}.ts", contentId);
            string bitrateString = ConfigHelper.GetAppSetting(string.Format("ForceBitrate.{0}", contentType)) ?? "0";
            
            if (fmt == null)
            {
                switch (platform)
                {
                    case Platform.Pc:
                        sb.AppendFormat("?fmt=x264_{0}k_flv", bitrateString);
                        break;
                    case Platform.Stb:
                        if (contentType == ConentType.Live) 
                        {
                            sb.AppendFormat("?fmt=x264_{0}k_mpegts", bitrateString);
                        }
                        else
                        {
                            sb.AppendFormat("?fmt=x264_{0}k_mpegts", bitrateString);
                        }
                        break;
                    case Platform.Ipad:
                        if (contentType == ConentType.Live)
                        {
                            sb.AppendFormat(".m3u8?fmt=x264_1500k_mpegts", bitrateString);
                        }
                        else
                        {
                            sb.AppendFormat(".m3u8?fmt=x264_{0}k_mpegts", bitrateString);
                        }
                        break;
                    default:
                        sb.AppendFormat("?fmt=x264_{0}k_mpegts", bitrateString);
                        break;
                }
            }
            else
            {
                sb.Append("?fmt=").Append(fmt);
            }

            if (isRateAdapt) sb.Append("&sora=1");
            if (delay > 0 && (contentType == ConentType.Live || contentType == ConentType.VirtualChannel))
            {
                sb.AppendFormat("&delay={0}", delay);
            }
            return sb.ToString();
        }
    }
}
