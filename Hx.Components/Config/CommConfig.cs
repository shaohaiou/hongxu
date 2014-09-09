using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.IO;

namespace Hx.Components.Config
{
    /// <summary>
    /// 项目的主要配置文件
    /// </summary>
    [Serializable]
    public class CommConfig
    {
        #region 构造函数

        private CommConfig(XmlDocument doc)
        {
            XmlDoc = doc;
            LoadValuesFromConfigurationXml();
        }

        #endregion

        #region 私有成员

        Hashtable providers = new Hashtable();                              //保存数据库访问的提供者类
        NameValueCollection appSetting = new NameValueCollection();         //appsetting节点信息
        private int cacheFactor = 5;                                        //默认的缓存权值
        private string version = "1.0";                                     //版本号
        private XmlDocument XmlDoc = null;                                  //配置文件根节点
        private bool disableBackgroundThreads = false;                      //是否允许后台进程运行                                  //允许访问的文件：不经过Url重写
        #endregion

        #region 公共属性和方法

        public NameValueCollection AppSetting { get { return appSetting; } }
        public Hashtable Providers { get { return providers; } }
        public int CacheFactor { get { return cacheFactor; } }
        public bool IsDisableBackgroundThreads { get { return disableBackgroundThreads; } }
        public string Version { get { return version; } }



        /// <summary>
        /// 加载配置文件，并且封装配置信息
        /// </summary>
        /// <returns></returns>
        public static CommConfig GetConfig()
        {
            CommConfig config = MangaCache.GetLocal(GlobalKey.COMMCONFIG) as CommConfig;//尝试从缓存获取配置信息类
            if (config == null)
            {
                //当缓存中不存在时建立
                string path = GetFilePath();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                config = new CommConfig(doc);
                MangaCache.MaxLocalWithFile(GlobalKey.COMMCONFIG, config, path);
            }
            return config;
        }

        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <param name="nodePath">节点path</param>
        /// <returns>返回节点</returns>
        public XmlNode GetConfigSection(string nodePath)
        {
            return XmlDoc.SelectSingleNode(nodePath);
        }

        #endregion

        #region 私有方法

        private static string GetFilePath()
        {
            string path = null;
            HttpContext context = HttpContext.Current;
            if (context != null)
                path = context.Server.MapPath("~/config/common.config");
            else
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config\common.config");
            return path;
        }

        /// <summary>
        /// 遍历各个节点加载配置信息
        /// </summary>
        private void LoadValuesFromConfigurationXml()
        {
            XmlNode node = GetConfigSection("common/core");

            XmlAttributeCollection attributeCollection = node.Attributes;
            XmlAttribute att = null;
            //加载缓存权值信息
            att = attributeCollection["cacheFactor"];
            if (att != null)
                cacheFactor = Int32.Parse(att.Value);
            else
                cacheFactor = 5;
            att = attributeCollection["disableBackgroundThreads"];
            if (att != null)
                disableBackgroundThreads = bool.Parse(att.Value);
            else
                disableBackgroundThreads = false;
            att = attributeCollection["version"];
            if (att != null)
                version = att.Value;
            foreach (XmlNode child in node.ChildNodes)
            {
                //加载appSetting节点
                if (child.Name == "appSettings")
                {
                    GetAppSetting(child, appSetting);
                }
                //加载providers节点
                if (child.Name == "providers")
                {
                    GetProviders(child, providers);
                }

            }
        }


        /// <summary>
        /// 加载数据访问类型的信息
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="table">保存数据库访问类的容器</param>
        private void GetProviders(XmlNode node, Hashtable table)
        {

            foreach (XmlNode provider in node.ChildNodes)
            {

                switch (provider.Name)
                {
                    case "add":
                        table.Add(provider.Attributes["name"].Value, new Provider(provider.Attributes));
                        break;

                    case "remove":
                        table.Remove(provider.Attributes["name"].Value);
                        break;

                    case "clear":
                        table.Clear();
                        break;

                }

            }

        }

        /// <summary>
        /// 加载站点配置appSetting的信息
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="nv">保存站点配置字符串信息的容器</param>
        private void GetAppSetting(XmlNode node, NameValueCollection nv)
        {
            foreach (XmlNode app in node.ChildNodes)
            {

                switch (app.Name)
                {
                    case "add":
                        nv.Add(app.Attributes["key"].Value, app.Attributes["value"].Value);
                        break;

                    case "remove":
                        nv.Remove(app.Attributes["key"].Value);
                        break;

                    case "clear":
                        nv.Clear();
                        break;

                }

            }

        }


        #endregion
    }

    /// <summary>
    /// 用于保存单个数据库访问提供者类型信息
    /// </summary>
    [Serializable]
    public class Provider
    {
        string name;//标示名称
        string providerType;//具体类型
        NameValueCollection providerAttributes = new NameValueCollection();//保存其他属性

        public Provider(XmlAttributeCollection attributes)
        {
            name = attributes["name"].Value;
            providerType = attributes["type"].Value;
            foreach (XmlAttribute attribute in attributes)
            {
                //遍历保存其他信息
                if ((attribute.Name != "name") && (attribute.Name != "type"))
                    providerAttributes.Add(attribute.Name, attribute.Value);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Type
        {
            get
            {
                return providerType;
            }
        }

        public NameValueCollection Attributes
        {
            get
            {
                return providerAttributes;
            }
        }

    }
}
