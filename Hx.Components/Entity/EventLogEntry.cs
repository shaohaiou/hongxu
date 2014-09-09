using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    //事件类型
    public enum EventType
    {
        [Description("全部")]
        Entire = -1,//全部
        [Description("信息")]
        Information = 0,//信息
        [Description("警告")]
        Warning = 1,//警告
        [Description("错误")]
        Error = 2,//错误
        [Description("调试信息")]
        Debug = 3//调试信息
    }

    /// <summary>
    /// 事件日志实体类
    /// </summary>
    [Serializable]
    public class EventLogEntry
    {
        /// <summary>
        /// 信息
        /// </summary>
        [JsonProperty("message")]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 机器名
        /// </summary>
        [JsonProperty("machinename")]
        public string MachineName
        {
            get;
            set;
        }

        /// <summary>
        /// 分类
        /// </summary>
        [JsonProperty("category")]
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        [JsonProperty("entryid")]
        public int EntryID
        {
            get;
            set;
        }

        /// <summary>
        /// 事件ID
        /// </summary>
        [JsonProperty("eventid")]
        public int EventID
        {
            get;
            set;
        }

        /// <summary>
        /// 事件日期
        /// </summary>
        [JsonProperty("eventdate")]
        public DateTime EventDate
        {
            get;
            set;
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonProperty("eventtype")]
        public EventType EventType
        {
            get;
            set;
        }

        /// <summary>
        /// 应用程序名
        /// </summary>
        [JsonProperty("applicationname")]
        public string ApplicationName { get; set; }

        /// <summary>
        /// 应用程序ID
        /// </summary>
        [JsonProperty("applicationid")]
        public int ApplicationID { get; set; }

        /// <summary>
        /// 应用程序类型
        /// </summary>
        [JsonProperty("applicationtype")]
        public ApplicationType ApplicationType { get; set; }
        [JsonProperty("pcount")]
        public int PCount { get; set; }

        [JsonProperty("addtime")]
        public DateTime AddTime { get; set; }

        [JsonProperty("lastupdatetime")]
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 相关错误
        /// </summary>
        public Exception Ex { get; set; }

        [JsonProperty("uniquekey")]
        public string Uniquekey { get; set; }
    }
}
