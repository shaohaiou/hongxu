using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Hx.Components.Entity;
using Hx.Components.Query;
using Hx.Components.Enumerations;
using Hx.Components.Providers;
using Hx.Tools.Web;
using Hx.Components.Config;
using Hx.Tools;

namespace Hx.Components
{
    public class EventLogs
    {
        private EventLogs() { }


        public static List<EventLogEntry> GetList(int pageindex, int pagesize, EventLogQuery query, out int total)
        {
            return CommonDataProvider.Instance().GetEventLogs(pageindex, pagesize, query, out total);
        }

        #region  全局变量

        /// <summary>
        /// job事件ID起始编号
        /// </summary>
        public const int EVENTID_JOB = 1000;

        /// <summary>
        /// job警告事件ID起始编号
        /// </summary>
        public const int EVENTID_JOB_WARN = 1200;

        /// <summary>
        /// job调试事件ID起始编号
        /// </summary>
        public const int EVENTID_JOB_DEBUG = 1400;

        /// <summary>
        /// job错误事件ID起始编号
        /// </summary>
        public const int EVENTID_JOB_ERROR = 1600;

        /// <summary>
        /// task事件ID起始编号
        /// </summary>
        public const int EVENTID_TASK = 2000;

        /// <summary>
        /// task警告事件ID起始编号
        /// </summary>
        public const int EVENTID_TASK_WARN = 2200;

        /// <summary>
        /// task调试事件ID起始编号
        /// </summary>
        public const int EVENTID_TASK_DEBUG = 2400;

        /// <summary>
        /// task错误事件ID起始编号
        /// </summary>
        public const int EVENTID_TASK_ERROR = 2600;

        /// <summary>
        /// web事件ID起始编号
        /// </summary>
        public const int EVENTID_WEB = 3000;

        /// <summary>
        /// web警告事件ID起始编号
        /// </summary>
        public const int EVENTID_WEB_WARN = 3200;

        /// <summary>
        /// web调试事件ID起始编号
        /// </summary>
        public const int EVENTID_WEB_DEBUG = 3400;

        /// <summary>
        /// web错误事件ID起始编号
        /// </summary>
        public const int EVENTID_WEB_ERROR = 3600;

        /// <summary>
        /// 索引事件ID起始编号
        /// </summary>
        public const int EVENTID_INDEX = 4000;

        /// <summary>
        /// 索引警告事件ID起始编号
        /// </summary>
        public const int EVENTID_INDEX_WARN = 4200;

        /// <summary>
        /// 索引调试事件ID起始编号
        /// </summary>
        public const int EVENTID_INDEX_DEBUG = 4400;

        /// <summary>
        /// 索引错误事件ID起始编号
        /// </summary>
        public const int EVENTID_INDEX_ERROR = 4600;

        /// <summary>
        /// 其他事件ID起始编号
        /// </summary>
        public const int ENENTID_OTHER = 5000;

        #endregion

        #region TaskAndJob log

        public static void TaskLog(int id, int toolid, string message)
        {
            TaskWrite(message, 0, id, toolid, EventType.Information);
        }

        public static void TaskWarn(string message, int eventID, int entryid, int id, Exception ex)
        {
            TaskWrite(string.Format("{0}\n\n{1}\n{2}", message, ex.GetType(), ex.ToString()), eventID, entryid, id, EventType.Warning);
        }

