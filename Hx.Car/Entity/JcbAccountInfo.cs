using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Car.Enum;

namespace Hx.Car.Entity
{
    [Serializable]
    public class JcbAccountInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("UserID")]
        public int UserID { get; set; }

        [JsonProperty("AccountName")]
        public string AccountName { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("JcbSiteType")]
        public JcbSiteType JcbSiteType { get; set; }

        [JsonProperty("JcbSiteType")]
        public DateTime AddTime { get; set; }

        [JsonProperty("JcbAccountType")]
        public JcbAccountType JcbAccountType { get; set; }


        /// <summary>
        /// 帐号类型
        /// </summary>
        [JsonIgnore]
        public string Ext
        {
            get { return GetString("ext", ""); }
            set { SetExtendedAttribute("ext", value); }
        }
    }
}
