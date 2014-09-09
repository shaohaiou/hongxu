using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Hx.Components.Enumerations;

namespace Hx.URLRewriter.Config
{
    [Serializable()]
    public class RewriterRule
    {
        // private member variables...
        private string lookFor, sendTo, description;
        private int id, sort;
        private UrlRuleType ruletype = UrlRuleType.NoSet;

        #region 公共属性
        /// <summary>
        /// Gets or sets the pattern to look for.
        /// </summary>
        /// <remarks><b>LookFor</b> is a regular expression pattern.  Therefore, you might need to escape
        /// characters in the pattern that are reserved characters in regular expression syntax (., ?, ^, $, etc.).
        /// <p />
        /// The pattern is searched for using the <b>System.Text.RegularExpression.Regex</b> class's <b>IsMatch()</b>
        /// method.  The pattern is case insensitive.</remarks>
        public string LookFor
        {
            get
            {
                return lookFor;
            }
            set
            {
                lookFor = value;
            }
        }

        /// <summary>
        /// The string to replace the pattern with, if found.
        /// </summary>
        /// <remarks>The replacement string may use grouping symbols, like $1, $2, etc.  Specifically, the
        /// <b>System.Text.RegularExpression.Regex</b> class's <b>Replace()</b> method is used to replace
        /// the match in <see cref="LookFor"/> with the value in <b>SendTo</b>.</remarks>
        public string SendTo
        {
            get
            {
                return sendTo;
            }
            set
            {
                sendTo = value;
            }
        }
        //[BsonId]
        [XmlIgnore]
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }


        [XmlIgnore]
        public UrlRuleType RuleType
        {
            get
            {
                return ruletype;
            }
            set
            {
                ruletype = value;
            }
        }


        [XmlIgnore]
        public int Sort
        {
            get
            {
                return sort;
            }
            set
            {
                sort = value;
            }
        }

        [XmlIgnore]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        #endregion
    }
}
