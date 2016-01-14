using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class QuestionScoreInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("qid")]
        public int QuestionID { get; set; }

        [JsonProperty("qfactor")]
        public string QuestionFactor { get; set; }

        [JsonProperty("score")]
        public decimal Score { get; set; }

    }
}
