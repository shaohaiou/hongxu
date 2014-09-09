using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.URLRewriter.Config;
using Hx.Components.Config;
using Hx.Tools;
using System.Data.SqlClient;
using Hx.Components.Data;
using System.Data;

namespace Hx.URLRewriter
{
    public class MSSQLRewriterDateProvider : RewriterDateProvider
    {
        private string _con;
        private string _dbowner;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="constr"></param>
        /// <param name="owner"></param>
        public MSSQLRewriterDateProvider(string constr, string owner)
        {
            CommConfig config = CommConfig.GetConfig();
            _con = EncryptString.DESDecode(constr, config.AppSetting["key"]);
            _dbowner = owner;
        }

        public override int Add(RewriterRule rule)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO [dbo].[HX_RewriterRule]([RuleType],[LookFor],[SendTo],[Sort],[Description])");
            strSql.Append(" VALUES (@RuleType,@LookFor,@SendTo,@Sort,@Description)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@RuleType", (int)rule.RuleType);
            parameters[1] = new SqlParameter("@LookFor", rule.LookFor);
            parameters[2] = new SqlParameter("@SendTo", rule.SendTo);
            parameters[3] = new SqlParameter("@Sort", rule.Sort);
            parameters[4] = new SqlParameter("@Description", rule.Description);
            return DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, strSql.ToString(), parameters), -2);
        }

        public override void Update(RewriterRule rule)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE [dbo].[HX_RewriterRule] SET [RuleType] = @RuleType,[LookFor] = @LookFor,[SendTo] = @SendTo,[Sort]=@Sort,[Description]=@Description WHERE ID=@ID");
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@RuleType", (int)rule.RuleType);
            parameters[1] = new SqlParameter("@LookFor", rule.LookFor);
            parameters[2] = new SqlParameter("@SendTo", rule.SendTo);
            parameters[3] = new SqlParameter("@ID", rule.ID);
            parameters[4] = new SqlParameter("@Sort", rule.Sort);
            parameters[5] = new SqlParameter("@Description", rule.Description);
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sb.ToString(), parameters);
        }

        public override void Delete(int id)
        {
            string sql = "DELETE FROM [dbo].[HX_RewriterRule] where ID=@ID";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, new SqlParameter("@ID", id));
        }

        public override RewriterRuleCollection GetRules(Hx.Components.Enumerations.UrlRuleType type)
        {
            string sql = "SELECT [ID],[RuleType],[LookFor],[SendTo],[Sort],[Description] FROM [dbo].[HX_RewriterRule] where RuleType=@RuleType order by Sort asc";
            RewriterRuleCollection rules = new RewriterRuleCollection();
            RewriterRule rule = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@RuleType", (int)type)))
            {
                while (reader.Read())
                {
                    rule = PopulateRewriterRule(reader);
                    rules.Add(rule);
                }
            }
            return rules;
        }

        public override RewriterRule GetRule(int id)
        {
            string sql = "SELECT [ID],[RuleType],[LookFor],[SendTo],[Sort],[Description] FROM [dbo].[HX_RewriterRule] where ID=@ID";
            RewriterRule rule = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@ID", id)))
            {
                if (reader.Read())
                {
                    rule = PopulateRewriterRule(reader);
                }
            }
            return rule;
        }

        public override RewriterRule GetRuleByType(Hx.Components.Enumerations.UrlRuleType type)
        {
            string sql = "SELECT top 1 [ID],[RuleType],[LookFor],[SendTo],[Sort],[Description] FROM [dbo].[HX_RewriterRule] where RuleType=@RuleType order by Sort asc";
            RewriterRule rule = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@RuleType", (int)type)))
            {
                if (reader.Read())
                {
                    rule = PopulateRewriterRule(reader);
                }
            }
            return rule; throw new NotImplementedException();
        }

        public override void UpSort(List<int> ids, List<int> sorts)
        {
            if (ids.Count > 0)
            {
                if (ids.Count != sorts.Count)
                {
                    return;
                }
                string sql = "update HX_RewriterRule set Sort=@Sort where ID=@ID";
                List<CommandInfo> commands = new List<CommandInfo>();
                SqlParameter[] parameters = null;

                for (int i = 0; i < ids.Count; i++)
                {

                    parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter("@ID", ids[i]);
                    parameters[1] = new SqlParameter("@Sort", sorts[i]);
                    commands.Add(new CommandInfo(sql, parameters, CommandType.Text));
                }
                SqlHelper.ExecuteSqlTran(_con, commands);
            }
        }

        public override List<RewriterRule> GetAllRules()
        {
            string sql = "SELECT [ID],[RuleType],[LookFor],[SendTo],[Sort],[Description] FROM [dbo].[HX_RewriterRule] order by Sort asc";
            List<RewriterRule> rules = new List<RewriterRule>();
            RewriterRule rule = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    rule = PopulateRewriterRule(reader);
                    rules.Add(rule);
                }
            }
            return rules;
        }
    }
}