        public static void TaskError(string message, int eventID, int entryid, int appid, Exception ex)
        {
            TaskWrite(string.Format("{0}\n\n{1}\n{2}", message, ex.GetType(), ex.ToString()), eventID, entryid, appid, EventType.Error, ex);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void TaskDebug(string message, int eventID, int entryid, int appid)
        {
            TaskWrite(message, eventID, entryid, appid, EventType.Debug);
        }

        public static void TaskWrite(string message, int eventID, int entryid, int appid, EventType etype, Exception ex = null)
        {
            EventLogEntry entry = new EventLogEntry();
            entry.Message = message;
            entry.Category = "Task";
            entry.EventDate = DateTime.Now;
            entry.EventID = eventID;
            entry.EventType = etype;
            entry.ApplicationID = appid;
            entry.ApplicationType = ApplicationType.Tools;
            entry.EntryID = entryid;
            entry.Ex = ex;
            if (string.IsNullOrEmpty(entry.MachineName))
            {
                entry.MachineName = Environment.MachineName;
            }
            if (!string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = WebHelper.HtmlEncode(entry.Message);
            }
            if (string.IsNullOrEmpty(entry.ApplicationName))
            {
                entry.ApplicationName = "工具";
            }
            Write(entry);
        }

        public static void JobLog(string message)
        {
            JobWrite(message, 0, 0, EventType.Information);
        }

        public static void JobWarn(string message, int eventID, int entryid, int appid, Exception ex)
        {
            TaskWrite(string.Format("{0}\n\n{1}\n{2}", message, ex.GetType(), ex.ToString()), eventID, entryid, appid, EventType.Warning);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void JobDebug(string message, int eventID, int entryid, int appid)
        {
            TaskWrite(message, eventID, entryid, appid, EventType.Debug);
        }

        public static void JobError(string message, int eventID, int appid, Exception ex)
        {
            JobWrite(string.Format("{0}\n\n{1}\n{2}", message, ex.GetType(), ex.ToString()), eventID, appid, EventType.Error);
        }

        public static void JobWrite(string message, int eventID, int appid, EventType etype)
        {
            EventLogEntry entry = new EventLogEntry();
            entry.Message = message;
            entry.Category = "Job";
            entry.EventID = eventID;
            entry.EventType = etype;
            entry.EventDate = DateTime.Now;
            entry.ApplicationID = appid;
            if (appid > 0)
            {
                entry.ApplicationType = ApplicationType.Tools;
            }
            else
            {
                entry.ApplicationType = ApplicationType.NoSet;
            }
            entry.EntryID = 0;
            if (string.IsNullOrEmpty(entry.MachineName))
            {
                entry.MachineName = Environment.MachineName;
            }
            if (!string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = WebHelper.HtmlEncode(entry.Message);
            }
            CommConfig config = CommConfig.GetConfig();
            if (!string.IsNullOrEmpty(config.AppSetting["ApplicationName"]))
            {
                entry.ApplicationName = config.AppSetting["ApplicationName"];
            }
            if (!string.IsNullOrEmpty(config.AppSetting["ApplicationID"]))
            {
                entry.ApplicationID = DataConvert.SafeInt(config.AppSetting["ApplicationID"]);
            }
            //Write(entry);
        }

        #endregion

        #region Web log

        public static void WebLog(string message)
        {
            WebWrite(message, 1, 0, 0, EventType.Information);
        }

        public static void WebError(string message, int eventID, int siteid, Exception ex = null)
        {
            WebWrite(message, eventID, 0, siteid, EventType.Error, ex);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WebDebug(string message, int eventID, int siteid)
        {
            WebWrite(message, eventID, 0, siteid, EventType.Debug);
        }

        public static void WebWarn(string message, int eventID, int siteid)
        {
            WebWrite(message, eventID, 0, siteid, EventType.Warning);
        }

        public static void WebWrite(string message, int eventID, int entryid, int appid, EventType etype, Exception ex = null)
        {
            EventLogEntry entry = new EventLogEntry();
            entry.Message = message;
            entry.Category = "Web";
            entry.EventID = eventID;
            entry.EventDate = DateTime.Now;
            entry.EventType = etype;
            entry.ApplicationID = appid;
            entry.ApplicationType = ApplicationType.WebSite;
            entry.EntryID = entryid;
            entry.Ex = ex;
            if (string.IsNullOrEmpty(entry.MachineName))
            {
                entry.MachineName = Environment.MachineName;
            }
            if (!string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = WebHelper.HtmlEncode(entry.Message);
            }
            if (string.IsNullOrEmpty(entry.ApplicationName))
            {
                entry.ApplicationName = "网站";
            }
            Write(entry);
        }

        #endregion

        #region 工具日志

        public static void ToolsLog(string message)
        {
            ToolsWrite(message, 0, 0, EventType.Information);
        }

        public static void ToolsError(string message, int appid, Exception ex)
        {
            ToolsWrite(message, 1, appid, EventType.Error, ex);
        }

        public static void ToolsWrite(string message, int eventID, int appid, EventType etype, Exception ex = null)
        {
            EventLogEntry entry = new EventLogEntry();
            entry.Message = message;
            entry.Category = "工具";
            entry.EventID = eventID;
            entry.EventDate = DateTime.Now;
            entry.EventType = etype;
            entry.ApplicationID = appid;
            entry.ApplicationType = ApplicationType.Tools;
            entry.EntryID = 0;
            entry.Ex = ex;
            if (string.IsNullOrEmpty(entry.MachineName))
            {
                entry.MachineName = Environment.MachineName;
            }
            if (!string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = WebHelper.HtmlEncode(entry.Message);
            }
            if (string.IsNullOrEmpty(entry.ApplicationName))
            {
                entry.ApplicationName = "工具";
            }

            Write(entry);
        }

        #endregion

        public static void Write(EventLogEntry entry)
        {

            try
            {
                if (entry.ApplicationType != ApplicationType.NoSet && (entry.EventType == EventType.Information || entry.EventType == EventType.Debug))
                {
                    entry.Uniquekey = "";
                    CommConfig config = CommConfig.GetConfig();
                    CommonDataProvider.Instance().WriteEventLogEntry(entry);
                    return;
                }
                string key = string.Empty;
                if (entry.EventType == EventType.Warning)
                {
                    key = GlobalKey.EVENTLOG_KEY + "_" + string.Format("{0}-{1}-{2}-{3}", entry.ApplicationID, entry.EntryID, entry.EventID, (int)entry.EventType);
                }
                else if (entry.EventType == EventType.Error && entry.Ex != null)
                {
                    key = GlobalKey.EVENTLOG_KEY + "_" + string.Format("{0}-{1}-{2}-{3}-{4}-{5}", entry.ApplicationID, entry.EntryID, entry.EventID, (int)entry.EventType, entry.Ex.StackTrace.GetHashCode(), entry.Ex.Message.GetHashCode());
                    entry.Message = entry.Ex.Message + "-" + entry.Ex.StackTrace + "\r\n" + entry.Message;
                }

                EventLogEntry oldentry = MangaCache.Get(key) as EventLogEntry;

                if (oldentry != null)
                {
                    oldentry.PCount++;
                    oldentry.LastUpdateTime = DateTime.Now;
                }
                else
                {
                    DateTime time = DateTime.Now;
                    entry.PCount = 1;
                    entry.Uniquekey = key;
                    entry.AddTime = time;
                    entry.LastUpdateTime = time;
                    CommConfig config = CommConfig.GetConfig();
                    CommonDataProvider.Instance().WriteEventLogEntry(entry);
                    entry.PCount = 0;
                    MangaCache.Add(key, entry, 30, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.AboveNormal, new CacheItemRemovedCallback(RemovedCallback));
                }

            }
            catch { }
        }

        public static void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
        {
            try
            {
                EventLogEntry entry = v as EventLogEntry;
                if (entry != null && entry.AddTime != entry.LastUpdateTime)
                {
                    CommConfig config = CommConfig.GetConfig();
                    CommonDataProvider.Instance().WriteEventLogEntry(entry);
                }
            }
            catch { }
        }

    }
}
