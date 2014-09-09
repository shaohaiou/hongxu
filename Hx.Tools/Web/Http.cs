using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Hx.Tools.Web
{
    public class Http
    {
        #region GetPage

        public static string GetPage(string url)
        {
            string content = "";
            try
            {
                HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest1.KeepAlive = false;
                HttpWebResponse myHttpWebResponse1;

                myHttpWebResponse1 = (HttpWebResponse)myHttpWebRequest1.GetResponse();
                System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                Stream streamResponse = myHttpWebResponse1.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, utf8);
                Char[] readBuff = new Char[256];
                int count = streamRead.Read(readBuff, 0, 256);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    content += outputData;
                    count = streamRead.Read(readBuff, 0, 256);
                }
                myHttpWebResponse1.Close();
            }
            catch { }
            return (content);

        }

        /// <summary>  
        /// 获取Uri网页信息，默认编码utf8  
        /// </summary>  
        /// <param name="uri"></param>  
        /// <returns></returns>  
        public static string GetPage(Uri uri)
        {
            string content = "";
            HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)WebRequest.Create(uri);
            myHttpWebRequest1.KeepAlive = false;
            HttpWebResponse myHttpWebResponse1;
            try
            {
                myHttpWebResponse1 = (HttpWebResponse)myHttpWebRequest1.GetResponse();
                System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                Stream streamResponse = myHttpWebResponse1.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, utf8);
                Char[] readBuff = new Char[256];
                int count = streamRead.Read(readBuff, 0, 256);
                while (count > 0)
                {
                    String outputData = new String(readBuff, 0, count);
                    content += outputData;
                    count = streamRead.Read(readBuff, 0, 256);
                }
                myHttpWebResponse1.Close();
                return (content);
            }
            catch (WebException ex)
            {
                content = "在请求URL为：" + uri.ToString() + "的页面时产生错误，错误信息为" + ex.ToString();
                return (content);
            }
        }

        static CookieContainer m_cookCont = new CookieContainer();

        /// <summary>  
        /// 从URL获取数据。httpUrl:GET请求网址autoRedirect:是否自动跳转. encoding：编码  
        /// </summary>  
        /// <param name="httpUrl"></param>  
        /// <param name="autoRedirect"></param>  
        /// <returns></returns>  
        public static string GetPage(string httpUrl, bool autoRedirect, string encoding)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.AllowAutoRedirect = autoRedirect;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }
            return data;
        }

        /// <summary>  
        /// 从URL获取数据  
        /// </summary>  
        /// <param name="httpUrl">url</param>  
        /// <param name="ua"& gt;UserAgent信息</param>  
        /// <returns></returns>  
        public static string GetPage(string httpUrl, string ua)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.UserAgent = ua;
                httpWReq.AllowAutoRedirect = false;
                httpWReq.Timeout = 60000;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch
            {
            }
            return data;
        }

        /// <summary>  
        /// UTF-8读URL  
        /// </summary>  
        /// <param name="httpUrl"></param>  
        /// <returns></returns>  
        public static string GetPageByWebClientUTF8(string httpUrl)
        {
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;

            //方法一：  
            try
            {
                //使用DownloadData方法 从资源下载数据并返回 Byte 数组。  
                Byte[] pageData = wc.DownloadData(httpUrl);
                return Encoding.UTF8.GetString(pageData);
            }
            catch (Exception ex)
            {
                string content = "在请求URL为：" + httpUrl + "的页面时产生错误，错误信息为" + ex.ToString();
                return (content);
            }

        }

        /// <summary>  
        /// Default读URL  
        /// </summary>  
        /// <param name="httpUrl"></param>  
        /// <returns></returns>  
        public static string GetPageByWebClientDefault(string httpUrl)
        {
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            //方法二：   
            // ***************代码开始*******    
            try
            {
                //从资源以 Stream 的形式返回数据。  
                Stream resStream = wc.OpenRead(httpUrl);
                StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
                string text = sr.ReadToEnd();
                resStream.Close();
                return text;
            }
            catch (Exception ex)
            {
                string content = "在请求URL为：" + httpUrl + "的页面时产生错误，错误信息为" + ex.ToString();
                return (content);
            }
            // **************代码结束********   
        }

        /// <summary>  
        /// 将数据提交到URL，然后接受返回的数据  
        /// </summary>  
        /// <param name="postString"& gt;提交的字符串</param>  
        /// <param name="httpUrl">URL</param>  
        /// <returns></returns>  
        public static string GetPageByWebClient(string postString, string httpUrl)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            byte[] postData = Encoding.Default.GetBytes(postString);

            try
            {
                //将字节数组发送到资源，并返回包含任何响应的 Byte 数组。  
                byte[] responseData = client.UploadData(httpUrl, "POST", postData);
                Encoding _gbk = Encoding.Default;
                return _gbk.GetString(responseData);
            }
            catch (Exception ex)
            {
                string content = "在请求URL为：" + httpUrl + "的页面时产生错误，错误信息为" + ex.ToString();
                return (content);
            }
        }

        #endregion

        #region PostPage

        /// <summary>  
        /// postData:要发送的数据  
        /// xhttpUrl:发送网址  
        /// </summary>  
        /// <param name="postData"></param>  
        /// <param name="xhttpUrl"></param>  
        /// <returns></returns>  
        public static string Post(string postData, string xhttpUrl, bool autoRedirect, string encoding)
        {
            string cookieHeader = "";
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(xhttpUrl);
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            httpWReq.Method = "POST";
            httpWReq.AllowAutoRedirect = autoRedirect;
            httpWReq.CookieContainer = m_cookCont;
            httpWReq.CookieContainer.SetCookies(new Uri(xhttpUrl), cookieHeader);
            Stream reqStream = httpWReq.GetRequestStream();
            StreamWriter reqStreamWrite = new StreamWriter(reqStream);
            byte[] pdata = Encoding.Default.GetBytes(postData);
            char[] cpara = new char[pdata.Length];
            for (int i = 0; i < pdata.Length; i++)
            { cpara[i] = System.Convert.ToChar(pdata[i]); }
            reqStreamWrite.Write(cpara, 0, cpara.Length);

            reqStreamWrite.Close();
            reqStream.Close();
            HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
            httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
            m_cookCont = httpWReq.CookieContainer;
            Stream respStream = httpWResp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding(encoding));
            string data = respStreamReader.ReadToEnd();
            respStreamReader.Close();
            respStream.Close();
            return data;
        }
        /// <summary>  
        /// 向网址发送数据  
        /// </summary>  
        /// <param name="postData"></param>  
        /// <param name="xhttpUrl"></param>  
        /// <returns></returns>  
        public static string Post(string postData, string xhttpUrl)
        {
            string cookieHeader = "";
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(xhttpUrl);
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            httpWReq.Method = "POST";
            httpWReq.AllowAutoRedirect = false;
            httpWReq.CookieContainer = m_cookCont;
            httpWReq.CookieContainer.SetCookies(new Uri(xhttpUrl), cookieHeader);
            Stream reqStream = httpWReq.GetRequestStream();
            StreamWriter reqStreamWrite = new StreamWriter(reqStream);
            byte[] pdata = Encoding.Default.GetBytes(postData);
            //char[] cpara = new ASCIIEncoding().GetChars(pdata);  
            char[] cpara = new char[pdata.Length];
            for (int i = 0; i < pdata.Length; i++)
            { cpara[i] = System.Convert.ToChar(pdata[i]); }
            reqStreamWrite.Write(cpara, 0, cpara.Length);
            reqStreamWrite.Close();
            reqStream.Close();
            HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
            httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
            m_cookCont = httpWReq.CookieContainer;
            Stream respStream = httpWResp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.Default);
            string data = respStreamReader.ReadToEnd();
            respStreamReader.Close();
            respStream.Close();
            return data;
        }
        public static string Post(string postData, string xhttpUrl, string encoding)
        {
            return Post(postData, xhttpUrl, false, encoding);
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="RemoteFileUrl"></param>
        /// <param name="saveFilePath"></param>
        /// <param name="timeout"></param>
        /// <param name="tryTimes"></param>
        public static void DownloadFile(string RemoteFileUrl, string saveFilePath, int timeout, int tryTimes = 0)
        {
            try
            {
                FileInfo filesave = new FileInfo(saveFilePath);
                if (!Directory.Exists(filesave.DirectoryName))
                    Directory.CreateDirectory(filesave.DirectoryName);
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(RemoteFileUrl, saveFilePath);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch
            {
                if (tryTimes >= 3)
                {
                    return;
                }
                System.Threading.Thread.Sleep(500);
                DownloadFile(RemoteFileUrl, saveFilePath, timeout, ++tryTimes);
            }
        }

        #endregion 文件下载
    }
}
