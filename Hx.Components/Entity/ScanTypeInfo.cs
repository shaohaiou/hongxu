using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    public class ScanTypeInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int ValidAreaXTop { get; set; }

        public int ValidAreaYTop { get; set; }

        public int ValidAreaXBottom { get; set; }

        public int ValidAreaYBottom { get; set; }

        public string CorpPower { get; set; }
    }
}
