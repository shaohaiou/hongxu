using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    public class QuestionRecordInfo
    {
        public int ID { get; set; }

        public string PostUser { get; set; }

        public string PostUserName { get; set; }

        public DateTime PostTime { get; set; }

        public QuestionType QuestionType { get; set; }

        public int QuestionCompanyID { get; set; }

        public List<QuestionScoreInfo> QuestionScoreList { get; set; }

        public string QuestionScoreInfoListJson { get; set; }
    }
}
