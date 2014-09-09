using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Car.Entity
{
    /// <summary>
    /// 商业保险
    /// </summary>
    [Serializable]
    public class SybxInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return GetString("Name", ""); }
            set { SetExtendedAttribute("Name", value); }
        }
    }
}
