using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ClassLib4Net
{
	/// <summary>
	/// 图像处理助手
	/// </summary>
	public class ImageHelper
	{
		/// <summary>
		/// 获取Image的标准ImageFormat
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static ImageFormat Format(Image image)
		{
			ImageFormat format = image.RawFormat;
			if (format.Equals(ImageFormat.Jpeg))
				return ImageFormat.Jpeg;
			else if (format.Equals(ImageFormat.Png))
				return ImageFormat.Png;
			else if (format.Equals(ImageFormat.Gif))
				return ImageFormat.Gif;
			else if (format.Equals(ImageFormat.Icon))
				return ImageFormat.Icon;
			else if (format.Equals(ImageFormat.Bmp))
				return ImageFormat.Bmp;
			else if (format.Equals(ImageFormat.Emf))
				return ImageFormat.Emf;
			else if (format.Equals(ImageFormat.Exif))
				return ImageFormat.Exif;
			else if (format.Equals(ImageFormat.MemoryBmp))
				return ImageFormat.MemoryBmp;
			else if (format.Equals(ImageFormat.Tiff))
				return ImageFormat.Tiff;
			else if (format.Equals(ImageFormat.Wmf))
				return ImageFormat.Wmf;
			else
				return format;
		}

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        #region Image与byte数组的转换
        /// <summary>
        /// Image转换成byte[]数组
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
		{
			ImageFormat format = image.RawFormat;
			using (MemoryStream ms = new MemoryStream())
			{
				if (format.Equals(ImageFormat.Jpeg))
				{
					image.Save(ms, ImageFormat.Jpeg);
				}
				else if (format.Equals(ImageFormat.Png))
				{
					image.Save(ms, ImageFormat.Png);
				}
				else if (format.Equals(ImageFormat.Bmp))
				{
					image.Save(ms, ImageFormat.Bmp);
				}
				else if (format.Equals(ImageFormat.Gif))
				{
					image.Save(ms, ImageFormat.Gif);
				}
				else if (format.Equals(ImageFormat.Icon))
				{
					image.Save(ms, ImageFormat.Icon);
				}
				else if (format.Equals(ImageFormat.Emf))
				{
					image.Save(ms, ImageFormat.Emf);
				}
				else if (format.Equals(ImageFormat.Exif))
				{
					image.Save(ms, ImageFormat.Exif);
				}
				else if (format.Equals(ImageFormat.MemoryBmp))
				{
					image.Save(ms, ImageFormat.MemoryBmp);
				}
				else if (format.Equals(ImageFormat.Tiff))
				{
					image.Save(ms, ImageFormat.Tiff);
				}
				else if (format.Equals(ImageFormat.Wmf))
				{
					image.Save(ms, ImageFormat.Wmf);
				}

				byte[] buffer = new byte[ms.Length];
				//Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
				ms.Seek(0, SeekOrigin.Begin);
				ms.Read(buffer, 0, buffer.Length);
				return buffer;
			}
		}

		/// <summary>
		/// byte[]数组转换成图片
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static Image BytesToImage(byte[] bytes)
		{
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				ms.Position = 0;
				Image img = Image.FromStream(ms);
				ms.Close();
				return img;
			}
		}
        #endregion

        #region 生成缩略图
        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="sourceImg">原图片路径</param>
        /// <param name="desImg">裁剪后图片路径</param>
        /// <param name="left">X（单位：像素）</param>
        /// <param name="top">Y（单位：像素）</param>
        /// <param name="width">宽（单位：像素）</param>
        /// <param name="height">高（单位：像素）</param>
        public static void CutImage(string sourceImg, string desImg, int left, int top, int width, int height)
        {
            using (System.Drawing.Image img = System.Drawing.Bitmap.FromFile(sourceImg))
            {
                using (System.Drawing.Image imgToSave = new System.Drawing.Bitmap(width, height))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(imgToSave))
                    {
                        RectangleF sourceRect = new RectangleF(left, top, width, height);
                        RectangleF destinationRect = new RectangleF(0, 0, width, height);
                        g.DrawImage(img,
                                    destinationRect,
                                    sourceRect,
                                    GraphicsUnit.Pixel
                                    );
                        g.Save();
                        System.Drawing.Imaging.ImageFormat format = null != img.RawFormat ? img.RawFormat : System.Drawing.Imaging.ImageFormat.Jpeg;
                        imgToSave.Save(desImg, format);
                        g.Dispose();
                    }
                    imgToSave.Dispose();
                }
                img.Dispose();
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式（WH：指定高宽缩放（可能变形）；W：指定宽，高按比例；H：指定高，宽按比例；Cut：指定高宽裁减（不变形）；Auto：按照最大边长，另一边按比例）默认Auto方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
		{
			ThumbnailMode _mode;
			switch (mode)
			{
				case "WH": //指定高宽缩放（可能变形）  
					_mode = ThumbnailMode.WH;
					break;
				case "W": //指定宽，高按比例    
					_mode = ThumbnailMode.Width;
					break;
				case "H": //指定高，宽按比例
					_mode = ThumbnailMode.Height;
					break;
				case "Cut": //指定高宽裁减（不变形） 
					_mode = ThumbnailMode.Cut;
					break;
				default: //按照最大边长，另一边按比例
					_mode = ThumbnailMode.Auto;
					break;
			}
			MakeThumbnail(originalImagePath, thumbnailPath, width, height, _mode);
		}

		/// <summary>
		/// 生成缩略图
		/// </summary>
		/// <param name="originalImagePath">源图路径（物理路径）</param>
		/// <param name="thumbnailPath">缩略图路径（物理路径）</param>
		/// <param name="width">缩略图宽度</param>
		/// <param name="height">缩略图高度</param>
		/// <param name="mode">生成缩略图的方式（WH：指定高宽缩放（可能变形）；W：指定宽，高按比例；H：指定高，宽按比例；Cut：指定高宽裁减（不变形）；Auto：按照最大边长，另一边按比例）默认Auto方式</param>
		public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, ThumbnailMode mode)
		{
			System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
			int towidth = width;
			int toheight = height;
			int x = 0;
			int y = 0;
			int ow = originalImage.Width;
			int oh = originalImage.Height;
			switch (mode)
			{
				case ThumbnailMode.WH: //指定高宽缩放（可能变形）                
					break;
				case ThumbnailMode.Width: //指定宽，高按比例               
					toheight = originalImage.Height * width / originalImage.Width;
					break;
				case ThumbnailMode.Height: //指定高，宽按比例
					towidth = originalImage.Width * height / originalImage.Height;
					break;
				case ThumbnailMode.Cut: //指定高宽裁减（不变形）                
					if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
					{
						oh = originalImage.Height;
						ow = originalImage.Height * towidth / toheight;
						y = 0;
						x = (originalImage.Width - ow) / 2;
					}
					else
					{
						ow = originalImage.Width;
						oh = originalImage.Width * height / towidth;
						x = 0;
						y = (originalImage.Height - oh) / 2;
					}
					break;
				case ThumbnailMode.Auto: //按照最大边长，另一边按比例
					if (originalImage.Width >= originalImage.Height)
						toheight = originalImage.Height * width / originalImage.Width; //指定宽，高按比例 
					else
						towidth = originalImage.Width * height / originalImage.Height; //指定高，宽按比例
					break;
				default:
					break;
			}
			//新建一个bmp图片
			System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
			//新建一个画板
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
			//设置高质量插值法
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
			//设置高质量,低速度呈现平滑程度
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			//清空画布并以透明背景色填充
			g.Clear(System.Drawing.Color.Transparent);
			//在指定位置并且按指定大小绘制原图片的指定部分
			g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
				new System.Drawing.Rectangle(x, y, ow, oh),
				System.Drawing.GraphicsUnit.Pixel);
			try
			{
				//以jpg格式保存缩略图
				bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
			}
			catch (System.Exception e)
			{
				throw e;
			}
			finally
			{
				originalImage.Dispose();
				bitmap.Dispose();
				g.Dispose();
			}
		}

		/// <summary>
		/// 生成缩略图的方式
		/// </summary>
		public enum ThumbnailMode
		{
			/// <summary>
			/// 指定高宽缩放（可能变形）
			/// </summary>
			WH = 0,
			/// <summary>
			/// 指定宽，高按比例
			/// </summary>
			Width = 1,
			/// <summary>
			/// 指定高，宽按比例
			/// </summary>
			Height = 2,
			/// <summary>
			/// 指定高宽裁减（不变形）
			/// </summary>
			Cut = 3,
			/// <summary>
			/// 按照最大边长，另一边按比例
			/// </summary>
			Auto = 4
		}
        #endregion

        #region 生成缩略图（截图方式）
        /// <summary>
        /// 生成缩略图（截图方式）
        /// </summary>
        /// <param name="originalPath">原图像（物理路径）</param>
        /// <param name="cutX">裁剪区原图像左上角X轴坐标</param>
        /// <param name="cutY">裁剪区原图像左上角Y轴坐标</param>
        /// <param name="cutWidth">裁剪区原图像宽度</param>
        /// <param name="cutHeight">裁剪区原图像高度</param>
        /// <param name="destPath">目标图像（物理路径）</param>
        /// <param name="destWidth">目标图像宽度</param>
        /// <param name="destHeight">目标图像高度</param>
        /// <param name="quality">压缩质量(1-100数字越小压缩率越高，建议75)</param>
        public static void MakeThumbnail(string originalPath, float cutX, float cutY, float cutWidth, float cutHeight, string destPath, int destWidth, int destHeight, long quality = 75)
        {
            using (Image originalImg = Image.FromFile(originalPath))
            {
                using (Bitmap destBitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb))
                {
                    destBitmap.MakeTransparent(destBitmap.GetPixel(0, 0));
                    destBitmap.SetResolution(originalImg.HorizontalResolution > 72 ? originalImg.HorizontalResolution : 72, originalImg.VerticalResolution > 72 ? originalImg.VerticalResolution : 72);
                    using (Graphics g = Graphics.FromImage(destBitmap))
                    {
                        if (originalImg.RawFormat.Equals(ImageFormat.Jpeg) || originalImg.RawFormat.Equals(ImageFormat.Png) || originalImg.RawFormat.Equals(ImageFormat.Bmp))
                        {
                            g.Clear(Color.Transparent);
                        }
                        if (originalImg.RawFormat.Equals(ImageFormat.Gif))
                        {
                            g.Clear(Color.White);
                        }

                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        using (var attribute = new System.Drawing.Imaging.ImageAttributes())
                        {
                            attribute.SetWrapMode(WrapMode.TileFlipXY);
                            g.DrawImage(originalImg, new Rectangle(0, 0, destWidth, destHeight), cutX, cutY, cutWidth, cutHeight, GraphicsUnit.Pixel, attribute);
                        }
                        g.Dispose();
                    }

                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    switch (System.IO.Path.GetExtension(destPath).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            break;
                        case ".png":
                            myImageCodecInfo = GetEncoderInfo("image/png");
                            break;
                        case ".gif":
                            myImageCodecInfo = GetEncoderInfo("image/gif");
                            break;
                        case ".bmp":
                            myImageCodecInfo = GetEncoderInfo("image/bmp");
                            break;
                    }

                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality); //压缩质量(数字越小压缩率越高) 1-100
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    destBitmap.Save(destPath, myImageCodecInfo, myEncoderParameters);

                    destBitmap.Dispose();
                }
                originalImg.Dispose();
            }
        }

        /// <summary>
        /// 特别适合Wap移动端页面裁剪控件生成缩略图（截图方式）
        /// </summary>
        /// <param name="originalPath">原图像（物理路径）</param>
        /// <param name="cutX">裁剪区原图像左上角X轴坐标</param>
        /// <param name="cutY">裁剪区原图像左上角Y轴坐标</param>
        /// <param name="cutWidth">裁剪区原图像宽度</param>
        /// <param name="cutHeight">裁剪区原图像高度</param>
        /// <param name="cutNaturalWidth">裁剪控件原图像宽度</param>
        /// <param name="cutNaturalHeight">裁剪控件原图像高度</param>
        /// <param name="destPath">目标图像（物理路径）</param>
        /// <param name="destWidth">目标图像宽度</param>
        /// <param name="destHeight">目标图像高度</param>
        /// <param name="quality">压缩质量(1-100数字越小压缩率越高，建议75)</param>
        public static void MakeThumbnail(string originalPath, float cutX, float cutY, float cutWidth, float cutHeight, int cutNaturalWidth, int cutNaturalHeight, string destPath, int destWidth, int destHeight, long quality = 75)
        {
            using (Image originalImg = Image.FromFile(originalPath))
            {
                using (Bitmap destBitmap = new Bitmap(destWidth, destHeight, originalImg.PixelFormat)) // PixelFormat.Format24bppRgb))
                {
                    destBitmap.MakeTransparent(destBitmap.GetPixel(0, 0));
                    destBitmap.SetResolution(originalImg.HorizontalResolution > 72 ? originalImg.HorizontalResolution : 72, originalImg.VerticalResolution > 72 ? originalImg.VerticalResolution : 72);
                    using (Graphics g = Graphics.FromImage(destBitmap))
                    {
                        if (originalImg.RawFormat.Equals(ImageFormat.Jpeg) || originalImg.RawFormat.Equals(ImageFormat.Png) || originalImg.RawFormat.Equals(ImageFormat.Bmp))
                        {
                            g.Clear(Color.Transparent);
                        }
                        if (originalImg.RawFormat.Equals(ImageFormat.Gif))
                        {
                            g.Clear(Color.White);
                        }

                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        using (var attribute = new System.Drawing.Imaging.ImageAttributes())
                        {
                            attribute.SetWrapMode(WrapMode.TileFlipXY);
                            float xRatio = cutNaturalWidth > 0 ? originalImg.Width / cutNaturalWidth : 1f;
                            float yRatio = cutNaturalHeight > 0 ? originalImg.Height / cutNaturalHeight : 1f;

                            g.DrawImage(originalImg, new Rectangle(0, 0, destWidth, destHeight), cutX * xRatio, cutY * yRatio, cutWidth * xRatio, cutHeight * yRatio, GraphicsUnit.Pixel, attribute);
                        }
                        g.Dispose();
                    }

                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    switch (System.IO.Path.GetExtension(destPath).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            break;
                        case ".png":
                            myImageCodecInfo = GetEncoderInfo("image/png");
                            break;
                        case ".gif":
                            myImageCodecInfo = GetEncoderInfo("image/gif");
                            break;
                        case ".bmp":
                            myImageCodecInfo = GetEncoderInfo("image/bmp");
                            break;
                    }

                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality); //压缩质量(数字越小压缩率越高) 1-100
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    destBitmap.Save(destPath, myImageCodecInfo, myEncoderParameters);

                    destBitmap.Dispose();
                }
                originalImg.Dispose();
            }
        }

        //根据长宽自适应 按原图比例缩放 
        private static Size GetThumbnailSize(System.Drawing.Image original, int desiredWidth, int desiredHeight)
		{
			var widthScale = (double)desiredWidth / original.Width;
			var heightScale = (double)desiredHeight / original.Height;
			var scale = widthScale < heightScale ? widthScale : heightScale;
			return new Size
			{
				Width = (int)(scale * original.Width),
				Height = (int)(scale * original.Height)
			};
		}
		#endregion

		#region 生成缩略图

		/// <summary>
		/// 按照尺寸和压缩质量生成缩略图。需要指定缩略图绝对路径。使用默认的压缩质量（默认75）。
		/// </summary>
		/// <param name="srcFileFullName">源图片url</param>
		/// <param name="disFileFullName">目的图片url</param>
		/// <param name="smallSize">缩略图尺寸</param> 
		/// <returns>生成成功返回true，失败返回false。</returns>
		public static bool MakeSmallImage(string srcFileFullName, string disFileFullName, Size smallSize)
		{
			Image srcImg = null;

			try
			{
				srcImg = Image.FromFile(srcFileFullName);
			}
			catch
			{
				return false;
			}

			return MakeSmallImage(srcImg, disFileFullName, smallSize, 75);
		}

		/// <summary>
		/// 按照尺寸和压缩质量生成缩略图。需要指定缩略图绝对路径。
		/// </summary>
		/// <param name="srcFileFullName">源图片url</param>
		/// <param name="disFileFullName">目的图片url</param>
		/// <param name="smallSize">缩略图尺寸</param>
		/// <param name="quality">缩略图质量。赋值范围0－100。一般75能兼顾质量和图片大小。</param>
		/// <returns>生成成功返回true，失败返回false。</returns>
		public static bool MakeSmallImage(string srcFileFullName, string disFileFullName, Size smallSize, int quality)
		{
			Image srcImg = null;

			try
			{
				srcImg = Image.FromFile(srcFileFullName);
			}
			catch
			{
				return false;
			}

			return MakeSmallImage(srcImg, disFileFullName, smallSize, quality);
		}

		/// <summary>
		/// 按照限制的边长和压缩质量生成缩略图。需要指定缩略图绝对路径。使用默认的压缩质量。
		/// 如果图片的边长不大于指定的最大边长，直接拷贝原图作为缩略图。
		/// </summary>
		/// <param name="srcFileFullName">源图片url</param>
		/// <param name="disFileFullName">目的图片url</param>
		/// <param name="maxSideLength">最大边长</param>
		/// <param name="limitMode">最大边长限制模式：按照宽度，高度或最大边长。</param>       
		/// <returns>生成成功返回true，失败返回false。</returns>
		public static bool MakeSmallImage(string srcFileFullName, string disFileFullName, int maxSideLength, LimitSideMode limitMode)
		{
			Image srcImg = null;

			try
			{
				srcImg = Image.FromFile(srcFileFullName);
			}
			catch
			{
				return false;
			}
			decimal compressRate = 1;

			//按照压缩模式计算压缩比例
			switch (limitMode)
			{
				case LimitSideMode.Width:

					if (srcImg.Width <= maxSideLength)
					{
						File.Copy(srcFileFullName, disFileFullName);
						return true;
					}

					compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Width;

					break;

				case LimitSideMode.Height:

					if (srcImg.Height <= maxSideLength)
					{
						File.Copy(srcFileFullName, disFileFullName);
						return true;
					}

					compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Height;

					break;

				case LimitSideMode.Auto:
					if (srcImg.Width >= srcImg.Height)
					{
						if (srcImg.Width <= maxSideLength)
						{
							File.Copy(srcFileFullName, disFileFullName);
							return true;
						}
						compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Width;
					}
					else
					{
						if (srcImg.Height <= maxSideLength)
						{
							File.Copy(srcFileFullName, disFileFullName);
							return true;
						}
						compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Height;
					}
					break;
			}

			//计算缩略图大小
			Size smallSize = new Size(Convert.ToInt32(srcImg.Width * compressRate), Convert.ToInt32(srcImg.Height * compressRate));

			return MakeSmallImage(srcImg, disFileFullName, smallSize, 75);

		}

		/// <summary>
		/// 按照限制的边长和压缩质量生成缩略图。需要指定缩略图绝对路径。
		/// 如果图片的边长不大于指定的最大边长，直接拷贝原图作为缩略图。
		/// </summary>
		/// <param name="srcFileFullName">源图片url</param>
		/// <param name="disFileFullName">目的图片url</param>
		/// <param name="maxSideLength">最大边长</param>
		/// <param name="limitMode">最大边长限制模式：按照宽度，高度或最大边长。</param>
		/// <param name="quality">缩略图质量，赋值范围0－100，一般75能兼顾质量和图片大小。</param>
		/// <returns>生成成功返回true，失败返回false。</returns>
		public static bool MakeSmallImage(string srcFileFullName, string disFileFullName, int maxSideLength, LimitSideMode limitMode, int quality)
		{
			Image srcImg = null;

			try
			{
				srcImg = Image.FromFile(srcFileFullName);
			}
			catch
			{
				return false;
			}
			decimal compressRate = 1;

			//按照压缩模式计算压缩比例
			switch (limitMode)
			{
				case LimitSideMode.Width:

					if (srcImg.Width <= maxSideLength)
					{
						File.Copy(srcFileFullName, disFileFullName);
						return true;
					}

					compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Width;

					break;

				case LimitSideMode.Height:

					if (srcImg.Height <= maxSideLength)
					{
						File.Copy(srcFileFullName, disFileFullName);
						return true;
					}

					compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Height;

					break;

				case LimitSideMode.Auto:
					if (srcImg.Width >= srcImg.Height)
					{
						if (srcImg.Width <= maxSideLength)
						{
							File.Copy(srcFileFullName, disFileFullName);
							return true;
						}
						compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Width;
					}
					else
					{
						if (srcImg.Height <= maxSideLength)
						{
							File.Copy(srcFileFullName, disFileFullName);
							return true;
						}
						compressRate = Convert.ToDecimal(maxSideLength) / srcImg.Height;
					}
					break;
			}

			//计算缩略图大小
			Size smallSize = new Size(Convert.ToInt32(srcImg.Width * compressRate), Convert.ToInt32(srcImg.Height * compressRate));

			return MakeSmallImage(srcImg, disFileFullName, smallSize, quality);

		}

        /// <summary>
        /// 按照尺寸和压缩质量生成缩略图
        /// </summary>
        /// <param name="srcImage">源图片</param>
        /// <param name="disFileFullName">目的图片url</param>
        /// <param name="smallSize">缩略图尺寸</param>
        /// <param name="quality">缩略图质量，赋值范围0－100，一般75能兼顾质量和图片大小。</param>
        /// <returns></returns>
        private static bool MakeSmallImage(Image srcImage, string disFileFullName, Size smallSize, int quality)
		{
			bool rtn = true;


			Bitmap outBmp = null;

			try
			{
				ImageFormat srcFormat = srcImage.RawFormat;
				outBmp = new Bitmap(srcImage, smallSize);
				Graphics g = Graphics.FromImage(outBmp);

				// 设置画布的描绘质量
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(srcImage, new Rectangle(0, 0, smallSize.Width, smallSize.Height), 0, 0, srcImage.Width, srcImage.Height, GraphicsUnit.Pixel);
				g.Dispose();

				// 以下代码为保存图片时,设置压缩质量
				EncoderParameters encoderParams = new EncoderParameters();
				long[] qualityValue = new long[1];
				qualityValue[0] = quality;
				EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityValue);
				encoderParams.Param[0] = encoderParam;

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                switch (System.IO.Path.GetExtension(disFileFullName).ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        break;
                    case ".png":
                        myImageCodecInfo = GetEncoderInfo("image/png");
                        break;
                    case ".gif":
                        myImageCodecInfo = GetEncoderInfo("image/gif");
                        break;
                    case ".bmp":
                        myImageCodecInfo = GetEncoderInfo("image/bmp");
                        break;
                }

                outBmp.Save(disFileFullName, myImageCodecInfo, encoderParams);
            }
			catch
			{
				rtn = false;
			}
			finally
			{
				if (srcImage != null)
				{
					srcImage.Dispose();
				}

				if (outBmp != null)
				{
					outBmp.Dispose();
				}
			}

			return rtn;
		}

		/// <summary>
		/// 生成缩略图时的最大边长限制模式：按照宽度，高度或最大边长。
		/// </summary>
		public enum LimitSideMode
		{
			/// <summary>
			/// 宽度固定，高度按比例
			/// </summary>
			Width = 0,

			/// <summary>
			/// 高度固定，宽度按比例
			/// </summary>
			Height = 1,

			/// <summary>
			/// 按照最大边长，另一边按比例
			/// </summary>
			Auto = 2
		}

        #endregion

        #region 旋转图像
        /// <summary>
        /// 任意角度旋转（待测试）
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap Rotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            try
            {
                g.Clear(bkColor);
                g.DrawImageUnscaled(bmp, 1, 1);
                g.Dispose();

                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(new RectangleF(0f, 0f, w, h));
                Matrix mtrx = new Matrix();
                mtrx.Rotate(angle);
                RectangleF rct = path.GetBounds(mtrx);

                Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
                g = Graphics.FromImage(dst);
                g.Clear(bkColor);
                g.TranslateTransform(-rct.X, -rct.Y);
                g.RotateTransform(angle);
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImageUnscaled(tmp, 0, 0);

                return dst;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                g.Dispose();
                tmp.Dispose();
            }
        }

        /// <summary>
        /// 根据角度旋转图标（待测试）
        /// </summary>
        /// <param name="img"></param>
        public Image RotateImg(Image img, float angle)
        {
            //通过Png图片设置图片透明，修改旋转图片变黑问题。
            int width = img.Width;
            int height = img.Height;
            //角度
            Matrix mtrx = new Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            //得到旋转后的矩形
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            //生成目标位图
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);
            Point center = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(img, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage;
        }
        /// <summary>
        /// 根据角度旋转图标第二种方法（待测试）
        /// </summary>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Image RotateImg2(Image b, float angle)
        {
            angle = angle % 360;            //弧度转换
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            //原图的宽和高
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));
            //目标位图
            Image dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((W - w) / 2, (H - h) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
        }
        #endregion

        #region 图片压缩(降低质量)Compress

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitmap, Stream destStream, long level)
        {
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with 给定的 quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;
            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitMap">传入的Bitmap对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitMap, string destFile, long level)
        {
            Stream s = new FileStream(destFile, FileMode.Create);
            Compress(srcBitMap, s, level);
            s.Close();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcStream">传入的Stream对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Stream srcStream, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcStream);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcImg">传入的Image对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Image srcImg, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcImg);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">待压缩的BMP文件名</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(string srcFile, string destFile, long level)
        {
            // Create a Bitmap object based on a BMP file.
            Bitmap bm = new Bitmap(srcFile);
            Compress(bm, destFile, level);
            bm.Dispose();
        }

        #endregion 图片压缩(降低质量)

        #region C#中图片与BASE64码互相转换 
        /// <summary>
        /// 将图片转换为Base64字符串
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ToBase64(Image img)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binFormatter.Serialize(memStream, img);
            byte[] bytes = memStream.GetBuffer();
            memStream.Close();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将Base64字符串转换为图片
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static Image ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Image img = (Image)binFormatter.Deserialize(memStream);
            memStream.Close();
            return img;
        }

        #endregion

        #region 给图片加水印
        /// <summary>
        /// 给图片加水印
        /// 示例： Mark(@"D:\a.jpg", @"D:\logo.jpg", @"D:\b.jpg", 10, 10, 40);
        /// </summary>
        /// <param name="srcPicFileFullName">源文件物理路径</param>
        /// <param name="markPicFileFullName">水印图物理路径</param>
        /// <param name="outPicFileFullName">输出文件物理路径</param>
        /// <param name="rightSpace">水印图在全图的右边距</param>
        /// <param name="bottomSpace">水印图在全图的下边距</param>
        /// <param name="lucencyPercent">透明度 0:全透明 100:不透明</param>
        /// <returns>成功返回true 否则返回false</returns>
        public static bool Mark(string srcPicFileFullName, string markPicFileFullName, string outPicFileFullName, int rightSpace, int bottomSpace, int lucencyPercent)
		{
			bool result = false;
			Image srcImage = null;
			Image maskImage = null;
			Graphics g = null;

			try
			{
				//建立图形对象
				srcImage = Image.FromFile(srcPicFileFullName);
				maskImage = Image.FromFile(markPicFileFullName);
				g = Graphics.FromImage(srcImage);
				//获取要绘制图形坐标
				int x = srcImage.Width - rightSpace - maskImage.Width;
				int y = srcImage.Height - bottomSpace - maskImage.Height;
				//设置颜色矩阵
				float[][] matrixItems ={
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, (float)lucencyPercent/100f, 0},
                    new float[] {0, 0, 0, 0, 1}
                };
				ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
				ImageAttributes imgAttr = new ImageAttributes();
				imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				//绘制阴影图像
				g.DrawImage(maskImage, new Rectangle(x, y, maskImage.Width, maskImage.Height),
					0, 0, maskImage.Width, maskImage.Height, GraphicsUnit.Pixel, imgAttr);

				//保存文件
				string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };
				FileInfo file = new FileInfo(srcPicFileFullName);

				ImageFormat imageType = ImageFormat.Gif;
				switch (file.Extension.ToLower())
				{
					case ".jpg":
						imageType = ImageFormat.Jpeg;
						break;
					case ".gif":
						imageType = ImageFormat.Gif;
						break;
					case ".png":
						imageType = ImageFormat.Png;
						break;
					case ".bmp":
						imageType = ImageFormat.Bmp;
						break;
					case ".tif":
						imageType = ImageFormat.Tiff;
						break;
					case ".wmf":
						imageType = ImageFormat.Wmf;
						break;
					case ".ico":
						imageType = ImageFormat.Icon;
						break;
					default:
						break;
				}

				MemoryStream ms = new MemoryStream();
				srcImage.Save(ms, imageType);
				byte[] imgData = ms.ToArray();
				srcImage.Dispose();
				maskImage.Dispose();
				g.Dispose();

				FileStream fs = null;

				fs = new FileStream(outPicFileFullName, FileMode.Create, FileAccess.Write);

				if (fs != null)
				{
					fs.Write(imgData, 0, imgData.Length);
					fs.Close();
				}

				result = true;
			}
			catch
			{
				result = false;
			}

			finally
			{
				try
				{
					maskImage.Dispose();
					srcImage.Dispose();
					g.Dispose();
				}
				catch { }
			}

			return result;
		}
		/// <summary>
		/// 给图片加水印
		/// 示例：Mark(@"D:\a.jpg", @"D:\logo.jpg", @"D:\b.jpg", 3, 4, 20, 10); 
		/// </summary>
		/// <param name="srcPicFileFullName">源文件物理路径</param>
		/// <param name="markPicFileFullName">水印图物理路径</param>
		/// <param name="outPicFileFullName">输出文件物理路径</param>
		/// <param name="rightSpacePercent">水印图右边距占全图宽度的百分比</param>
		/// <param name="bottomSpacePercnet">水印图下边距占全图高度的百分比</param>
		/// <param name="maskWidthSizePercent">水印图宽度占全图宽度的百分比</param>
		/// <param name="lucencyPercent">透明度 0:全透明 100:不透明</param>
		/// <returns>成功返回true 否则返回false</returns>
		public static bool Mark(string srcPicFileFullName, string markPicFileFullName, string outPicFileFullName, int rightSpacePercent, int bottomSpacePercnet, int maskWidthSizePercent, int lucencyPercent)
		{
			bool result = false;
			Image srcImage = null;
			Image maskImage = null;
			Graphics g = null;

			try
			{
				//建立图形对象
				srcImage = Image.FromFile(srcPicFileFullName);
				maskImage = Image.FromFile(markPicFileFullName);
				g = Graphics.FromImage(srcImage);

				int rectWidth = Convert.ToInt32(srcImage.Width * ((float)maskWidthSizePercent / 100f));
				int rectHeight = Convert.ToInt32(maskImage.Height * ((float)rectWidth / maskImage.Width));
				int rightSpace = Convert.ToInt32(srcImage.Width * ((float)rightSpacePercent / 100f)) + rectWidth;
				int bottomSpace = Convert.ToInt32(srcImage.Height * ((float)bottomSpacePercnet / 100f)) + rectHeight;
				//获取要绘制图形坐标
				int x = srcImage.Width - rightSpace;
				int y = srcImage.Height - bottomSpace;
				//设置颜色矩阵
				float[][] matrixItems ={
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, (float)lucencyPercent/100f, 0},
                    new float[] {0, 0, 0, 0, 1}
                };
				ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
				ImageAttributes imgAttr = new ImageAttributes();
				imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				//绘制阴影图像
				g.DrawImage(maskImage, new Rectangle(x, y, rectWidth, rectHeight),
					0, 0, maskImage.Width, maskImage.Height, GraphicsUnit.Pixel, imgAttr);

				//保存文件
				string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };
				FileInfo file = new FileInfo(srcPicFileFullName);

				ImageFormat imageType = ImageFormat.Gif;
				switch (file.Extension.ToLower())
				{
					case ".jpg":
						imageType = ImageFormat.Jpeg;
						break;
					case ".gif":
						imageType = ImageFormat.Gif;
						break;
					case ".png":
						imageType = ImageFormat.Png;
						break;
					case ".bmp":
						imageType = ImageFormat.Bmp;
						break;
					case ".tif":
						imageType = ImageFormat.Tiff;
						break;
					case ".wmf":
						imageType = ImageFormat.Wmf;
						break;
					case ".ico":
						imageType = ImageFormat.Icon;
						break;
					default:
						break;
				}

				MemoryStream ms = new MemoryStream();
				srcImage.Save(ms, imageType);
				byte[] imgData = ms.ToArray();
				srcImage.Dispose();
				maskImage.Dispose();
				g.Dispose();

				FileStream fs = null;

				fs = new FileStream(outPicFileFullName, FileMode.Create, FileAccess.Write);

				if (fs != null)
				{
					fs.Write(imgData, 0, imgData.Length);
					fs.Close();
				}

				result = true;
			}
			catch
			{
				result = false;
			}

			finally
			{
				try
				{
					maskImage.Dispose();
					srcImage.Dispose();
					g.Dispose();
				}
				catch { }
			}

			return result;
		}

		#endregion 给图片加水印
	}
}
