using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class JobOfferInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        
        /// <summary>
        /// 扩展预留字段
        /// </summary>
        [JsonIgnore]
        public string Ext
        {
            get { return GetString("Ext", ""); }
            set { SetExtendedAttribute("Ext", value); }
        }
    }
}
