using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Hx.Tools
{
    /// <summary>
    /// 图片帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 获取图片中的各帧并保存,一般gif文件包含多个帧
        /// </summary>
        /// <param name="pPath">源图片路径</param>
        /// <returns>返回帧数组</returns>
        public static Image[] GetFramesFromImage(string source)
        {
            Image gif = Image.FromFile(source);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            //获取帧数

            int count = gif.GetFrameCount(fd);
            Image[] imgs = new Image[count];
            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                imgs[i] = gif;
            }
            return imgs;

        }


        /// <summary>
        /// 获得图像高宽信息
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public static ImageInformation GetImageInfo(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                Image image = Image.FromStream(fs, false);
                ImageInformation imageInfo = new ImageInformation();
                imageInfo.Width = image.Width;
                imageInfo.Height = image.Height;
                imageInfo.Size = (int)fs.Length;
                image.Dispose();
                return imageInfo;
            }
        }

        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="mimeType">图片类型字符串</param>
        /// <returns></returns>
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


        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;

            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            return new Size(newWidth, newHeight);

        }


        #region 缩略图
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="source">图片地址</param>
        /// <param name="Stream">缩放后的Stream对象</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <return>缩略后的图片</return>
        public static void MakeThumbnail(string source, Stream stream, int width, int height)
        {
            //读取图片文件
            using (FileStream fs = new FileStream(source, FileMode.Open))
            {
                Image myImage = Image.FromStream(fs, true);
                MakeThumbnail(myImage, stream, width, height);
            }
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="srcBitmap">图片源</param>
        /// <param name="Stream">缩放后的Stream对象</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <return>缩略后的图片</return>
        public static void MakeThumbnail(Image srcBitmap, Stream stream, int width, int height)
        {
            //取得图片大小  
            Size mySize = ResizeImage(srcBitmap.Width, srcBitmap.Height, width, height);
            //新建一个bmp图片  
            Image bitmap = new Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(srcBitmap, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height),
            System.Drawing.GraphicsUnit.Pixel);
            bitmap.Save(stream, srcBitmap.RawFormat);
            g.Dispose();
            srcBitmap.Dispose();
            bitmap.Dispose();
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="srcBitmap">图片源</param>
        /// <param name="savepath">保存地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <return>缩略后的图片</return>
        public static void MakeThumbnail(Image srcBitmap, string savepath, int width, int height)
        {
            //取得图片大小  
            Size mySize = ResizeImage(srcBitmap.Width, srcBitmap.Height, width, height);
            //新建一个bmp图片  
            Image bitmap = new Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(srcBitmap, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height),
            System.Drawing.GraphicsUnit.Pixel);
            bitmap.Save(savepath, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            srcBitmap.Dispose();
            bitmap.Dispose();
        }


        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="source">图片源地址</param>
        /// <param name="savepath">保存地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <return>缩略后的图片</return>
        public static void MakeThumbnail(string source, string savepath, int width, int height)
        {
            using (FileStream fs = new FileStream(savepath, FileMode.Create))
            {
                MakeThumbnail(source, fs, width, height);
            }
        }

        #endregion

        #region 压缩图片

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Image srcBitmap, Stream destStream, long level)
        {
            ImageCodecInfo myImageCodecInfo;

            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;
            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
            srcBitmap.Dispose();
            //srcBitmap.Save(destStream, srcBitmap.RawFormat);
        }

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">待压缩的BMP文件名</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(string srcFile, string destFile, long level)
        {
            using (FileStream fs = new FileStream(srcFile, FileMode.Open))
            {
                Image bm = Image.FromStream(fs, true);
                Compress(bm, destFile, level);
                bm.Dispose();
            }
        }

        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitMap">传入的Bitmap对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Image srcBitMap, string destFile, long level)
        {
            using (Stream s = new FileStream(destFile, FileMode.Create))
            {
                Compress(srcBitMap, s, level);
            }
        }
        #endregion

        /// <summary>
        /// 图片编辑:缩放，压缩，亮度，对比度
        /// </summary>
        /// <param name="sourcefile">图片源路径</param>
        /// <param name="destfile">缩放后图片输出路径</param>
        /// <param name="destheight">缩放后图片高度</param>
        /// <param name="destwidth">缩放后图片宽度</param>
        /// <param name="light">图片亮度</param>
        /// <param name="contrast">图片对比度</param>
        /// <param name="destwidth">图片质量</param>
        /// <returns>是否编辑成功</returns>
        public static bool ProcessImages(string sourceFile, string destFile, int destHeight, int destWidth, int light, int contrast, int destde)
        {
            using (Stream s = new FileStream(destFile, FileMode.Create))
            {
                return ProcessImages(sourceFile, s, destHeight, destWidth, light, contrast, destde);
            }
        }

        /// <summary>
        /// 图片编辑:缩放，压缩，亮度，对比度
        /// </summary>
        /// <param name="sourcefile">图片源路径</param>
        /// <param name="stream">缩放后图片输出流</param>
        /// <param name="destheight">缩放后图片高度</param>
        /// <param name="destwidth">缩放后图片宽度</param>
        /// <param name="light">图片亮度</param>
        /// <param name="contrast">图片对比度</param>
        /// <param name="destwidth">图片质量</param>
        /// <returns>是否编辑成功</returns>
        public static bool ProcessImages(string sourceFile, Stream stream, int destHeight, int destWidth, int light, int contrast, int destde)
        {
            Image imgSource = null;
            Bitmap outBmp = null;

            try
            {
                imgSource = Image.FromFile(sourceFile);//获取源文件
                ImageFormat thisFormat = imgSource.RawFormat;//获取图片格式
                //thisFormat.
                #region 缩放
                //取得图片大小  
                Size mySize = ResizeImage(imgSource.Width, imgSource.Height, destWidth, destHeight);
                //新建一个bmp图片  
                outBmp = new Bitmap(mySize.Width, mySize.Height);
                //新建一个画板  
                Graphics g = Graphics.FromImage(outBmp);
                //设置高质量插值法  
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度  
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空一下画布  
                g.Clear(Color.WhiteSmoke);
                //在指定位置画图  
                g.DrawImage(imgSource, new System.Drawing.Rectangle(0, 0, outBmp.Width, outBmp.Height),
                new System.Drawing.Rectangle(0, 0, imgSource.Width, imgSource.Height),
                System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();
                outBmp.Save(stream, thisFormat);
                outBmp.Dispose();
                #endregion
                if (light != 0)
                {
                    int lt = (int)(light * 0.01 * 255);
                    outBmp = KiLighten(outBmp, lt);
                }
                if (contrast != 0)
                {
                    outBmp = KiContrast(outBmp, contrast);
                }
                Compress(outBmp, stream, destde);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (imgSource != null)
                {
                    imgSource.Dispose();
                }
                if (outBmp != null)
                {
                    outBmp.Dispose();
                }
            }
        }

        /// <summary>
        /// 图像明暗调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">亮度[-255, 255]</param>
        /// <returns></returns>
        public static Bitmap KiLighten(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }

            if (degree < -255) degree = -255;
            if (degree > 255) degree = 255;

            try
            {

                int width = b.Width;
                int height = b.Height;

                int pix = 0;

                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 处理指定位置像素的亮度
                            for (int i = 0; i < 3; i++)
                            {
                                pix = p[i] + degree;

                                if (degree < 0) p[i] = (byte)Math.Max(0, pix);
                                if (degree > 0) p[i] = (byte)Math.Min(255, pix);

                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }

                b.UnlockBits(data);

                return b;
            }
            catch
            {
                return null;
            }

        } // end of Lighten

        /// <summary>
        /// 图像对比度调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">对比度[-100, 100]</param>
        /// <returns></returns>
        public static Bitmap KiContrast(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }

            if (degree < -100) degree = -100;
            if (degree > 100) degree = 100;

            try
            {

                double pixel = 0;
                double contrast = (100.0 + degree) / 100.0;
                contrast *= contrast;
                int width = b.Width;
                int height = b.Height;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 处理指定位置像素的对比度
                            for (int i = 0; i < 3; i++)
                            {
                                pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                                if (pixel < 0) pixel = 0;
                                if (pixel > 255) pixel = 255;
                                p[i] = (byte)pixel;
                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }
                b.UnlockBits(data);
                return b;
            }
            catch
            {
                return null;
            }
        } // end of Contrast

        /// <summary>
        /// 产生新的图片
        /// </summary>
        /// <param name="oldImage">原来的图片</param>
        /// <param name="cut">裁剪信息实体类</param>
        /// <returns></returns>
        public static Bitmap GenerateBitmap(Image oldImage, Cutter cut)
        {
            if (oldImage == null)
                throw new ArgumentNullException("oldImage");//源图片不存在

            Image newBitmap = new Bitmap(cut.SaveWidth, cut.SaveHeight);
            //重画源图片
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(oldImage, new Rectangle(0, 0, cut.SaveWidth, cut.SaveHeight), new Rectangle(0, 0, cut.Width, cut.Height), GraphicsUnit.Pixel);
                g.Save();
            }

            Bitmap bmp = new Bitmap(cut.CutterWidth, cut.CutterHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(newBitmap, 0, 0, new Rectangle(cut.X, cut.Y, cut.CutterWidth, cut.CutterHeight), GraphicsUnit.Pixel);
                g.Save();
                newBitmap.Dispose();
            }
            return bmp;
        }

    }




    /// <summary>
    /// 图片信息
    /// </summary>
    public struct ImageInformation
    {
        private int width;

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        private int height;

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private int size;

        /// <summary>
        /// 图片文件大小
        /// </summary>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}
