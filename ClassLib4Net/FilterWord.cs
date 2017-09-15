using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassLib4Net
{
    /// <summary>
	/// 过滤词处理
	/// 熊学浩
	/// </summary>
	public static class FilterWord
    {
        /// <summary>
        /// 正则表达式验证匹配
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 正则表达式替换字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Replace(string input, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern) || replacement == null) return string.Empty;
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 正则表达式分割字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Split(string input, string pattern)
        {
            string[] arrayString = Regex.Split(input, pattern, RegexOptions.IgnoreCase);
            List<string> _arrayString = new List<string>();
            foreach (string s in arrayString)
            {
                if (!string.IsNullOrWhiteSpace(s) && !Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase))
                    _arrayString.Add(s);
                else
                    continue;
            }
            return _arrayString.ToArray();
        }

        #region 清理字符串
        /// <summary>
        /// 移除字符串中所有空格，制表符，回车，换行，分页符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpace(string str)
        {
            return Regex.Replace(str, @"\s", ""); //移除空格，制表符，回车，换行，分页符
        }
        /// <summary>
        /// 清除字符串中的换行，分段，分页，制表符等
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return str
                .Replace("^|", "")
                .Replace("^p", "")
                .Replace("^t", "")
                .Replace("^m", "")
                .Replace("\n", "") //换行 0x000A
                .Replace("\r", "") //回车 0x000D
                .Replace("\f", "") //换页 0x000C
                .Replace("\t", "") //水平制表符 0x0009
                .Replace("\v", "") //垂直制表符 0x000B
                .Trim();
        }

        /// <summary>
        /// 转义JSON中数据特殊字符（转义处理双引号，正斜杠，反斜杠，换行符号等）
        /// </summary>
        /// <param name="text">JSON中数据文本</param>
        /// <returns>转义后的JSON中数据文本方便反序列化</returns>
        public static string EscapeToJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            StringBuilder sb = new StringBuilder();
            Char[] stringArray = text.ToCharArray();
            for (long i = 0; i < stringArray.LongLength; i++)
            {
                switch (stringArray[i])
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\v':
                        sb.Append("\\v");
                        break;
                    default:
                        sb.Append(stringArray[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 移除字符串中所有单引号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSingleQuotes(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return str.Replace("'", "");
        }

        /// <summary>
        /// 清理字符串中的正则表达式符号 (\ ^ $ * + ? { } . ( ) : = ! [ ] | -)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearRegex(string text)
        {
            text = Replace(
                Replace(
                    Replace(
                        Replace(
                            Replace(
                                Replace(
                                    Replace(
                                        Replace(
                                            text
                                            , @"(\,)", "，")
                                        , @"(\.)", "。")
                                    , @"(\()", "（")
                                , @"(\))", "）")
                            , @"(\:)", "：")
                        , @"(\!)", "！")
                    , @"(\[)", "【")
                , @"(\])", "】");
            return Replace(text, @"(\\|\^|\$|\*|\+|\?|\{|\}|\,|\.|\(|\)|\:|\=|\!|\[|\]|\||\-)", "");
        }
        #endregion



        #region 常用的格式匹配
        /// <summary>
        /// 是否匹配Guid格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchGuid(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                return Regex.IsMatch(input, @"^[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}$", RegexOptions.IgnoreCase);
            else
                return false;
        }
        #endregion

        #region 获取字符串的实际字节长度的方法
        /// <summary>
        /// 获取字符串的实际字节长度的方法
        /// </summary>
        /// <param name="source">字符串</param>
        /// <returns>实际长度</returns>
        public static int GetRealLength(string source)
        {
            return Encoding.Default.GetByteCount(source);
        }
        #endregion

        #region 按字节数截取字符串的方法
        /// <summary>
        /// 按字节数截取字符串的方法
        /// </summary>
        /// <param name="source">要截取的字符串（可空）</param>
        /// <param name="length">要截取的字节数</param>
        /// <param name="suffix">结果字符串的后缀（超出部分显示为该后缀）</param>
        /// <returns></returns>
        public static string SubString(string source, int length, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(source) || length < 1) return string.Empty;
            string temp = string.Empty;
            if (GetRealLength(source) <= length)//如果长度比需要的长度length小,返回原字符串
            {
                return source;
            }
            else
            {
                int t = 0;
                char[] q = source.ToCharArray();
                for (int i = 0; i < q.Length && t < length; i++)
                {
                    if ((int)q[i] > 127)//是否汉字
                    {
                        temp += q[i];
                        t += 2;
                    }
                    else
                    {
                        temp += q[i];
                        t++;
                    }
                }
                if (!string.IsNullOrEmpty(suffix))
                    temp += suffix;
                return temp;
            }
        }
        #endregion


        #region 清除html标签

        /// <summary>
        /// 清除html标签
        /// 贾志勇
        /// 2014-12-9
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <returns></returns>
        public static string RemoveHtml(string htmlstring)
        {
            if (string.IsNullOrWhiteSpace(htmlstring))
                return string.Empty;
            //删除脚本
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            //htmlstring.Replace("<", "");
            //htmlstring.Replace(">", "");
            //htmlstring.Replace("\r\n", "");
            //htmlstring.Replace("&nbsp;", "");
            //htmlstring = System.Web.HttpContext.Current.Server.HtmlEncode(htmlstring).Trim();
            return htmlstring;
        }

        ///<summary>
        ///替换字符串中的特殊字符为html标记
        /// 贾志勇
        /// 2014-12-9
        ///</summary>
        ///<param name="theString">需要进行替换的文本。</param>
        ///<returns>替换完的文本。</returns>
        public static string HtmlEncode(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return string.Empty;
            theString = theString.Replace(">", "&gt;");
            theString = theString.Replace("<", "&lt;");
            theString = theString.Replace(" ", "&nbsp;");
            theString = theString.Replace("\"", "&quot;");
            theString = theString.Replace("\'", "&#39;");
            theString = theString.Replace("\n", "<br/>");
            theString = theString.Replace("&", "&amp;");
            return theString;
        }

        ///<summary>
        ///恢复字符串中的特殊字符
        /// 贾志勇
        /// 2014-12-9
        ///</summary>
        ///<param name="theString">需要恢复的文本。</param>
        ///<returns>恢复好的文本。</returns>
        public static string HtmlDiscode(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return string.Empty;
            theString = theString.Replace("&gt;", ">");
            theString = theString.Replace("&lt;", "<");
            theString = theString.Replace("&nbsp;", " ");
            theString = theString.Replace("&quot;", "\"");
            theString = theString.Replace("&#39;", "\'");
            theString = theString.Replace("<br/>", "\n");
            return theString;
        }


        #endregion


        #region 过滤词
        /// <summary>
        /// 是否包含过滤词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Contain(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            bool result = false;
            if (!result) result = Regex.IsMatch(text, filterWord, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return result;
        }

        /// <summary>
        /// 过滤处理
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="replacement">替换字符串</param>
        /// <returns>过滤处理后的字符串</returns>
        public static string Filter(string text, string replacement)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return Regex.Replace(
                text
                , filterWord, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        /// <summary>
        /// 过滤处理，替换字符串“*”
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns></returns>
        public static string Filter(string text)
        {
            return Filter(text, "*");
        }

        #region 过滤词集
        /// <summary>
        /// 过滤词
        /// </summary>
        private const string filterWord = @"(18禁|1Pondo|3D轮盘|4事件|64时期|6合|6四|7个军区|8023部队|89风波|89年|89事件|89之|9.12事件|98反华|9ping|9评|9坪|A级|A片|bignews|bitch|boxun|CCP|cdjp|chinaliberal|chinamz|chinesenewsnet|cnd|creaders|dafa|dajiyuan|ddgcd|dfdz|DJY|downloader|dpp|E周刊|fa lun|falong|falu|falun|falundafa|fa轮|flg|FL大法|FL功|freechina|freenet|fuck|GCD|gong chan dang|GONG党|hentai|hongzhi|hrichina|huanet|hypermart|h漫|h图|incest|jiuping|K粉|LHZ|lihongzhi|liuhecai|liusi|minghui|minghuinews|nacb|nmis|ONS|peacehall|PK黑社会|playboy|porn|renminbao|renmingbao|rfa|safeweb|SB|shit|svdc|taip|TAM|tiananmen|tianwang|tibetalk|TMD|TND|TNND|triangleboy|tusha|UltraSurf|unixbox|ustibet|voa|wangce|wstaiji|www.bignews.org|www.boxun.com|www.cdjp.org|www.chinamz.org|www.chinesenewsnet.com|www.cnd.org|www.dajiyuan.com|www.renminbao.com|www.rfa.org |www.tibetalk.com|xiaoping|xinsheng|xuechao|xxx|XX功|yuming|zh.wikipedia.org|zhengjian|zhengjianwang|zhenshanren|zhuanfalun|zifen|八九六四|白立朴|白梦|百家乐|拜大哥|办理文凭|办理证件|帮人怀孕|包过|包养|包夜|保钓|保过|报码|暴力袭警|暴亂|北大三角地论坛|北京 独立|北京当局|北京风波|北京之春|北美自由论坛|被虐|边界新约|编成与基地|鞭满|婊子|冰毒|波霸|博讯|薄格|不爱党|彩宝|藏獨|藏字石|操B|操你|操你妈|操你奶奶|曹刚川|厕奴|插B|插插|插她|插你|插他|插我|柴玲|長瀨愛|常劲|朝河蘭|潮吹|车仑大法|成人片|程铁军|程真|赤匪|赤化|臭作|出售 答案|出售 器官|出售 枪|出售 肾|出售假币|出售枪支|出售手枪|川島和津實|传送答案|吹萧|春夏自由论坛|春药|催情药|达赖|打飞机|打炮|大参考|大法弟子|大盖帽|大蓋帽|大鸡巴|大纪元|大纪元时报|大纪元新闻网|大纪园|大妓院|大紀元|大家论坛|大澤惠|大中国论坛|大中华论坛|大众真人真事|代办 证件|代开发票|代考|代写论文|代孕|带考|戴海静|盗取 卡号|盗取qq|盗取密码|邓矮子|邓狗|邓笑贫|迪里夏提|弟疼|弟痛|第二首都|电狗|电话 交友|电鸡|电警棒|电视流氓|东方闪电|东南西北论谈|东西南北论坛|董元辰|独夫|独夫民贼|独立台湾会|杜智富|短信答案|短信群发器|段桂清|多黨|屙民|二十四事件|发愣|发抡|发仑|发伦|发囵|发沦|发轮|发轮功|发正念|發倫|發淪|發輪|發論|法 轮 功|法0功|法L |法lun|法O功|法X功|法抡|法仑|法仑功|法伦|法囵|法沦|法纶|法轮|法轮大法|法轮功|法倫|法淪|法輪|法論|法某功|反G|反party|反封锁技术|饭岛爱|飯島愛|防区和任务|仿真枪|仿真手枪|飞扬论坛|斐得勒|废统|费良勇|分家在|粉饰太平|风雨神州|风雨神州论坛|封从德|風花|佛展千手法|夫妻交换|干她|干你|干他|肛交|肛门|港独|高 智 晟|高勤荣|高文谦|高校暴乱|高校骚乱|高瞻|睾丸|戈扬|哥疼|哥痛|根敦.确吉|根敦.确吉尼玛|公产党|功法|功友|共X党|共独|共匪|共狗|共贼|古方迷香|古月金帛涛|关卓中|官匪一家|贯通两极法|龟头|国统纲领|国统会|国研新闻邮件|国贼|红灯区|红客联盟|红色恐怖|红志|红智|宏法|宏志|洪传|洪吟|洪志|洪智|紅志|紅智|花花公子|花园网|华建敏|华通时事论坛|华闻 王政|华语世界论坛|华岳时事论坛|换肾|黄色电影|黄色小电影|黄翔|活体取肾|鸡巴|鸡毛信文汇|姬胜德|积克馆|激情 电话|激情 交友|激情视频|激情小电影|及川奈央|级答案|集体 自杀|寂寞少妇|佳静安定片|賈慶林|监听器|监听王|简鸿章|建国党|江core|江八点|江独裁|江流氓|江罗|江绵恒|江戏子|江泽慧|江澤民|江贼|江贼民|江折民|江猪|江猪媳|江主席|将则民|僵贼|僵贼民|疆獨|疆独|疆独藏独|酱猪媳|叫床|叫春|姐疼|姐痛|金尧如|金澤文子|精液|警匪一家|警民冲突|靖国|九ping|九抨|九凭|开苞|看JJ|考后付款|考前答案|考前密卷|考前预测|考试作弊|考中答案|嗑药|空军部署|口交|口淫|狂操|狂插|狂干|狂搞|劳动教养所|老虎机|老江|老毛|黎安友|黎阳平|李 宏 志|李大轮子|李大师|李登辉|李红志|李红痔|李宏志|李洪X|李洪宽|李洪志|李月月鸟|历史的伤口|连胜德|連發|联盟党|联总|联总之声传单|廉政大论坛|炼功|梁光烈|梁擎墩|凌锋|凌辱|菱恝|流血事件|六4|六HE彩|六合|六合彩|六级 答案|六四|六四 |六四事件|龙虎豹|龙卷风|龙新民|露点|露毛|露乳|陆独|陆军部署|陆肆|陆委会|乱交|抡功|伦公|伦功|伦攻|沦公|沦功|沦攻|轮大|轮公|轮功|轮攻|轮子功|倫公|倫功|倫攻|淪公|淪功|淪攻|輪公|輪功|輪攻|论公|论功|论攻|論公|論功|論攻|羅幹|裸聊|妈个|媽個|麻衣|麻醉钢枪|麻醉枪|麻醉药|麻醉乙醚|买卖枪支|买枪|买肾|买真枪|卖国|卖枪|卖身|毛厕洞|毛一鲜|毛贼东|美 国 之 音|美国参考|美国之音|妹疼|妹痛|蒙独|蒙汗药|蒙汗药粉|迷幻药|迷昏药|迷魂药|迷奸药|迷药|绵恒|灭共|灭日|民联|民意论坛|民运|民運|民阵|民猪|民主还专政|明慧|莫伟强|母子乱伦|木子论坛|内射|奶子|男奴|南大自由论坛|嫩逼|倪育贤|你妈逼|你说我说论坛|你他妈|女奴|女女|女士服务|女優|拍肩神药|潘国平|盘古乐队|泡友|陪聊|喷精|喷尿|屁眼|騙局|嫖娼|破处|破网软件|蒲团|七大军区|七个军区|枪击藏民|枪决女犯|枪决现场|枪手|枪支|枪支弹药|强歼|强效失意药|抢粮记|窃听器|窃听器材|轻舟快讯|情色|求肾|犬交|确吉尼玛 |群交|群射|热比娅|热站政论网|人民报|人民报讯|人民大众时事参考|人民内情真相|人民真实|人民真实报导|人民真实报道|人民之声论坛|人兽杂交|肉棒|肉洞|肉棍|肉欲|乳交|软弱外交|瑞安事件|三级 答案|三级片|三陪|三浦愛佳|三唑仑|骚女|色狼|色情服务|色情小电影|色友|杀害学生|杀手 雇佣|傻B|傻逼|上海 独立|上海帮|上海帮|上海孤儿院|社会主义灭亡|射精|身份证生成器|身体健康 肾|神通加持法|升达毕业证|时代论坛|史久武|示wei|收 小弟|手机复制|手机监听|兽交|爽死|谁是新中国|税力|顺利过|私处|四川独立|四大舰队|四级 答案|四事件|台du|台獨|台盟|台湾 独立|台湾狗|台湾国|台湾建国运动组织|台湾青年独立联盟|台湾政论区|台湾自由联盟|臺灣獨立|太子党|特码|提供 答案|替考|天安门母亲|天鹅之旅|天怒|铁血师|童屹|统独|统独论坛|捅你|捅我|投毒杀人|透视眼镜|屠杀学生|退党|退党保平安|外交论坛|外交与方略|外蒙 回归|万维读者论坛|汪岷|亡党|亡共|网络封锁|网络警察|网特|维吾尔自由运动|伪大|无界|无界浏览器|无码|无毛|无网界|无线耳机|吾尔开西|吾尔开希|吾尔开希|吾爾開希|無修正|伍凡|武腾兰|武藤兰|武藤蘭|夕樹舞子|西藏流亡政府|西藏论坛|西藏天葬|西单民主墙|下岗工人长恨歌|香港 独立|香港彩|小参考|小电影|小穴|小泽玛莉亚|小泽圆|小澤園|新观察论坛|新华举报|新华内情|新华通论坛|新疆獨立|新疆独立|新疆王|新生网|新唐人|新闻封锁|星崎未來|性爱|性伴侣|性服务|性高潮|性伙伴|性交|性奴|性息|学潮|学潮|学生领袖|学院爆动|学运|学自联|雪山狮子|血腥图片|颜射|艳舞|央视内部晚会|殃视|阳具|摇头丸|夜话紫禁城|夜勤病栋|夜總會|一边一国|一党私利|一党专政|一黨|一夜激情|一夜情|伊東|乙醚|义解|阴唇|阴道|阴蒂|阴户|阴茎|阴毛|阴门|阴囊|淫秽|淫靡|淫水|隐形耳机|罂粟籽|櫻井|影子政府|雍战胜|优昙婆罗|有偿 肾|幼齿|幼交|宇明网|预测答案|元一夜|援交|远程偷拍|造爱|择民|贼民|招聘 打手|招收 小弟|找男|找女|侦探设备|真 善 忍|真善忍|镇压学生|正见网|正义党论坛|郑义|政府无能|支联会|中俄边界新约|中功|中国民主同盟|中国民主正义党|中华养生益智功|中石油国家电网倒数|中特|中正纪念歌|助考|涿州 饲养基地|子女任职名单|自 由 门|自fen|自摸|自杀手册|自杀指南|自由门|自由亚洲|自由运动|自制手枪|足交|作爱|坐交|坐台|做爱|做台|做炸弹)";
        #endregion

        #endregion

    }
}
