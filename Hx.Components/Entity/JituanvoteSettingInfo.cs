using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class JituanvoteSettingInfo : ExtendedAttributes
    {
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
        /// 活动须知
        /// </summary>
        [JsonIgnore]
        public string MustKnow
        {
            get { return GetString("MustKnow", ""); }
            set { SetExtendedAttribute("MustKnow", value); }
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
