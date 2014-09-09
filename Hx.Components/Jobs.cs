using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using Hx.Components.Config;
using System.Xml;
using Hx.Components.Entity;

namespace Hx.Components
{
    /// <summary>
    /// 定时任务管理类
    /// </summary>
    public class Jobs
    {
        private static readonly Jobs _jobs = null;//单例对象
        private Hashtable jobList = new Hashtable();//执行任务的容器
        private int Interval = -1;//延迟执行时间
        private Timer singleTimer = null;//定时执行
        private DateTime _created;//任务建立
        private DateTime _started;//任务开始
        private DateTime _completed;//任务完成
        private bool _isRunning;//是否正在运行

        #region 初始化

        static Jobs()
        {
            _jobs = new Jobs();//创建单个实例
        }

        /// <summary>
        /// 返回单个对象（单例模式）
        /// </summary>
        /// <returns></returns>
        public static Jobs Instance()
        {
            return _jobs;
        }

        /// <summary>
        ///不允许创建
        /// </summary>
        private Jobs()
        {
            _created = DateTime.Now;
        }

        #endregion

        //当前任务
        public Hashtable CurrentJobs
        {
            get { return jobList; }
        }

        //获取任务运行信息
        public override string ToString()
        {
            return string.Format("任务创建时间: {0}, 上个任务开始时间: {1},上个任务结束时间 : {2}, 任务是否正在运行: {3}, 延迟时间: {4}", _created, _started, _completed, _isRunning, Interval / 60000);
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public System.Collections.Specialized.ListDictionary CurrentStats
        {
            get
            {
                System.Collections.Specialized.ListDictionary stats = new System.Collections.Specialized.ListDictionary();
                stats.Add("Created", _created);
                stats.Add("LastStart", _started);
                stats.Add("LastStop", _completed);
                stats.Add("IsRunning", _isRunning);
                stats.Add("Minutes", Interval / 60000);
                return stats;
            }
        }

        /// <summary>
        /// 查找并开始执行任务，已经存在的任务停止
        /// </summary>
        public void Start()
        {
            if (jobList.Count != 0)
                return;
            CommConfig config = CommConfig.GetConfig();
            if (!config.IsDisableBackgroundThreads)
            {
                XmlNode node = config.GetConfigSection("common/jobs");
                if (node == null)
                {
                    return;
                }
                bool isSingleThread = true;//是否单线程运行
                XmlAttribute singleThreadAttribute = node.Attributes["singleThread"];
                if (singleThreadAttribute != null && !string.IsNullOrEmpty(singleThreadAttribute.Value) && string.Compare(singleThreadAttribute.Value, "false", true) == 0)
                    isSingleThread = false;
                else
                {
                    isSingleThread = true;
                    XmlAttribute millisecond = node.Attributes["millisecond"];
                    if (millisecond != null && !string.IsNullOrEmpty(millisecond.Value))
                    {
                        try
                        {
                            Interval = Int32.Parse(millisecond.Value);
                        }
                        catch
                        {
                            Interval = -1;
                        }
                    }
                    XmlAttribute seconds = node.Attributes["seconds"];
                    if (Interval < 0 && seconds != null && !string.IsNullOrEmpty(seconds.Value))
                    {
                        try
                        {
                            Interval = Int32.Parse(seconds.Value) * 1000;
                        }
                        catch
                        {
                            Interval = -1;
                        }
                    }
                    XmlAttribute minutes = node.Attributes["minutes"];
                    if (Interval < 0 && minutes != null && !string.IsNullOrEmpty(minutes.Value))
                    {
                        try
                        {
                            Interval = Int32.Parse(minutes.Value) * 60000;
                        }
                        catch
                        {
                            Interval = 15 * 60000;
                        }
                    }
                    if (Interval == -1)
                    {
                        Interval = 15 * 60000;
                    }

                }

                //创建任务
                foreach (XmlNode jnode in node.ChildNodes)
                {
                    if (jnode.NodeType != XmlNodeType.Comment)
                    {
                        XmlAttribute typeAttribute = jnode.Attributes["type"];
                        XmlAttribute nameAttribute = jnode.Attributes["name"];

                        Type type = Type.GetType(typeAttribute.Value);
                        if (type != null)
                        {
                            if (!jobList.Contains(nameAttribute.Value))
                            {
                                Job j = new Job(type, jnode);
                                jobList[nameAttribute.Value] = j;
                                if (!isSingleThread || !j.SingleThreaded)//多个线程运行任务
                                {
                                    j.PostJob += new EventHandler(PostJob);
                                    j.InitializeTimer();
                                }
                            }
                        }
                    }
                }
                if (isSingleThread)
                {
                    //如果是单线程运行则创建单个定时器运行任务
                    singleTimer = new Timer(new TimerCallback(call_back), null, Interval, Interval);
                }
            }
        }

        /// <summary>
        /// 单线程运行回调函数
        /// </summary>
        /// <param name="state"></param>
        private void call_back(object state)
        {
            _isRunning = true;//正在运行
            _started = DateTime.Now;//开始时间
            singleTimer.Change(Timeout.Infinite, Timeout.Infinite);//设置定时器的回调函数不会被调用

            //遍历执行
            foreach (Job job in jobList.Values)
                if (job.Enabled && job.SingleThreaded)
                {
                    if (job.IsFirstRun)
                    {
                        job.PostJob += new EventHandler(PostJob);
                    }
                    job.ExecuteJob();
                }
            singleTimer.Change(Interval, Interval);//重新设置定时器的回调函数
            _isRunning = false;//停止运行
            _completed = DateTime.Now;//完成时间
        }

        /// <summary>
        /// 完成任务时执行
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void PostJob(object o, EventArgs args)
        {
            Job job = o as Job;
            //EventLogs.Info(string.Format("{0}任务执行完成", job.Name), "任务事件", 302);//写入日志信息
        }

        /// <summary>
        /// 开始任务时执行
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void PreJob(object o, EventArgs args)
        {
            Job job = o as Job;
            //EventLogs.Info(string.Format("{0}任务开始启动", job.Name), "任务事件", 301);//写入日志信息

        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public void Stop()
        {
            if (jobList != null)
            {
                foreach (Job job in jobList.Values)
                    job.Dispose();
                jobList.Clear();

                if (singleTimer != null)
                {
                    singleTimer.Dispose();
                    singleTimer = null;
                }
            }
        }

        /// <summary>
        /// 检查任务是否在任务列表里
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <returns>bool</returns>
        public bool IsJobEnabled(string jobName)
        {
            if (!jobList.Contains(jobName))
                return false;
            return ((Job)jobList[jobName]).Enabled;
        }
    }
}
