/**
* 命名空间: TestConsoleApplication.Image
*
* 功 能： N/A
* 类 名： ImageTest
*
* Ver 变更日期 负责人 当前系统用户名 CLR版本 变更内容
* ───────────────────────────────────
* V0.01 2020/7/4 23:07:25 熊仔其人 xxh 4.0.30319.42000 初版
*
* Copyright (c) 2020 熊仔其人 Corporation. All rights reserved.
*┌─────────────────────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．   │
*│　版权所有：熊仔其人 　　　　　　　　　　　　　　　　　　　　　　　 │
*└─────────────────────────────────────────────────┘
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApplication.Image
{
    /// <summary>
    /// 图片测试
    /// </summary>
    public class ImageTest
    {
        public static void WaterMarkByText()
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;
            string oldImgPath, newImgPath;
            oldImgPath = @"D:\WorkSpace\ClassLib4Net.git\trunk\TestConsoleApplication\Image\imgs\520bacc8d7688ff23a8daaba7e5bdd3.jpg";
            newImgPath = @"D:\WorkSpace\ClassLib4Net.git\trunk\TestConsoleApplication\Image\imgs\520bacc8d7688ff23a8daaba7e5bdd3_1.jpg";

            //decimal Latitude = returnList.Data.Where(x => x.StationID == item.StationID && x.CountryId == c.Country && x.ImageUrl == ii).Select(x => x.Latitude).FirstOrDefault();
            string WaterMark = string.Format("{0}\n\r{1}\n\r{2}\n\r{3}\n\r{4}\n\r", 450000001515, System.Web.HttpUtility.UrlDecode("中华人民共和国", System.Text.Encoding.UTF8), ("经度:" + 110.000045 + ",纬度:" + 39.000123 + ""), System.Web.HttpUtility.UrlDecode("啦啦啦啦啦啦", System.Text.Encoding.UTF8), "yyyy年MM月dd日 HH:mm");
            ClassLib4Net.ImageWaterMarkHelper.WaterMarkByText(oldImgPath, newImgPath, WaterMark, ClassLib4Net.ImageWaterMarkHelper.WaterMarkPosition.RightBottom, fontsize: 12);
        }


    }
}
