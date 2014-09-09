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
    /// 图片工具类
    /// </summary>
    public class ImageHelp
    {
        /// <summary>
        /// 获取图片中的各帧
        /// </summary>
        /// <param name="pPath">源图片路径</param>
        /// <param name="pSavePath">保存目录</param>
        public void GetFrames(string pPath, string pSavedPath)
        {
            Image gif = Image.FromFile(pPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);

            //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            int count = gif.GetFrameCount(fd);

            //以Jpeg格式保存各帧
            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(pSavedPath + "\\frame_" + i + ".jpg", ImageFormat.Jpeg);
            }
        }

        /**/
        /// <summary>
        /// 获取图片缩略图
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pWidth">缩略图宽度</param>
        /// <param name="pHeight">缩略图高度</param>
        public void GetSmaller(string pPath, string pSavedPath, int pWidth, int pHeight)
        {
            string fileSaveUrl = pSavedPath + "\\smaller.jpg";

            using (FileStream fs = new FileStream(pPath, FileMode.Open))
            {

                MakeSmallImg(fs, fileSaveUrl, pWidth, pHeight);
            }

        }

        //按模版比例生成缩略图（以流的方式获取源文件）  
        //生成缩略图函数  
        //顺序参数：源图文件流、缩略图存放地址、模版宽、模版高  
        //注：缩略图大小控制在模版区域内  
        public void MakeSmallImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight)
        {
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(fromFileStream, true);

            //缩略图宽、高  
            System.Double newWidth = myImage.Width, newHeight = myImage.Height;
            //宽大于模版的横图  
            if (myImage.Width > myImage.Height || myImage.Width == myImage.Height)
            {
                if (myImage.Width > templateWidth)
                {
                    //宽按模版，高按比例缩放  
                    newWidth = templateWidth;
                    newHeight = myImage.Height * (newWidth / myImage.Width);
                }
            }
            //高大于模版的竖图  
            else
            {
                if (myImage.Height > templateHeight)
                {
                    //高按模版，宽按比例缩放  
                    newHeight = templateHeight;
                    newWidth = myImage.Width * (newHeight / myImage.Height);
                }
            }

            //取得图片大小  
            System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
            System.Drawing.GraphicsUnit.Pixel);

            /////文字水印  
            //System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(bitmap);
            //System.Drawing.Font f = new Font("Lucida Grande", 6);
            //System.Drawing.Brush b = new SolidBrush(Color.Gray);
            //G.DrawString("Ftodo.com", f, b, 0, 0);
            //G.Dispose();

            ///图片水印  
            //System.Drawing.Image   copyImage   =   System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("pic/1.gif"));  
            //Graphics   a   =   Graphics.FromImage(bitmap);  
            //a.DrawImage(copyImage,   new   Rectangle(bitmap.Width-copyImage.Width,bitmap.Height-copyImage.Height,copyImage.Width,   copyImage.Height),0,0,   copyImage.Width,   copyImage.Height,   GraphicsUnit.Pixel);  

            //copyImage.Dispose();  
            //a.Dispose();  
            //copyImage.Dispose();  

            //保存缩略图  
            if (File.Exists(fileSaveUrl))
            {
                File.SetAttributes(fileSaveUrl, FileAttributes.Normal);
                File.Delete(fileSaveUrl);
            }

            bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();
        }

        /**/
        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="pFormat">保存格式，通常可以是jpeg</param>
        public void GetPart(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            string normalJpgPath = pSavedPath + "\\normal.jpg";

            using (Image originalImg = Image.FromFile(pPath))
            {
                Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);
                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）


                ///文字水印  
                System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(partImg);
                System.Drawing.Font f = new Font("Lucida Grande", 6);
                System.Drawing.Brush b = new SolidBrush(Color.Gray);
                G.Clear(Color.White);
                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
                G.DrawString("Ftodo.com", f, b, 0, 0);
                G.Dispose();

                originalImg.Dispose();
                if (File.Exists(normalJpgPath))
                {
                    File.SetAttributes(normalJpgPath, FileAttributes.Normal);
                    File.Delete(normalJpgPath);
                }
                partImg.Save(normalJpgPath, ImageFormat.Jpeg);
            }
        }

        /**/
        /// <summary>
        /// 获取按比例缩放的图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="imageWidth">缩放后的宽度</param>
        /// <param name="imageHeight">缩放后的高度</param>
        public void GetPart(string pPath, string pSavedPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY, int imageWidth, int imageHeight)
        {
            string normalJpgPath = pSavedPath + "\\normal.jpg";
            using (Image originalImg = Image.FromFile(pPath))
            {
                if (originalImg.Width == imageWidth && originalImg.Height == imageHeight)
                {
                    GetPart(pPath, pSavedPath, pPartStartPointX, pPartStartPointY, pPartWidth, pPartHeight, pOrigStartPointX, pOrigStartPointY);
                    return;
                }

                Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Image zoomImg = originalImg.GetThumbnailImage(imageWidth, imageHeight, callback, IntPtr.Zero);//缩放
                Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);

                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new Size(pPartWidth, pPartHeight));//目标位置
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

                ///文字水印  
                System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(partImg);
                System.Drawing.Font f = new Font("Lucida Grande", 6);
                System.Drawing.Brush b = new SolidBrush(Color.Gray);
                G.Clear(Color.White);

                graphics.DrawImage(zoomImg, destRect, origRect, GraphicsUnit.Pixel);
                G.DrawString("Ftodo.com", f, b, 0, 0);
                G.Dispose();

                originalImg.Dispose();
                if (File.Exists(normalJpgPath))
                {
                    File.SetAttributes(normalJpgPath, FileAttributes.Normal);
                    File.Delete(normalJpgPath);
                }
                partImg.Save(normalJpgPath, ImageFormat.Jpeg);
            }
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
        #region Helper


        /// <summary>
        /// 获得图像高宽信息
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public static ImageInformation GetImageInfo(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                ImageInformation imageInfo = new ImageInformation();
                imageInfo.Width = image.Width;
                imageInfo.Height = image.Height;
                return imageInfo;
            }
        }

        /// <summary>
        /// 通过文件名获取图片信息
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 图片翻转
        /// </summary>
        /// <param name="oldImage">图像源</param>
        /// <param name="angle">翻转角度( -180, -90, 90, 180,)</param>
        /// <returns></returns>
        public static Bitmap RotateImage(Image oldImage, float angle)
        {
            float na = Math.Abs(angle);
            if (na != 90 && na != 180)//不在角度范围的抛出异常
                throw new ArgumentException("angle could only be -180, -90, 90, 180 degrees, but now it is " + angle.ToString() + " degrees!");
            int ow = oldImage.Width;
            int oh = oldImage.Height;
            int nw = ow;
            int nh = oh;
            //90/-90
            if (na == 90)
            {
                nw = oh;
                nh = ow;
            }

            Bitmap bmp = new Bitmap(nw, nh);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;

                Point offset = new Point((nw - ow) / 2, (nh - oh) / 2);
                Rectangle rect = new Rectangle(offset.X, offset.Y, ow, oh);
                Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(angle);
                g.TranslateTransform(-center.X, -center.Y);
                g.DrawImage(oldImage, rect);
                g.ResetTransform();
                g.Save();
            }
            return bmp;
        }

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
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(oldImage, new Rectangle(0, 0, cut.SaveWidth, cut.SaveHeight), new Rectangle(0, 0, cut.Width, cut.Height), GraphicsUnit.Pixel);
                g.Save();
            }

            Bitmap bmp = new Bitmap(cut.CutterWidth, cut.CutterHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(newBitmap, 0, 0, new Rectangle(cut.X, cut.Y, cut.CutterWidth, cut.CutterHeight), GraphicsUnit.Pixel);
                g.Save();
                newBitmap.Dispose();
            }
            return bmp;
        }
        #endregion
    }



    /// <summary>
    /// 裁剪实体类
    /// </summary>
    public class Cutter
    {
        #region 构造函数

        public Cutter()
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
        { }

        public Cutter(double _Zoom, int _X, int _Y, int _CutterWidth, int _CutterHeight, int _Width, int _Height)
            : this(_Zoom, _X, _Y, _CutterWidth, _CutterHeight, _Width, _Height, (int)(_Zoom * _Width), (int)(_Zoom * _Height))
        { }

        public Cutter(double _Zoom, int _X, int _Y, int _CutterWidth, int _CutterHeight, int _Width, int _Height, int _SaveWidth, int _SaveHeight)
        {
            this._Zoom = _Zoom;
            this._X = _X;
            this._Y = _Y;
            this._CutterHeight = _CutterHeight;
            this._CutterWidth = _CutterWidth;
            this._Width = _Width;
            this._Height = _Height;
            this._SaveWidth = _SaveWidth;
            this._SaveHeight = _SaveHeight;
        }
        #endregion

        #region -Properties-
        private int _SaveWidth;
        /// <summary>
        /// 改变后的宽度
        /// </summary>
        public int SaveWidth
        {
            get { return _SaveWidth; }
            set { _SaveWidth = value; }
        }
        private int _SaveHeight;
        /// <summary>
        /// 改变后的高度
        /// </summary>
        public int SaveHeight
        {
            get { return _SaveHeight; }
            set { _SaveHeight = value; }
        }
        private double _Zoom;

        /// <summary>
        /// 放大率
        /// </summary>
        public double Zoom
        {
            get { return _Zoom; }
            set { _Zoom = value; }
        }

        private int _X;
        /// <summary>
        /// 剪切图片的开始横坐标
        /// </summary>
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }
        private int _Y;
        /// <summary>
        /// 剪切图片的开始纵坐标
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private int _CutterWidth;
        /// <summary>
        /// 当前宽度
        /// </summary>
        public int CutterWidth
        {
            get { return _CutterWidth; }
            set { _CutterWidth = value; }
        }
        private int _CutterHeight;
        /// <summary>
        ///当前高度
        /// </summary>
        public int CutterHeight
        {
            get { return _CutterHeight; }
            set { _CutterHeight = value; }
        }
        private int _Width;
        /// <summary>
        ///原始宽度
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        private int _Height;
        /// <summary>
        /// 原始高度
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        #endregion
    }
}
