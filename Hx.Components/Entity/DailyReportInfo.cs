using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    [Serializable]
    public class DailyReportInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("dayunique")]
        public string DayUnique { get; set; }

        [JsonProperty("corporationid")]
        public int CorporationID { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("createtime")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("lastupdateuser")]
        public string LastUpdateUser { get; set; }

        [JsonProperty("lastupdatetime")]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 日报数据
        /// </summary>
        [JsonIgnore]
        public string SCReport
        {
            get { return GetString("SCReport", ""); }
            set { SetExtendedAttribute("SCReport", value); }
        }
    }
}
