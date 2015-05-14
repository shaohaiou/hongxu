using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    public class CardidlistpackInfo
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public List<string> card_id_list { get; set; }

        public int total_num { get; set; }
    }
}
