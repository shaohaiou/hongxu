using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using Hx.Tools;
using System.Drawing;

namespace Hx.BackAdmin.webservice
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class UploadServices : System.Web.Services.WebService
    {
        [WebMethod(Description = "Web 服务提供的方法，文件上载")]
        public string UploadImage(byte[] fs, string fileName)
        {
            try
            {
                string _path = "/upload";
                string path = Server.MapPath(_path);
                DateTime time = DateTime.Now;
                ///定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。
                string uploadPath = string.Format(@"{0}", path);
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string extension = Path.GetExtension(fileName);
                string newfileName = Path.GetFileNameWithoutExtension(fileName) + "_" + time.ToString("yyyyMMddHHmmss") + extension;
                string fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                while (System.IO.File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                FileStream f = new FileStream(fullFileName, FileMode.Create);
                ///把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                string url = string.Format("{0}/{1}", _path, newfileName.Replace(@"\", "/"));
                return url;
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.ToString();
            }
        }
        [WebMethod(Description = "Web 服务提供的方法，文件上载")]
        public string WeixinUploadImage(byte[] fs, string fileName)
        {
            try
            {
                string _path = "/upload/weixin/benzvote";
                string path = Server.MapPath(_path);
                DateTime time = DateTime.Now;
                ///定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。
                string uploadPath = string.Format(@"{0}", path);
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string extension = Path.GetExtension(fileName);
                string newfileName = Path.GetFileNameWithoutExtension(fileName) + "_" + time.ToString("yyyyMMddHHmmss") + extension;
                string fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                while (System.IO.File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                FileStream f = new FileStream(fullFileName, FileMode.Create);
                ///把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                string url = string.Format("{0}/{1}", _path, newfileName.Replace(@"\", "/"));
                return url;
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.ToString();
            }
        }
        [WebMethod(Description = "Web 服务提供的方法，文件上载")]
        public string WeixinjtUploadImage(byte[] fs, string fileName)
        {
            try
            {
                string _path = "/upload/weixin/jituanvote";
                string path = Server.MapPath(_path);
                DateTime time = DateTime.Now;
                ///定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。
                string uploadPath = string.Format(@"{0}", path);
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string extension = Path.GetExtension(fileName);
                string newfileName = Path.GetFileNameWithoutExtension(fileName) + "_" + time.ToString("yyyyMMddHHmmss") + extension;
                string fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                while (System.IO.File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                FileStream f = new FileStream(fullFileName, FileMode.Create);
                ///把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                string url = string.Format("{0}/{1}", _path, newfileName.Replace(@"\", "/"));
                return url;
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.ToString();
            }
        }
        [WebMethod(Description = "Web 服务提供的方法，文件上载")]
        public string JobUploadImage(byte[] fs, string fileName)
        {
            try
            {
                string _path = "/upload/job";
                string path = Server.MapPath(_path);
                DateTime time = DateTime.Now;
                ///定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。
                string uploadPath = string.Format(@"{0}", path);
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string extension = Path.GetExtension(fileName);
                string newfileName = Path.GetFileNameWithoutExtension(fileName) + "_" + time.ToString("yyyyMMddHHmmss") + extension;
                string fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                while (System.IO.File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                FileStream f = new FileStream(fullFileName, FileMode.Create);
                ///把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                string url = string.Format("{0}/{1}", _path, newfileName.Replace(@"\", "/"));
                return url;
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.ToString();
            }
        }
        [WebMethod(Description = "Web 服务提供的方法，ckeditor文件上载")]
        public string CkeditorUpload(byte[] fs, string fileName)
        {
            try
            {
                string _path = "/upload";
                string path = Server.MapPath(_path);
                DateTime time = DateTime.Now;
                ///定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                ///定义实际文件对象，保存上载的文件。
                string virtualPath = string.Format(@"{0}\", "fck");
                string uploadPath = string.Format(@"{0}\{1}", path, virtualPath);
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string extension = Path.GetExtension(fileName);
                string newfileName = time.ToString("yyyyMMddHHmmss") + Utils.CreateRandom(1000, 9999) + extension;
                string fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                while (System.IO.File.Exists(fullFileName))
                {
                    newfileName = time.ToString("yyyyMMddHHmmss") + Utils.CreateRandom(1000, 9999) + extension;
                    fullFileName = string.Format(@"{0}\{1}", uploadPath, newfileName);
                }
                FileStream f = new FileStream(fullFileName, FileMode.Create);
                ///把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                string url = string.Format("{0}/{1}", _path, (virtualPath + newfileName).Replace(@"\", "/"));
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
                //return ex.ToString();
            }
        }

        [WebMethod(Description = "Web 服务提供的方法，裁剪图片")]
        public string CutImage(string src, string zoom, string x, string y, string width, string height)
        {
            try
            {
                //检查是否有权限操作
                string url = src;
                src = Server.MapPath(src.Replace("http://" + new Uri(src).Host, string.Empty));
                string topath = src.Replace("\\src\\", "\\pic\\");
                string ext = Path.GetExtension(src).ToLower();
                string newfileName = topath.Substring(0, topath.Length - ext.Length) + "_c" + ext;//裁剪后的文件名
                string newfileurl = url.Replace("/src/", "/pic/").Substring(0, url.Length - ext.Length) + "_c" + ext;
                int _width = (int)(int.Parse(width) / double.Parse(zoom));
                int _height = (int)(int.Parse(height) / double.Parse(zoom));
                int _x = (int)(-int.Parse(x) / double.Parse(zoom));
                int _y = (int)(-int.Parse(y) / double.Parse(zoom));

                Bitmap oldBitmap = new Bitmap(src);
                Cutter cut = new Cutter(
                    double.Parse(zoom),
                    -int.Parse(x),
                    -int.Parse(y),
                    int.Parse(width),
                    int.Parse(height),
                    oldBitmap.Width,
                    oldBitmap.Height);

                Bitmap bmp = ImageHelper.GenerateBitmap(oldBitmap, cut);
                string temp = newfileName;
                ImageHelper.MakeThumbnail(bmp, temp, 200, 150);

                System.Drawing.Image originalImage = System.Drawing.Image.FromFile(src, true);

                originalImage.Dispose();
                oldBitmap.Dispose();
                //File.Delete(src);

                return "{msg:'success',src:'" + newfileurl + "'}";//返回图片路径
            }
            catch
            {
                return "{msg:'error'}";//返回出错信息
            }
        }

        [WebMethod(Description = "Web 服务提供的方法，裁剪图片")]
        public string CutImages(string src, string zoom, string x, string y, string width, string height)
        {
            try
            {
                //检查是否有权限操作
                string url = src;
                src = Server.MapPath(src.Replace("http://" + new Uri(src).Host, string.Empty));
                string topath = src.Replace("\\src\\", "\\pics\\");
                string ext = Path.GetExtension(src).ToLower();
                string newfileName = topath.Substring(0, topath.Length - ext.Length) + "_c" + ext;//裁剪后的文件名
                string newfileurl = url.Replace("/src/", "/pics/").Substring(0, url.Length - ext.Length + 1) + "_c" + ext;
                string newfileName_s = topath.Substring(0, topath.Length - ext.Length) + "_s" + ext;//裁剪后的文件名
                string newfileurl_s = url.Replace("/src/", "/pics/").Substring(0, url.Length - ext.Length + 1) + "_s" + ext;
                int _width = (int)(int.Parse(width) / double.Parse(zoom));
                int _height = (int)(int.Parse(height) / double.Parse(zoom));
                int _x = (int)(-int.Parse(x) / double.Parse(zoom));
                int _y = (int)(-int.Parse(y) / double.Parse(zoom));

                Bitmap oldBitmap = new Bitmap(src);
                Cutter cut = new Cutter(
                    1,
                    -int.Parse(x),
                    -int.Parse(y),
                    _width,
                    _height,
                    oldBitmap.Width,
                    oldBitmap.Height);

                Bitmap bmp = ImageHelper.GenerateBitmap(oldBitmap, cut);
                string temp = newfileName;
                ImageHelper.MakeThumbnail(bmp, temp, 800, 600);
                bmp = ImageHelper.GenerateBitmap(oldBitmap, cut);
                temp = newfileName_s;
                ImageHelper.MakeThumbnail(bmp, temp, 460, 345);

                System.Drawing.Image originalImage = System.Drawing.Image.FromFile(src, true);

                originalImage.Dispose();
                oldBitmap.Dispose();
                //File.Delete(src);

                return "{msg:'success',src:'" + newfileurl_s + "'}";//返回图片路径
            }
            catch
            {
                return "{msg:'error'}";//返回出错信息
            }
        }
    }
}
