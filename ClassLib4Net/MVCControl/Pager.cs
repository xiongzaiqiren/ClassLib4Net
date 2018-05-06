/* 
* @Author: xiongzaiqiren -> https://hbdd.taobao.com/
* @Date:   2013-09-18 14:17:35
* @Last Modified by:   xiongzaiqiren
* @Last Modified time: 2018-05-05 23:30:37
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Web.Mvc
{
    /// <summary>
    /// MVC分页控件属性
    /// </summary>
    public partial class Pager
    {
        public Pager() { }
        /// <summary>
        /// 分页控件属性
        /// </summary>
        /// <param name="CurrentPageIndex">当前页码</param>
        /// <param name="PageSize">页容量</param>
        /// <param name="TotalRecord">总记录数</param>
        /// <param name="urlPrefix">URL前缀(当前URL)</param>
        public Pager(int CurrentPageIndex, int PageSize, int TotalRecord, string urlPrefix)
        {
            this.CurrentPageIndex = CurrentPageIndex;
            this.PageSize = PageSize;
            this.TotalRecord = TotalRecord;
            this.urlPrefix = urlPrefix;
        }

        /// <summary>
        /// 分页Html控件id
        /// </summary>
        public string ID { get; set; } = "AspNetPager";
        /// <summary>
        /// 分页Html控件ClassName
        /// </summary>
        public string ClassName { get; set; } = "AspNetPager";

        /// <summary>
        /// 显示分页控件
        /// </summary>
        public bool ShowPager { get; set; } = true;

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int PageSize { get; set; } = 15;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 显示页码的数目,建议值:3,5,7,9...
        /// </summary>
        public int PageNum { get; set; } = 5;

        private int _TotalPage = 0;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                if(_TotalPage <= 0)
                {
                    _TotalPage = (TotalRecord + PageSize - 1) / PageSize;
                }
                return _TotalPage;
            }
            set { _TotalPage = value; }
        }
        /// <summary>
        /// 显示总页数
        /// </summary>
        public bool ShowTotalPage { get; set; } = true;
        /// <summary>
        /// 显示总记录数
        /// </summary>
        public bool ShowTotalRecord { get; set; } = true;

        /// <summary>
        /// 共
        /// </summary>
        public string TotalText { get; set; } = "共";
        /// <summary>
        /// 页
        /// </summary>
        public string TotalPageText { get; set; } = "页";
        /// <summary>
        /// 条
        /// </summary>
        public string TotalRecordText { get; set; } = "条";

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// PageIndex参数名字
        /// </summary>
        public string PageIndexParamName { get; set; } = "PageIndex";
        /// <summary>
        /// URL前缀(当前URL)
        /// </summary>
        public string urlPrefix { get; set; }

        /// <summary>
        /// 首页
        /// </summary>
        public string homePage { get; set; } = "首页";
        /// <summary>
        /// 上一页
        /// </summary>
        public string PreviousPage { get; set; } = "上一页";
        /// <summary>
        /// 下一页
        /// </summary>
        public string nextPage { get; set; } = "下一页";
        /// <summary>
        /// 尾页
        /// </summary>
        public string endPage { get; set; } = "尾页";

        /// <summary>
        /// 显示“跳转到”
        /// </summary>
        public bool ShowJumpTo { get; set; } = true;
        /// <summary>
        /// 跳转到
        /// </summary>
        public string JumpToText { get; set; } = "跳转到";
        /// <summary>
        /// 确定
        /// </summary>
        public string JumpToBtnText { get; set; } = "确定";

        /// <summary>
        /// 分页按钮组
        /// </summary>
        public IList<PagerButton> PageList { get; set; }

    }

    /// <summary>
    /// MVC分页控件属性
    /// </summary>
    public partial class Pager
    {
        /// <summary>
        /// 分部视图分页模板名称
        /// </summary>
        public string PagerTempName { get; set; }

        /// <summary>
        /// Controller名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// Action名称
        /// </summary>
        public string ActionName { get; set; }

    }

    /// <summary>
    /// 页码按钮
    /// </summary>
    public class PagerButton
    {
        public PagerButton() { }
        /// <summary>
        /// 构造页码属性
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageText">文本</param>
        /// <param name="link">链接</param>
        /// <param name="disabled">失效/禁用</param>
        /// <param name="current">是当前按钮</param>
        public PagerButton(int pageIndex, string pageText, string link = "", bool disabled = false, bool current = false)
        {
            PageIndex = pageIndex;
            PageText = pageText;
            Link = link;
            this.disabled = disabled;
            this.current = current;
        }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string PageText { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 是当前按钮
        /// </summary>
        public bool current { get; set; }
        /// <summary>
        /// 失效/禁用
        /// </summary>
        public bool disabled { get; set; }

        public string outerHTML
        {
            get
            {
                return string.Format("<a {0} class=\"{1}\" href=\"{2}\">{3}</a>"
              , disabled ? "disabled=\"disabled\" readonly=\"readonly\"" : string.Empty
              , current ? "current" : disabled ? "disabled" : string.Empty
              , Link, PageText);
            }
        }
    }
    /// <summary>
    /// MVC分页帮助类
    /// 修改：熊仔其人 2014-09-01
    /// 这段代码，以前我和上帝都能看的懂，现在，只有上帝能看懂
    /// 继续这样改下去，估计连上帝都看不懂了
    /// </summary>
    public static class PageHelper
    {
        /// <summary>
        /// MVC分页控件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pagerId">分页控件Id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="totalSize">总记录数</param>
        /// <param name="pageNum">显示的页码数目</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="actionName">动作名称</param>
        /// <param name="pagerTempName">分部视图分页模板名称</param>
        /// <returns></returns>
        public static MvcHtmlString Pager(this HtmlHelper helper,
            string pagerId, //分页控件Id
            int pageIndex, //当前页
            int pageSize, //每页显示的记录数目
            int totalSize, //总记录
            int pageNum, //显示的页码数目
            string controllerName, //控制器名称
            string actionName, //动作名称
            string pagerTempName //分部视图分页模板名称
        )
        {
            Pager pager = new Pager();
            pager.PagerTempName = pagerTempName;
            pager.ID = pagerId;
            pager.PageSize = pageSize;
            pager.TotalRecord = totalSize;
            pager.CurrentPageIndex = pageIndex;
            pager.TotalPage = (totalSize % pageSize == 0) ? (totalSize / pageSize) : (totalSize / pageSize) + 1;
            pager.PageNum = pageNum;
            pager.ControllerName = controllerName;
            pager.ActionName = actionName;
            if(pager.TotalPage > 0 && pager.TotalPage >= pageIndex)
            {
                pager.ShowPager = true;//显示分页
                List<PagerButton> pageList = new List<PagerButton>();
                int step = pageNum;//偏移量
                int leftPageNum = (pageIndex - pageNum < 1) ? 1 : (pageIndex - pageNum);//左边界
                int rightPageNum = (pageIndex + pageNum > pager.TotalPage) ? pager.TotalPage : (pageIndex + pageNum);//右边界
                int pageCount = rightPageNum - leftPageNum + 1;
                var sourceList = Enumerable.Range(leftPageNum, pageCount);
                pageList.AddRange(sourceList.Select(p => new PagerButton
                {
                    PageIndex = p,
                    PageText = p.ToString()
                }));
                pager.PageList = pageList;
            }
            else
            {
                pager.ShowPager = false;//页数少于一页，则不显示分页
            }
            return helper.Partial(pager.PagerTempName, pager);
        }

        /// <summary>
        /// MVC分页控件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pagerId">分页控件Id</param>
        /// <param name="totalSize">总记录数</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="pageNum">显示的页码数目</param>
        /// <param name="pagerTempName">分部视图分页模板名称</param>
        /// <returns></returns>
        public static MvcHtmlString Pager(this HtmlHelper helper,
            string pagerId, //分页控件Id
            int totalSize, //总记录数
            int pageIndex, //当前页码
            int pageSize = 10, //每页显示10条
            int pageNum = 5, //显示的页码数目
            string pagerTempName = "_PagerTemp" //分页控件模板
        )
        {
            System.Web.Routing.RouteData routeData = helper.ViewContext.RouteData;
            string controllerName = routeData.GetRequiredString("controller");//当前的Controller
            string actionName = routeData.GetRequiredString("action");//当前的action
            return Pager(helper, pagerId, pageIndex, pageSize, totalSize, pageNum, controllerName, actionName, pagerTempName);
        }

        /// <summary>
        /// 生成链接
        /// </summary>
        private static string BuildLink(string urlPrefix, string symbol, string PageIndexParamName, int PageIndex)
        {
            return string.Format("{0}{1}{2}={3}", urlPrefix, symbol, PageIndexParamName, PageIndex);
        }
        /// <summary>
        /// 生成分页按钮
        /// </summary>
        private static void BuildPageList(Pager pager, string symbol)
        {
            if(pager.TotalPage > 1 && pager.PageNum > 1)
            {
                int tempPageIndex;
                if(pager.CurrentPageIndex == 1)
                {
                    tempPageIndex = pager.CurrentPageIndex + pager.PageNum;
                    for(int i = pager.CurrentPageIndex; (i < tempPageIndex && i <= pager.TotalPage); i++)
                    {
                        pager.PageList.Add(new PagerButton(i, i.ToString(), BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, i), false, i == pager.CurrentPageIndex));
                    }

                    if(tempPageIndex < pager.TotalPage)
                    {
                        pager.PageList.Add(new PagerButton(0, "...", "", true, false));
                    }
                }
                else if(pager.CurrentPageIndex == pager.TotalPage)
                {
                    tempPageIndex = (pager.TotalPage - pager.PageNum) + 1;
                    if(tempPageIndex > 1)
                    {
                        pager.PageList.Add(new PagerButton(0, "...", "", true, false));
                    }

                    for(int i = tempPageIndex; i <= pager.TotalPage; i++)
                    {
                        pager.PageList.Add(new PagerButton(i, i.ToString(), BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, i), false, i == pager.CurrentPageIndex));
                    }
                }
                else
                {
                    tempPageIndex = (pager.PageNum + 2 - 1) / 2;
                    int Before = pager.CurrentPageIndex - tempPageIndex, After = pager.CurrentPageIndex + tempPageIndex;
                    if(Before < 0)
                    {
                        After += (0 - Before);
                    }

                    if(After > pager.TotalPage)
                    {
                        Before -= (After - pager.TotalPage);
                    }

                    if(Before > 1)
                    {
                        pager.PageList.Add(new PagerButton(0, "...", "", true, false));
                    }

                    for(int i = (Before <= 1 ? 1 : Before); (i <= After && i <= pager.TotalPage); i++)
                    {
                        pager.PageList.Add(new PagerButton(i, i.ToString(), BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, i), false, i == pager.CurrentPageIndex));
                    }

                    if(After < pager.TotalPage)
                    {
                        pager.PageList.Add(new PagerButton(0, "...", "", true, false));
                    }

                }
            }
        }

        /// <summary>
        /// MVC分页控件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static string PagerHtml(this HtmlHelper helper, Pager pager)
        {
            if(null == pager || !pager.ShowPager) return "";
            if(pager.PageSize <= 0) throw new Exception(string.Format("The value of the {0} is invalid. {0}={1}", nameof(pager.PageSize), pager.PageSize));
            if(string.IsNullOrWhiteSpace(pager.urlPrefix)) throw new Exception(string.Format("The value of the {0} is invalid. {0}={1}. Sample: {0}=Request.Url.ToString()", nameof(pager.urlPrefix), pager.urlPrefix));

            //string reg1 = @"\?PageIndex=\d*&", reg2 = @"\?PageIndex=\d*", reg3 = @"&PageIndex=\d*";
            string reg1 = @"\?" + pager.PageIndexParamName + @"\=\d*&", reg2 = @"\?" + pager.PageIndexParamName + @"\=\d*", reg3 = @"&" + pager.PageIndexParamName + @"\=\d*";
            //设置当前分页符号,如果当前URL有参数则使用&，没有参数则使用?.支持无限地址栏参数
            string symbol = new Regex(reg1).Match(pager.urlPrefix, 0).Success ? "&" : new Regex(reg2).Match(pager.urlPrefix, 0).Success || pager.urlPrefix.IndexOf("?") == -1 ? "?" : "&";
            //将当前已有的PageIndex参数删除
            pager.urlPrefix = new Regex(reg1).Match(pager.urlPrefix, 0).Success ? Regex.Replace(pager.urlPrefix, reg1, "?") : Regex.Replace(Regex.Replace(pager.urlPrefix, reg2, ""), reg3, "");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("<div id=\"{0}\" class=\"{1}\">", pager.ID, pager.ClassName));
            if(pager.ShowTotalPage || pager.ShowTotalRecord)
            {
                sb.AppendFormat("<span class=\"parts\">{0}", pager.TotalText);
                if(pager.ShowTotalRecord)
                {
                    sb.AppendFormat("<font>{0}</font>{1}", pager.TotalRecord, pager.TotalRecordText);
                }
                if(pager.ShowTotalPage)
                {
                    sb.AppendFormat("<font>{0}</font>{1}", pager.TotalPage, pager.TotalPageText);
                }
                sb.AppendLine("</span>");
            }

            if(pager.TotalPage > 1)
            {
                if(pager.CurrentPageIndex <= 1)
                {
                    pager.PageList = new List<PagerButton>();
                    if(!string.IsNullOrWhiteSpace(pager.homePage))
                        pager.PageList.Add(new PagerButton(1, pager.homePage, "", true));
                    if(!string.IsNullOrWhiteSpace(pager.PreviousPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex - 1, pager.PreviousPage, "", true));

                    BuildPageList(pager, symbol);

                    if(!string.IsNullOrWhiteSpace(pager.nextPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex + 1, pager.nextPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.CurrentPageIndex + 1), false));
                    if(!string.IsNullOrWhiteSpace(pager.endPage))
                        pager.PageList.Add(new PagerButton(pager.TotalPage, pager.endPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.TotalPage), false));
                }
                else if(pager.CurrentPageIndex >= pager.TotalPage)
                {
                    pager.PageList = new List<PagerButton>();
                    if(!string.IsNullOrWhiteSpace(pager.homePage))
                        pager.PageList.Add(new PagerButton(1, pager.homePage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, 1), false));
                    if(!string.IsNullOrWhiteSpace(pager.PreviousPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex - 1, pager.PreviousPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.CurrentPageIndex - 1), false));

                    BuildPageList(pager, symbol);

                    if(!string.IsNullOrWhiteSpace(pager.nextPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex + 1, pager.nextPage, "", true));
                    if(!string.IsNullOrWhiteSpace(pager.endPage))
                        pager.PageList.Add(new PagerButton(pager.TotalPage, pager.endPage, "", true));
                }
                else
                {
                    pager.PageList = new List<PagerButton>();
                    if(!string.IsNullOrWhiteSpace(pager.homePage))
                        pager.PageList.Add(new PagerButton(1, pager.homePage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, 1), false));
                    if(!string.IsNullOrWhiteSpace(pager.PreviousPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex - 1, pager.PreviousPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.CurrentPageIndex - 1), false));

                    BuildPageList(pager, symbol);

                    if(!string.IsNullOrWhiteSpace(pager.nextPage))
                        pager.PageList.Add(new PagerButton(pager.CurrentPageIndex + 1, pager.nextPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.CurrentPageIndex + 1), false));
                    if(!string.IsNullOrWhiteSpace(pager.endPage))
                        pager.PageList.Add(new PagerButton(pager.TotalPage, pager.endPage, BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, pager.TotalPage), false));
                }

            }
            else
            {
                pager.PageList = new List<PagerButton>();
                if(!string.IsNullOrWhiteSpace(pager.homePage))
                    pager.PageList.Add(new PagerButton(1, pager.homePage, "", true));
                if(!string.IsNullOrWhiteSpace(pager.PreviousPage))
                    pager.PageList.Add(new PagerButton(pager.CurrentPageIndex - 1, pager.PreviousPage, "", true));

                if(pager.TotalPage == 1)
                    pager.PageList.Add(new PagerButton(1, 1.ToString(), BuildLink(pager.urlPrefix, symbol, pager.PageIndexParamName, 1), false, true));

                if(!string.IsNullOrWhiteSpace(pager.nextPage))
                    pager.PageList.Add(new PagerButton(pager.CurrentPageIndex + 1, pager.nextPage, "", true));
                if(!string.IsNullOrWhiteSpace(pager.endPage))
                    pager.PageList.Add(new PagerButton(pager.TotalPage, pager.endPage, "", true));
            }

            if(pager.PageList != null && pager.PageList.Count > 0)
            {
                foreach(var i in pager.PageList)
                {
                    sb.AppendLine(i.outerHTML);
                }
            }

            if(pager.ShowJumpTo)
                sb.AppendLine("&nbsp;<input type=\"text\" id=\"txtPageIndex\" class=\"fnum\" value=\"" + pager.JumpToText + "\" onfocus=\"if(this.value=='" + pager.JumpToText + "') this.value='';this.style.color='#000'; return true;\" onblur=\"if(this.value=='') this.value='" + pager.JumpToText + "'; this.style.color='#000'; return true;\" /><input type=\"button\" class=\"jump\" value=\"" + pager.JumpToBtnText + "\" onclick=\"var _txtPageIndex = document.getElementById('txtPageIndex').value;if (_txtPageIndex && /[0-9]+/.test(_txtPageIndex)) {window.location.href=document.getElementById('hidCurrentURL').value.replace('#PageIndex#', parseInt(_txtPageIndex));}else{document.getElementById('txtPageIndex').value='';return false;}\" /><input type=\"hidden\" id=\"hidCurrentURL\" value=\"" + string.Format("{0}{1}{2}=#PageIndex#", pager.urlPrefix, symbol, pager.PageIndexParamName) + "\" />");

            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// MVC分页控件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pager">分页参数</param>
        /// <returns></returns>
        public static IHtmlString Pager(this HtmlHelper helper, Pager pager)
        {
            return helper.Raw(helper.PagerHtml(pager));
        }

        /// <summary>
        /// MVC分页控件
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="CurrentPageIndex">当前页码</param>
        /// <param name="PageSize">页容量</param>
        /// <param name="TotalRecord">总记录数</param>
        /// <param name="urlPrefix">URL前缀(当前URL)</param>
        /// <param name="TotalPage">总页数</param>
        /// <param name="PageIndexParamName">PageIndex参数名字</param>
        /// <returns></returns>
        public static IHtmlString Pager(this HtmlHelper helper, int CurrentPageIndex, int PageSize, int TotalRecord, string urlPrefix, int TotalPage = 0, string PageIndexParamName = "PageIndex")
        {
            var pager = new Pager(CurrentPageIndex, PageSize, TotalRecord, urlPrefix) { TotalPage = TotalPage, PageIndexParamName = PageIndexParamName };
            return helper.Raw(helper.PagerHtml(pager));
        }

    }
}