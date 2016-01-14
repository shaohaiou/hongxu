using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    public class QuestionInfo
    {
        public int ID { get; set; }
        public QuestionItem QuestionItem { get; set; }
        public string QuestionFacor { get; set; }
        public QuestionType QuestionType { get; set; }
        public string QuestionIntroduce { get; set; }
    }
}
