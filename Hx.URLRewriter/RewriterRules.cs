using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.URLRewriter.Config;
using Hx.Components.Enumerations;

namespace Hx.URLRewriter
{
    public class RewriterRules
    {
        public int Add(RewriterRule rule)
        {
            return RewriterDateProvider.Instance().Add(rule);
        }

        public void Update(RewriterRule rule)
        {
            RewriterDateProvider.Instance().Update(rule);
        }

        public void Delete(int id)
        {
            RewriterDateProvider.Instance().Delete(id);
        }

        public RewriterRuleCollection GetRules(UrlRuleType type)
        {
            return RewriterDateProvider.Instance().GetRules(type);
        }

        public RewriterRule GetRule(int id)
        {
            return RewriterDateProvider.Instance().GetRule(id);
        }

        public RewriterRule GetRuleByType(UrlRuleType type)
        {
            return RewriterDateProvider.Instance().GetRuleByType(type);
        }

        public void UpSort(List<int> ids, List<int> sorts)
        {
            RewriterDateProvider.Instance().UpSort(ids, sorts);
        }
    }
}
