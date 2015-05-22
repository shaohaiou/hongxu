using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class CardSettingInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 权限用户
        /// </summary>
        [JsonIgnore]
        public string PowerUser
        {
            get { return GetString("PowerUser", string.Empty); }
            set { SetExtendedAttribute("PowerUser", value.ToString()); }
        }

        /// <summary>
        /// 总开关
        /// </summary>
        [JsonIgnore]
        public int Switch
        {
            get { return GetInt("Switch", 0); }
            set { SetExtendedAttribute("Switch", value.ToString()); }
        }

        /// <summary>
        /// AppID
        /// </summary>
        [JsonIgnore]
        public string AppID
        {
            get { return GetString("AppID", ""); }
            set { SetExtendedAttribute("AppID", value); }
        }

        /// <summary>
        /// AppSecret
        /// </summary>
        [JsonIgnore]
        public string AppSecret
        {
            get { return GetString("AppSecret", ""); }
            set { SetExtendedAttribute("AppSecret", value); }
        }

        /// <summary>
        /// 微信号
        /// </summary>
        [JsonIgnore]
        public string AppNumber
        {
            get { return GetString("AppNumber", ""); }
            set { SetExtendedAttribute("AppNumber", value); }
        }

        /// <summary>
        /// 公众号名称
        /// </summary>
        [JsonIgnore]
        public string AppName
        {
            get { return GetString("AppName", ""); }
            set { SetExtendedAttribute("AppName", value); }
        }

        /// <summary>
        /// 公众号二维码图片
        /// </summary>
        [JsonIgnore]
        public string AppImgUrl
        {
            get { return GetString("AppImgUrl", ""); }
            set { SetExtendedAttribute("AppImgUrl", value); }
        }

        /// <summary>
        /// 关注引导页面
        /// </summary>
        [JsonIgnore]
        public string AttentionUrl
        {
            get { return GetString("AttentionUrl", ""); }
            set { SetExtendedAttribute("AttentionUrl", value); }
        }

        /// <summary>
        /// 活动背景图
        /// </summary>
        [JsonIgnore]
        public string BgImgUrl
        {
            get { return GetString("BgImgUrl", ""); }
            set { SetExtendedAttribute("BgImgUrl", value); }
        }

        /// <summary>
        /// 活动规则
        /// </summary>
        [JsonIgnore]
        public string ActRule
        {
            get { return GetString("ActRule", ""); }
            set { SetExtendedAttribute("ActRule", value); }
        }

        /// <summary>
        /// 奖项设置
        /// </summary>
        [JsonIgnore]
        public string Awards
        {
            get { return GetString("Awards", ""); }
            set { SetExtendedAttribute("Awards", value); }
        }



        /// <summary>
        /// 中奖率
        /// </summary>
        [JsonIgnore]
        public int WinRate
        {
            get { return GetInt("WinRate", 100); }
            set { SetExtendedAttribute("WinRate", value.ToString()); }
        }

        /// <summary>
        /// 分享图片Url
        /// </summary>
        [JsonIgnore]
        public string ShareImgUrl
        {
            get { return GetString("ShareImgUrl", ""); }
            set { SetExtendedAttribute("ShareImgUrl", value); }
        }

        /// <summary>
        /// 分享链接地址
        /// </summary>
        [JsonIgnore]
        public string ShareLinkUrl
        {
            get { return GetString("ShareLinkUrl", ""); }
            set { SetExtendedAttribute("ShareLinkUrl", value); }
        }

        /// <summary>
        /// 分享描述
        /// </summary>
        [JsonIgnore]
        public string ShareDesc
        {
            get { return GetString("ShareDesc", ""); }
            set { SetExtendedAttribute("ShareDesc", value); }
        }

        /// <summary>
        /// 分享标题
        /// </summary>
        [JsonIgnore]
        public string ShareTitle
        {
            get { return GetString("ShareTitle", ""); }
            set { SetExtendedAttribute("ShareTitle", value); }
        }
    }
}
