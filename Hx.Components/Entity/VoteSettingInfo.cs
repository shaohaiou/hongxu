﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    public class VoteSettingInfo : ExtendedAttributes
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
        /// 页头图片
        /// </summary>
        [JsonIgnore]
        public string PageHeadImg
        {
            get { return GetString("PageHeadImg", ""); }
            set { SetExtendedAttribute("PageHeadImg", value); }
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
        /// 活动须知
        /// </summary>
        [JsonIgnore]
        public string MustKnow
        {
            get { return GetString("MustKnow", ""); }
            set { SetExtendedAttribute("MustKnow", value); }
        }

        /// <summary>
        /// 活动须知字体颜色
        /// </summary>
        [JsonIgnore]
        public string ColorMustKnow
        {
            get { return GetString("ColorMustKnow", ""); }
            set { SetExtendedAttribute("ColorMustKnow", value); }
        }

        /// <summary>
        /// 投票期限
        /// </summary>
        [JsonIgnore]
        public int OverdueMinutes
        {
            get { return GetInt("OverdueMinutes", 0); }
            set { SetExtendedAttribute("OverdueMinutes", value.ToString()); }
        }

        /// <summary>
        /// 投票次数限制
        /// </summary>
        [JsonIgnore]
        public int VoteTimes
        {
            get { return GetInt("VoteTimes", 0); }
            set { SetExtendedAttribute("VoteTimes", value.ToString()); }
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