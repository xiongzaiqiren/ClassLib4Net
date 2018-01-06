using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ClassLib4Net.VerificationCode
{
    public class ValidateImg
    {
        public static void ShowAuthCode(Stream stream, out string code)
        {
            Random random = new Random();
            code = random.Next(1000, 9999).ToString();

            Bitmap bitmap = CreateAuthCode(code);
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
        }

        public static string GenerateCheckCodes(int iCount)
        {
            int number;
            string checkCode = String.Empty;
            int iSeed = DateTime.Now.Millisecond;
            System.Random random = new Random(iSeed);
            for(int i = 0; i < iCount; i++)
            {
                number = random.Next(10);
                checkCode += number.ToString();
            }
            
            return checkCode;
        }

        private static Bitmap CreateAuthCode(string str)
        {
            Font fn = new Font("宋体", 12);
            Brush forecolor = Brushes.Black;
            Brush bgcolor = Brushes.White;
            PointF pf = new PointF(5, 5);
            Bitmap bitmap = new Bitmap(100, 25);
            Rectangle rec = new Rectangle(0, 0, 100, 25);
            Graphics gh = Graphics.FromImage(bitmap);
            gh.FillRectangle(bgcolor, rec);
            gh.DrawString(str, fn, forecolor, pf);
            return bitmap;
        }

        public static Bitmap CreateCheckCodeImage(string checkCode)
        {
            if(checkCode == null || checkCode.Trim() == String.Empty)
                return null;
            int iWordWidth = 15;
            int iImageWidth = checkCode.Length * iWordWidth;
            Bitmap image = new Bitmap(iImageWidth, 20);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器 
                Random random = new Random();
                //清空图片背景色 
                g.Clear(Color.White);

                //画图片的背景噪音点
                for(int i = 0; i < 20; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                //画图片的背景噪音线 
                for(int i = 0; i < 2; i++)
                {
                    int x1 = 0;
                    int x2 = image.Width;
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    if(i == 0)
                    {
                        g.DrawLine(new Pen(Color.Gray, 2), x1, y1, x2, y2);
                    }

                }


                for(int i = 0; i < checkCode.Length; i++)
                {

                    string Code = checkCode[i].ToString();
                    int xLeft = iWordWidth * (i);
                    random = new Random(xLeft);
                    int iSeed = DateTime.Now.Millisecond;
                    int iValue = random.Next(iSeed) % 4;
                    if(iValue == 0)
                    {
                        Font font = new Font("Arial", 13, (FontStyle.Bold | System.Drawing.FontStyle.Italic));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.Red, 1.5f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if(iValue == 1)
                    {
                        Font font = new System.Drawing.Font("楷体", 13, (FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.DarkRed, 1.3f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if(iValue == 2)
                    {
                        Font font = new System.Drawing.Font("宋体", 13, (System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Green, Color.Blue, 1.2f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                    else if(iValue == 3)
                    {
                        Font font = new System.Drawing.Font("黑体", 13, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Bold));
                        Rectangle rc = new Rectangle(xLeft, 0, iWordWidth, image.Height);
                        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Blue, Color.Green, 1.8f, true);
                        g.DrawString(Code, font, brush, xLeft, 2);
                    }
                }
                ////画图片的前景噪音点 
                for(int i = 0; i < 8; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线 
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                //Response.ClearContent();
                //Response.BinaryWrite(ms.ToArray());

                return image;
            }
            finally
            {
                g.Dispose();
                //image.Dispose();
            }
        }

    }
}
