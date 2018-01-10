using System;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using System.Drawing.Drawing2D;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;

namespace ClassLib4Net
{
    /// <summary>
    /// QRCode 二维码助手
    /// 熊学浩
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, int scale)
        {
            return Create(data, string.Empty, scale, System.Drawing.Color.Empty, System.Drawing.Color.Empty);
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <param name="savePath">保存文件真实路径</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, int scale, String savePath)
        {
            System.Drawing.Image image = Create(data, string.Empty, scale, savePath, System.Drawing.Color.Empty, System.Drawing.Color.Empty);
            saveImage(image, savePath);
            return image;
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="logoPath">中间的logo图标真实路径</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, String logoPath, int scale)
        {
            return Create(data, logoPath, scale, System.Drawing.Color.Empty, System.Drawing.Color.Empty);
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="logoPath">中间的logo图标真实路径</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <param name="backgroundColor">背景色（可为空）</param>
        /// <param name="foregroundColor">前景色（可为空）</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, String logoPath, int scale, System.Drawing.Color backgroundColor, System.Drawing.Color foregroundColor)
        {
            ThoughtWorks.QRCode.Codec.QRCodeEncoder encoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            encoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = scale > 0 ? scale : 4; //大小(值越大生成的二维码图片像素越高)
                                                         /*
                                                          * 生成的二维码图片的大小是会根据所压缩的信息内容而变化的，网上提供的例子是通过new BufferedImage(139, 139, BufferedImage.TYPE_INT_RGB);来创建图像对象的，默认的情况下图片的大小是139*139，这个大小是比较适合QrcodeVersion为7的情况。
                                                          * 将图片的大小设置到300*300就可以很好的支持QrcodeVersion为20的情况，并且可以正常的解码。
                                                          * QrcodeVersion的范围值是0-40,0的含义是表示压缩的信息量将会根据实际传入值确定，只有最高上限的控制，而且图片的大小将会根据信息量自动缩放。
                                                          * 1-40的范围值，则有固定的信息量上限，而且图片的大小会固定在一个大小上，不会根据信息量的多少而变化。
                                                          */
            encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;//错误效验、错误更正(有4个等级)

            if(backgroundColor != null && !backgroundColor.IsEmpty)
                encoder.QRCodeBackgroundColor = backgroundColor;
            if(foregroundColor != null && !foregroundColor.IsEmpty)
                encoder.QRCodeForegroundColor = foregroundColor;

            System.Drawing.Bitmap _bitmap;
            //动态调整二维码版本号,上限40，过长返回空白图片，编码后字符最大字节长度2953
            bool _success = false;
            while(true)
            {
                try
                {
                    _bitmap = encoder.Encode(data);
                    _success = true;
                    break;
                }
                catch(IndexOutOfRangeException e)
                {
                    if(encoder.QRCodeVersion < 40)
                    {
                        encoder.QRCodeVersion++;
                    }
                    else
                    {
                        _bitmap = new System.Drawing.Bitmap(100, 100);
                        _success = false;
                        break;
                        throw e;
                    }
                }
            }

            if(_bitmap != null && _success)
            {
                if(!string.IsNullOrEmpty(logoPath))
                {
                    System.Drawing.Image image = CombinImage(_bitmap, logoPath);
                    return image;
                }
                else
                    return _bitmap;
            }
            else
                return null;
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="logoPath">中间的logo图标真实路径</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <param name="savePath">保存文件真实路径</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, String logoPath, int scale, String savePath)
        {
            System.Drawing.Image image = Create(data, logoPath, scale);
            saveImage(image, savePath);
            return image;
        }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="data">二维码数据</param>
        /// <param name="logoPath">中间的logo图标真实路径</param>
        /// <param name="scale">二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)</param>
        /// <param name="savePath">保存文件真实路径</param>
        /// <param name="backgroundColor">背景色（可为空）</param>
        /// <param name="foregroundColor">前景色（可为空）</param>
        /// <returns></returns>
        public static System.Drawing.Image Create(String data, String logoPath, int scale, String savePath, System.Drawing.Color backgroundColor, System.Drawing.Color foregroundColor)
        {
            System.Drawing.Image image = Create(data, logoPath, scale, backgroundColor, foregroundColor);
            saveImage(image, savePath);
            return image;
        }

        #region 二维码中间的logo
        /// <summary>    
        /// 调用此函数后使此两种图片合并，类似相册，有个    
        /// 背景图，中间贴自己的目标图片    
        /// </summary>    
        /// <param name="QrCode">二维码源图片</param>    
        /// <param name="logoPath">logo图片真实路径</param>    
        private static System.Drawing.Image CombinImage(System.Drawing.Image QrCode, string logoPath)
        {
            System.Drawing.Image logo = System.Drawing.Image.FromFile(logoPath);        //照片图片    
            int width = (int)(QrCode.Width * 0.20);
            int height = (int)(QrCode.Height * 0.20);

            if(width < 65) { }
            else if(width < 100) { width = 75; }
            else if(width < 150) { width = 95; }
            else if(width < 200) { width = 115; }
            else if(width < 250) { width = 135; }
            else if(width < 300) { width = 155; }
            else { width = 155; }

            if(height < 65) { }
            else if(height < 100) { height = 75; }
            else if(height < 150) { height = 95; }
            else if(height < 200) { height = 115; }
            else if(height < 250) { height = 135; }
            else if(height < 300) { height = 155; }
            else { height = 155; }

            //if (width > 100) width = 100;
            //if (height > 100) height = 100;

            int framelogo = width > 30 ? width / 15 : 2; //中间logo的边框

            if(logo != null && logo.Width > 0 && logo.Height > 0)
            {
                if(logo.Width > (width - framelogo) || logo.Height > (height - framelogo))
                    logo = KiResizeImage(logo, (width - framelogo), (height - framelogo), 0);
                else
                {
                    width = logo.Width + framelogo;
                    height = logo.Height + framelogo;
                }

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(QrCode);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿 
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; //文字抗锯齿

                //g.DrawImage(QrCode, 0, 0, 相框宽, 相框高);
                g.DrawImage(QrCode, 0, 0, QrCode.Width, QrCode.Height);

                //中间的logo四周刷一层白色边框
                //g.FillRectangle(System.Drawing.Brushes.White, QrCode.Width / 2 - width / 2 - framelogo, QrCode.Height / 2 - height / 2 - framelogo, width + framelogo, height + framelogo);

                int radius = framelogo * 2; //圆角半径
                Rectangle rectArc = new Rectangle(QrCode.Width / 2 - width / 2 - framelogo, QrCode.Height / 2 - height / 2 - framelogo, width + framelogo, height + framelogo); // 弧度区域 
                FillRoundRectangle(g, new SolidBrush(ConvertToColor("#FFFFFF")), rectArc, radius);

                //g.DrawImage(logo, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    
                //g.DrawImage(logo, QrCode.Width / 2 - width / 2, QrCode.Height / 2 - height / 2, logo.Width, logo.Height);

                CreateRoundedCorner(logo, ConvertToColor("#FFFFFF"), "", "TopLeft");
                CreateRoundedCorner(logo, ConvertToColor("#FFFFFF"), "", "TopRight");
                CreateRoundedCorner(logo, ConvertToColor("#FFFFFF"), "", "BottomLeft");
                CreateRoundedCorner(logo, ConvertToColor("#FFFFFF"), "", "BottomRight");
                g.DrawImage(logo, QrCode.Width / 2 - width / 2, QrCode.Height / 2 - height / 2, logo.Width, logo.Height);

                g.Dispose();
                logo.Dispose();
                GC.Collect();
            }

            return QrCode;
        }

        /// <summary>    
        /// Resize图片    
        /// </summary>    
        /// <param name="bmp">原始Bitmap</param>    
        /// <param name="newW">新的宽度</param>    
        /// <param name="newH">新的高度</param>    
        /// <param name="Mode">保留着，暂时未用</param>    
        /// <returns>处理以后的图片</returns>    
        private static System.Drawing.Image KiResizeImage(System.Drawing.Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                System.Drawing.Image b = new System.Drawing.Bitmap(newW, newH);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
                // 插值算法的质量    
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, newW, newH), new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 解码二维码
        /// </summary>
        /// <param name="bitmap">二维码图片</param>
        /// <returns></returns>
        public static string Decode(System.Drawing.Bitmap bitmap)
        {
            String decodedString = string.Empty;
            if(bitmap != null)
            {
                //解析二维码信息
                ThoughtWorks.QRCode.Codec.QRCodeDecoder decoder = new ThoughtWorks.QRCode.Codec.QRCodeDecoder();
                decodedString = decoder.decode(new ThoughtWorks.QRCode.Codec.Data.QRCodeBitmapImage(bitmap));
            }
            return decodedString;
        }
        /// <summary>
        /// 解码二维码
        /// </summary>
        /// <param name="QRCodePath">二维码图片路径</param>
        /// <returns></returns>
        public static string Decode(String QRCodePath)
        {
            return Decode(new Bitmap(QRCodePath));
        }

        #region 辅助方法
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="savePath"></param>
        private static void saveImage(System.Drawing.Image image, String savePath)
        {
            if(image != null)
            {
                if(string.IsNullOrEmpty(savePath))
                {
                    savePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                //文件扩展名
                string fileExtension = System.IO.Path.GetExtension(savePath).ToLower();
                if(string.IsNullOrEmpty(fileExtension))
                {
                    if(!System.IO.Directory.Exists(savePath))
                    {
                        System.IO.Directory.CreateDirectory(savePath);
                    }
                    savePath += savePath.TrimEnd(new Char[] { '\\' }) + "\\" + Guid.NewGuid().ToString() + ".png";
                }
                image.Save(savePath);
            }
        }
        #endregion

        #region 圆角矩形边框
        /// <summary>
        /// 将HTML颜色表示形式翻译成System.Drawing.Color结构
        /// 熊学浩
        /// </summary>
        /// <param name="htmlcolor"></param>
        /// <returns></returns>
        public static System.Drawing.Color ConvertToColor(string htmlcolor)
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(htmlcolor);
            System.Drawing.Color newcolor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
            return newcolor;
        }
        /// <summary>
        /// 圆角矩形边框
        /// 熊学浩
        /// </summary>
        /// <param name="g"></param>
        /// <param name="brush"></param>
        /// <param name="rect"></param>
        /// <param name="cornerRadius"></param>
        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using(System.Drawing.Drawing2D.GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }
        internal static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            System.Drawing.Drawing2D.GraphicsPath roundedRect = new System.Drawing.Drawing2D.GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
        #endregion

        #region 图片圆角矩形
        /// <summary>
        /// 图形的四个角实现圆角矩形
        /// 熊学浩
        /// </summary>
        /// <param name="sSrcFilePath"></param>
        /// <param name="color"></param>
        /// <param name="sDstFilePath"></param>
        /// <param name="sCornerLocation"></param>
        /// <returns></returns>
        public static System.Drawing.Image CreateRoundedCorner(string sSrcFilePath, System.Drawing.Color color, string sDstFilePath, string sCornerLocation)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(sSrcFilePath);
            return CreateRoundedCorner(image, color, sDstFilePath, sCornerLocation);
        }
        /// <summary>
        /// 图形的四个角实现圆角矩形
        /// 熊学浩
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        /// <param name="sDstFilePath"></param>
        /// <param name="sCornerLocation"></param>
        /// <returns></returns>
        public static System.Drawing.Image CreateRoundedCorner(System.Drawing.Image image, System.Drawing.Color color, string sDstFilePath, string sCornerLocation)
        {
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            GraphicsPath rectPath = CreateRoundRectanglePath(rect, image.Width / 10, sCornerLocation); //构建圆角外部路径
            Brush b = new SolidBrush(color);//圆角背景白色
            g.DrawPath(new Pen(b), rectPath);
            g.FillPath(b, rectPath);
            g.Dispose();
            if(!string.IsNullOrWhiteSpace(sDstFilePath))
            {
                image.Save(sDstFilePath, ImageFormat.Jpeg);
            }
            //image.Dispose();
            return image;
        }
        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundRectanglePath(Rectangle rect, int radius, string sPosition)
        {
            System.Drawing.Drawing2D.GraphicsPath rectPath = new System.Drawing.Drawing2D.GraphicsPath();
            switch(sPosition)
            {
                case "TopLeft":
                    {
                        rectPath.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90);
                        rectPath.AddLine(rect.Left, rect.Top, rect.Left, rect.Top + radius);
                        break;
                    }

                case "TopRight":
                    {
                        rectPath.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90);
                        rectPath.AddLine(rect.Right, rect.Top, rect.Right - radius, rect.Top);
                        break;
                    }

                case "BottomLeft":
                    {
                        rectPath.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        rectPath.AddLine(rect.Left, rect.Bottom - radius, rect.Left, rect.Bottom);
                        break;
                    }

                case "BottomRight":
                    {
                        rectPath.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        rectPath.AddLine(rect.Right - radius, rect.Bottom, rect.Right, rect.Bottom);
                        break;
                    }

            }
            return rectPath;
        }
        #endregion

        /// <summary>  
        /// 创建二维码  2018/01/10
        /// </summary>  
        /// <param name="QRString">二维码字符串</param>  
        /// <param name="QRCodeEncodeMode">二维码编码(Byte、AlphaNumeric、Numeric)</param>  
        /// <param name="QRCodeScale">二维码尺寸(Version为0时，1：26x26，每加1宽和高各加25</param>  
        /// <param name="QRCodeVersion">二维码密集度0-40</param>  
        /// <param name="QRCodeErrorCorrect">二维码纠错能力(L：7% M：15% Q：25% H：30%)</param>  
        /// <param name="hasLogo">是否有logo(logo尺寸50x50，QRCodeScale>=5，QRCodeErrorCorrect为H级)</param>  
        /// <param name="logoFilePath">logo路径</param>  
        /// <returns></returns>  
        public static Image Create(string QRString, string QRCodeEncodeMode, short QRCodeScale, int QRCodeVersion, string QRCodeErrorCorrect, bool hasLogo, string logoFilePath)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            switch(QRCodeEncodeMode)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = QRCodeScale;
            qrCodeEncoder.QRCodeVersion = QRCodeVersion;

            switch(QRCodeErrorCorrect)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                case "H":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }

            Image image = qrCodeEncoder.Encode(QRString, System.Text.Encoding.UTF8);
            if(hasLogo)
            {
                Image copyImage = Image.FromFile(logoFilePath);
                Graphics g = Graphics.FromImage(image);
                int x = image.Width / 2 - copyImage.Width / 2;
                int y = image.Height / 2 - copyImage.Height / 2;
                g.DrawImage(copyImage, new Rectangle(x, y, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                g.Dispose();

                copyImage.Dispose();
            }
            //image.Dispose();
            return image;
        }


    }
}