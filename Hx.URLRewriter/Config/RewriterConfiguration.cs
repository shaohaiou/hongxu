using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Hx.Components;
using Hx.Components.Web;
using Hx.Components.Enumerations;

namespace Hx.URLRewriter.Config
{
    [Serializable]
    [XmlRoot("RewriterConfig")]
    public class RewriterConfiguration
    {
        // private member variables
        private RewriterRuleCollection rules;			// an instance of the RewriterRuleCollection class...

        /// <summary>
        /// GetConfig() returns an instance of the <b>RewriterConfiguration</b> class with the values populated from
        /// the Web.config file.  It uses XML deserialization to convert the XML structure in Web.config into
        /// a <b>RewriterConfiguration</b> instance.
        /// </summary>
        /// <returns>A <see cref="RewriterConfiguration"/> instance.</returns>
        public static RewriterConfiguration GetConfig()
        {
            string key = GlobalKey.REWRITER_KEY + (int)HXContext.Current.UrlRuleType;
            RewriterConfiguration config = MangaCache.Get(key) as RewriterConfiguration;
            if (config == null)
            {
                config = new RewriterConfiguration();
                RewriterRules rulebll = new RewriterRules();
                RewriterRuleCollection rules = rulebll.GetRules(HXContext.Current.UrlRuleType);
                config.Rules = rules;
                MangaCache.Max(key, config);
            }
            return config;
        }

        public static void ReloadConfig(UrlRuleType urlrule)
        {
            string key = GlobalKey.REWRITER_KEY + (int)urlrule;

            RewriterConfiguration config = new RewriterConfiguration();
            RewriterRules rulebll = new RewriterRules();
            RewriterRuleCollection rules = rulebll.GetRules(urlrule);
            config.Rules = rules;
            MangaCache.Max(key, config);
        }

        #region Public Properties
        /// <summary>
        /// A <see cref="RewriterRuleCollection"/> instance that provides access to a set of <see cref="RewriterRule"/>s.
        /// </summary>
        public RewriterRuleCollection Rules
        {
            get
            {
                return rules;
            }
            set
            {
                rules = value;
            }
        }
        #endregion
    }
}
