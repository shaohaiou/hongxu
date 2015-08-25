using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class ScenecodeSettingInfo : ExtendedAttributes
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
    }
}
