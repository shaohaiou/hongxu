using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    public class CardPullRecordInfo
    {
        public int ID { get; set; }

        public int SID { get; set; }

        public string Openid { get; set; }

        public string UserName { get; set; }

        public string Cardid { get; set; }

        public string Cardtitle { get; set; }

        public string Cardawardname { get; set; }

        public string Cardlogourl { get; set; }

        public string PullResult { get; set; }

        public DateTime AddTime { get; set; }
    }
}
