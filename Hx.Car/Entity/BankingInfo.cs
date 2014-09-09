using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Car.Entity
{
    /// <summary>
    /// 金融方案
    /// </summary>
    [Serializable]
    public class BankingInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// 银行id
        /// </summary>
        [JsonProperty("bankid")]
        public int BankID { get; set; }
    }
}
