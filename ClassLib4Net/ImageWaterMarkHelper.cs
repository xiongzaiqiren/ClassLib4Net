using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ClassLib4Net
{
    /// <summary>
    /// 图片添加水印
    /// </summary>
    public class ImageWaterMarkHelper
    {
        /// <summary>
        /// 水印在原图上的位置
        /// 图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下
        /// </summary>
        public enum WaterMarkPosition : int
        {
            /// <summary>
            /// 不使用
            /// </summary>
            Disable = 0,
            /// <summary>
            /// 左上
            /// </summary>
            LeftTop = 1,
            /// <summary>
            /// 中上
            /// </summary>
            MiddleTop = 2,
            /// <summary>
            /// 右上
            /// </summary>
            RightTop = 3,
            /// <summary>
            /// 左中
            /// </summary>
            LeftMiddle = 4,
            /// <summary>
            /// 中央
            /// </summary>
            Center = 5,
            /// <summary>
            /// 左中
            /// </summary>
            RightMiddle = 6,
            /// <summary>
            /// 左下
            /// </summary>
            LeftBottom = 7,
            /// <summary>
            /// 中下
            /// </summary>
            MiddleBottom = 8,
            /// <summary>
            /// 右下
            /// </summary>
            RightBottom = 9
        }


        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="oldImgPath">服务器图片物理路径</param>
        /// <param name="newImgPath">保存图片物理路径</param>
        /// <param name="watermarkImgPath">水印图片物理路径</param>
        /// <param name="watermarkPosition">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        public static void WaterMarkByImg(string oldImgPath, string newImgPath, string watermarkImgPath, ImageWaterMarkHelper.WaterMarkPosition watermarkPosition, int quality, int watermarkTransparency)
        {
            if(!File.Exists(oldImgPath))
                return;
            byte[] _ImageBytes = File.ReadAllBytes(oldImgPath);
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));

            if(!File.Exists(watermarkImgPath))
                return;
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkImgPath);

            if(watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if(watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch(watermarkPosition)
            {
                case ImageWaterMarkHelper.WaterMarkPosition.LeftTop:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.MiddleTop:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightTop:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.LeftMiddle:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.Center:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightMiddle:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.LeftBottom:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.MiddleBottom:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightBottom:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach(ImageCodecInfo codec in codecs)
            {
                if(codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if(quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if(ici != null)
                img.Save(newImgPath, ici, encoderParams);
            else
                img.Save(newImgPath);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 根据背景图大小计算文字水印的推荐大小
        /// </summary>
        /// <param name="bgSize"></param>
        /// <param name="defaultEmSize"></param>
        /// <returns></returns>
        private static int emSize(int bgWidth, int bgHeight, ImageWaterMarkHelper.WaterMarkPosition watermarkPosition, int defaultEmSize = 12)
        {
            //暂时只支持横排
            if(bgWidth <= 500)
                return defaultEmSize;
            else if(bgWidth <= 1024)
                return 24;
            else if(bgWidth <= 2048)
                return 48;
            else if(bgWidth <= 3024)
                return 64;
            else
                return 80;
        }

        /// <summary>
        /// 文字水印
        /// 示例：ClassLib4Net.ImageWaterMarkHelper.WaterMarkByText(r1, r2, "116.2555160522461,40.08475112915039\n\r北京市海淀区友谊路\n\r" + DateTime.Now.ToString("yyyy年MM月dd日"), 9, fontsize: 25);
        /// </summary>
        /// <param name="oldImgPath">服务器图片物理路径</param>
        /// <param name="newImgPath">保存图片物理路径</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkPosition">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontname">字体</param>
        /// <param name="fontsize">字体大小</param>
        public static void WaterMarkByText(string oldImgPath, string newImgPath, string watermarkText, ImageWaterMarkHelper.WaterMarkPosition watermarkPosition, int quality = 72, string fontname = "宋体", int fontsize = 12)
        {
            if(!File.Exists(oldImgPath))
                return;
            byte[] _ImageBytes = File.ReadAllBytes(oldImgPath);
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));
            fontsize = emSize(img.Width, img.Height, watermarkPosition, fontsize);

            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch(watermarkPosition)
            {
                case ImageWaterMarkHelper.WaterMarkPosition.LeftTop:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.MiddleTop:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightTop:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.LeftMiddle:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.Center:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightMiddle:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.LeftBottom:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.MiddleBottom:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case ImageWaterMarkHelper.WaterMarkPosition.RightBottom:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos + 3, ypos + 3);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach(ImageCodecInfo codec in codecs)
            {
                if(codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if(quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if(ici != null)
                img.Save(newImgPath, ici, encoderParams);
            else
                img.Save(newImgPath);

            g.Dispose();
            img.Dispose();
        }



    }


}
