using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class JituanvotePothunterInfo : ExtendedAttributes
    {
        [JsonProperty("ID")]
        public int ID { get; set; }
        
        /// <summary>
        /// 姓名
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return GetString("Name", ""); }
            set { SetExtendedAttribute("Name", value); }
        }

        /// <summary>
        /// 照片
        /// </summary>
        [JsonIgnore]
        public string PicPath
        {
            get { return GetString("PicPath", ""); }
            set { SetExtendedAttribute("PicPath", value); }
        }

        /// <summary>
        /// 序号
        /// </summary>
        [JsonIgnore]
        public int SerialNumber
        {
            get { return GetInt("SerialNumber", 0); }
            set { SetExtendedAttribute("SerialNumber", value.ToString()); }
        }

        /// <summary>
        /// 得票数
        /// </summary>
        [JsonIgnore]
        public int Ballot
        {
            get { return GetInt("Ballot", 0); }
            set { SetExtendedAttribute("Ballot", value.ToString()); }
        }

        /// <summary>
        /// 排名
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 风采展示图片1
        /// </summary>
        [JsonIgnore]
        public string IntroducePic1
        {
            get { return GetString("IntroducePic1", ""); }
            set { SetExtendedAttribute("IntroducePic1", value); }
        }

        /// <summary>
        /// 风采展示图片2
        /// </summary>
        [JsonIgnore]
        public string IntroducePic2
        {
            get { return GetString("IntroducePic2", ""); }
            set { SetExtendedAttribute("IntroducePic2", value); }
        }

        /// <summary>
        /// 风采展示图片3
        /// </summary>
        [JsonIgnore]
        public string IntroducePic3
        {
            get { return GetString("IntroducePic3", ""); }
            set { SetExtendedAttribute("IntroducePic3", value); }
        }

        /// <summary>
        /// 参赛宣言
        /// </summary>
        [JsonIgnore]
        public string Declare
        {
            get { return GetString("Declare", ""); }
            set { SetExtendedAttribute("Declare", value); }
        }

        /// <summary>
        /// 个人介绍
        /// </summary>
        [JsonIgnore]
        public string Introduce
        {
            get { return GetString("Introduce", ""); }
            set { SetExtendedAttribute("Introduce", value); }
        }

        /// <summary>
        /// 被评论数
        /// </summary>
        [JsonIgnore]
        public int Comments
        {
            get { return GetInt("Comments", 0); }
            set { SetExtendedAttribute("Comments", value.ToString()); }
        }

    }
}
