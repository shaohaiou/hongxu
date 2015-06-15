using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class VoteRecordInfo : ExtendedAttributes
    {
        public int ID { get; set; }

        public int SID { get; set; }

        /// <summary>
        /// 选手ID
        /// </summary>
        public int AthleteID { get; set; }

        /// <summary>
        /// 选手姓名
        /// </summary>
        public string AthleteName { get; set; }

        /// <summary>
        /// 选手序号
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 投票人
        /// </summary>
        public string Voter { get; set; }

        /// <summary>
        /// 投票时间
        /// </summary>
        public DateTime AddTime { get; set; }

        [JsonIgnore]
        public string Openid
        {
            get { return GetString("Openid", ""); }
            set { SetExtendedAttribute("Openid", value); }
        }

        [JsonIgnore]
        public string Nickname
        {
            get { return GetString("Nickname", ""); }
            set { SetExtendedAttribute("Nickname", value); }
        }

        [JsonIgnore]
        public int Sex
        {
            get { return GetInt("Sex", 0); }
            set { SetExtendedAttribute("Sex", value.ToString()); }
        }

        [JsonIgnore]
        public string City
        {
            get { return GetString("City", ""); }
            set { SetExtendedAttribute("City", value); }
        }

        [JsonIgnore]
        public string Country
        {
            get { return GetString("Country", ""); }
            set { SetExtendedAttribute("Country", value); }
        }

        [JsonIgnore]
        public string Province
        {
            get { return GetString("Province", ""); }
            set { SetExtendedAttribute("Province", value); }
        }
    }
}
