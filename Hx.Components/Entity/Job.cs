using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Hx.Components.Interface;
using System.Threading;
using System.Xml;

namespace Hx.Components.Entity
{
    [Serializable]
    [XmlRoot("job")]
    public class Job : IDisposable
    {
        public event EventHandler PreJob;
        public event EventHandler PostJob;

        #region 私用成员
        private IJob _ijob;
        private bool _enabled = true;//是否允许运行，默认允许
        private Type _jobType;//任务类型
        private string _name;//任务类名
        private bool _enableShutDown = false;// 发生异常是否允许停止
        private bool disposed = false;
        private Timer _timer = null;//定时执行器
        private XmlNode _node = null;
        private bool _singleThread = true;//是否单一线程中执行
        private DateTime _lastStart;//上次开始时间
        private DateTime _lastSucess;//上次成功时间
        private DateTime _lastEnd;//上次结束时间
        private bool _isRunning;//是否正在运行
        private int _seconds = -1;//运行间隔秒单位
        private int _minutes = 15;//运行间隔分钟单位
        private int _millisecond = -1;//运行的毫秒单位
        private bool _isFirstRun = true;

        /// <summary>
        /// 间隔时间
        /// </summary>
        protected int Interval
        {
            get
            {

                if (_millisecond > 0)
                    return _millisecond;
                if (_seconds > 0)
                    return _seconds * 1000;
                return Minutes * 60000;
            }
        }

        #endregion

        /// <summary>
        /// 准备开始任务
        /// </summary>
        private void OnPreJob()
        {
            //try
            //{
            if (PreJob != null)
                PreJob(this, EventArgs.Empty);
            //}
            //catch (Exception ex)
            //{
            //    EventLogs.Warn("PreJob 出错", "Jobs", 807, ex);
            //}
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        private void OnPostJob()
        {
            //try
            //{
            if (PostJob != null)
                PostJob(this, EventArgs.Empty);
            //}
            //catch (Exception ex)
            //{
            //    EventLogs.Warn("OnPostJob 出错", "Jobs", 807, ex);
            //}
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="ijob"></param>
        /// <param name="node"></param>
        public Job(Type ijob, XmlNode node)
        {
            _node = node;
            _jobType = ijob;

            XmlAttribute att = node.Attributes["enabled"];//是否允许运行
            if (att != null)
                this._enabled = bool.Parse(att.Value);

            att = node.Attributes["enableShutDown"];//是否允许停止
            if (att != null)
                this._enableShutDown = bool.Parse(att.Value);
            att = node.Attributes["name"];//任务类名
            if (att != null)
                this._name = att.Value;

            att = node.Attributes["millisecond"];
            if (att != null)
            {
                _millisecond = Int32.Parse(att.Value);
            }

            att = node.Attributes["seconds"];
            if (att != null)
            {
                _seconds = Int32.Parse(att.Value);
            }

            att = node.Attributes["minutes"];
            if (att != null)
            {
                try
                {
                    this._minutes = Int32.Parse(att.Value);
                }
                catch
                {
                    this._minutes = 15;
                }
            }

            att = node.Attributes["singleThread"];//任务是否在单线程下运行
            if (att != null && !string.IsNullOrEmpty(att.Value) && string.Compare(att.Value, "false", false) == 0)
                _singleThread = false;
        }

        /// <summary>
        ///创建timer
        /// </summary>
        public void InitializeTimer()
        {
            if (_timer == null && Enabled)
            {
                _timer = new Timer(new TimerCallback(timer_Callback), null, Interval, Interval);
            }
        }

        /// <summary>
        /// timer回调函数
        /// </summary>
        /// <param name="state"></param>
        private void timer_Callback(object state)
        {
            if (!Enabled)
                return;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            ExecuteJob();
            if (Enabled)
                _timer.Change(Interval, Interval);
            else
                this.Dispose();
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        public void ExecuteJob()
        {
            OnPreJob();
            _isRunning = true;
            IJob ijob = this.CreateJobInstance();
            if (ijob != null)
            {
                _lastStart = DateTime.Now;
                try
                {
                    _isFirstRun = false;
                    ijob.Execute(this._node);
                    _lastEnd = _lastSucess = DateTime.Now;

                }
                catch (Exception ex)
                {
                    this._enabled = !this.EnableShutDown;
                    _lastEnd = DateTime.Now;
                }
            }
            _isRunning = false;
            OnPostJob();
        }

        #region 公共属性

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public DateTime LastStarted
        {
            get { return _lastStart; }
        }

        public DateTime LastEnd
        {
            get { return _lastEnd; }
        }

        public DateTime LastSuccess
        {
            get { return _lastSucess; }
        }

        public bool SingleThreaded
        {
            get { return _singleThread; }
        }

        public bool IsFirstRun
        {
            get { return _isFirstRun; }
        }

        public Type JobType
        {
            get { return this._jobType; }

        }

        public int Minutes
        {
            get { return _minutes; }
            set { _minutes = value; }
        }



        /// <summary>
        /// 发生异常是否停止运行
        /// </summary>
        public bool EnableShutDown
        {
            get { return this._enableShutDown; }
        }


        /// <summary>
        /// 任务名
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// 任务是否运行执行
        /// </summary>
        public bool Enabled
        {
            get { return this._enabled; }
        }


        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public IJob CreateJobInstance()
        {
            if (Enabled)
            {
                if (_ijob == null)
                {

                    if (_jobType != null)
                    {
                        try
                        {
                            _ijob = Activator.CreateInstance(_jobType) as IJob;
                        }
                        catch (Exception ex)
                        {
                            _ijob = null;
                            EventLogs.JobError("创建任务实例发生异常", EventLogs.EVENTID_JOB_ERROR + 1, 0, ex);
                        }
                    }
                    _enabled = (_ijob != null);

                    if (!_enabled)
                        this.Dispose();
                }
            }
            return _ijob;
        }

        #endregion

        #region

        /// <summary>
        /// 停止任务
        /// </summary>
        public void Dispose()
        {
            if (_timer != null && !disposed)
            {
                lock (this)
                {
                    _timer.Dispose();
                    _timer = null;
                    disposed = true;
                }
            }
        }

        #endregion
    }
}
