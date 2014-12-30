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
        /// 引导框图片
        /// </summary>
        [JsonIgnore]
        public string PicPath
        {
            get { return GetString("PicPath", ""); }
            set { SetExtendedAttribute("PicPath", value); }
        }


    }
}
