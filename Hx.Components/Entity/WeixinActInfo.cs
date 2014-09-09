using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    /// <summary>
    /// 微信活动
    /// </summary>
    [Serializable]
    public class WeixinActInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("Openid")]
        public string Openid { get; set; }

        [JsonProperty("Nickname")]
        public string Nickname { get; set; }

        [JsonProperty("Sex")]
        public byte Sex { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("Subscribetime")]
        public string Subscribetime { get; set; }

        [JsonProperty("AddTime")]
        public DateTime AddTime { get; set; }

        [JsonProperty("AtcValue")]
        public int AtcValue { get; set; }
    }
}
