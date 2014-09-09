using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    public class ShowUrl
    {
        private string _text;
        private string _url;

        /// <summary>
        /// 链接文本
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
    /// <summary>
    /// 需要显示的Url跟文本 的集合
    /// </summary>
    public class ShowUrlCollection : List<ShowUrl>
    {
    }
}
