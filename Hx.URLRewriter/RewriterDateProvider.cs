using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Config;
using Hx.URLRewriter.Config;
using Hx.Components;
using System.Data;
using Hx.Tools;

namespace Hx.URLRewriter
{
    public abstract class RewriterDateProvider
    {
        private static RewriterDateProvider _defaultprovider = null;
        private static object _lock = new object();
        private static readonly string DEFAULT_PROVDIER_REWRITER = "MSSQLRewriterDateProvider";

        /// <summary>
        /// 返回默认的数据提供者类
        /// </summary>
        /// <returns></returns>
        public static RewriterDateProvider Instance()
        {
            LoadDefaultProviders();
            return _defaultprovider;
        }

        /// <summary>
        ///从配置文件加载默认数据库访问提供者类
        /// </summary>
        private static void LoadDefaultProviders()
        {
            if (_defaultprovider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_defaultprovider == null)
                    {
                        CommConfig config = CommConfig.GetConfig();
                        Provider dataProvider = (Provider)config.Providers[DEFAULT_PROVDIER_REWRITER];
                        _defaultprovider = DataProvider.Instance(dataProvider) as RewriterDateProvider;

                    }
                }
            }
        }

        public abstract int Add(RewriterRule rule);

        public abstract void Update(RewriterRule rule);

        public abstract void Delete(int id);

        public abstract RewriterRuleCollection GetRules(Hx.Components.Enumerations.UrlRuleType type);

        public abstract RewriterRule GetRule(int id);

        public abstract RewriterRule GetRuleByType(Hx.Components.Enumerations.UrlRuleType type);

        public abstract void UpSort(List<int> ids, List<int> sorts);

        public abstract List<RewriterRule> GetAllRules();

        public static RewriterRule PopulateRewriterRule(IDataReader reader)
        {
            RewriterRule rewriter = new RewriterRule();
            rewriter.ID = DataConvert.SafeInt(reader["ID"]);
            rewriter.LookFor = reader["LookFor"] as string;
            rewriter.SendTo = reader["SendTo"] as string;
            rewriter.RuleType = (Hx.Components.Enumerations.UrlRuleType)(byte)reader["RuleType"];
            rewriter.Sort = DataConvert.SafeInt(reader["Sort"]);
            rewriter.Description = reader["Description"].ToString();
            return rewriter;
        }
    }
}
