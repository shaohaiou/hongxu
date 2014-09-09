using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Hx.Tools
{
    public class ExpLog
    {
        private static readonly string LOG_DIR = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["logPath"];
        private static readonly string LOG_FILE = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["logPath"] + "explog.txt";
        private static object o = new object();

        public ExpLog()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static void Write(Exception e)
        {
            StreamWriter sw = null;
            try
            {
                if (!Directory.Exists(LOG_DIR))
                {
                    Directory.CreateDirectory(LOG_DIR);
                }
                int index = 0;
                string filePath = LOG_DIR + @"explog " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                FileInfo fi = new FileInfo(filePath);
                while (fi.Exists && fi.Length > 1024 * 1024)
                {
                    index++;
                    filePath = LOG_DIR + @"explog " + DateTime.Now.ToString("yyyy-MM-dd") + string.Format("_{0}.txt", index);
                    fi = new FileInfo(filePath);
                }
                sw = new StreamWriter(filePath, true, System.Text.Encoding.UTF8);
                sw.WriteLine(DateTime.Now.ToString() + "        " + e.Message);
                sw.WriteLine("Source:" + e.Source);
                sw.WriteLine("TargetSite:" + e.TargetSite);
                sw.WriteLine(EnhancedStackTrace(new StackTrace(e, true)));
                sw.WriteLine("====================================");
                sw.Close();
            }
            catch
            {
                if (sw != null) sw.Close();
            }
        }

        private static string EnhancedStackTrace(StackTrace st)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("---- Stack Trace ----");
            sb.Append(Environment.NewLine);

            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                MemberInfo mi = sf.GetMethod();
                sb.Append(StackFrameToString(sf));
            }
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }
        private static string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();
            int intParam; MemberInfo mi = sf.GetMethod();
            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);
            // -- build method params           
            sb.Append("(");
            intParam = 0;
            foreach (ParameterInfo param in sf.GetMethod().GetParameters())
            {
                intParam += 1;
                sb.Append(param.Name);
                sb.Append(" As ");
                sb.Append(param.ParameterType.Name);
            }
            sb.Append(")");
            sb.Append(Environment.NewLine);
            // -- if source code is available, append location info           
            sb.Append("       ");
            if (string.IsNullOrEmpty(sf.GetFileName()))
            {
                sb.Append("(unknown file)");
                //-- native code offset is always available               
                sb.Append(": N ");
                sb.Append(String.Format("{0:#00000}", sf.GetNativeOffset()));
            }
            else
            {
                sb.Append(System.IO.Path.GetFileName(sf.GetFileName()));
                sb.Append(": line ");
                sb.Append(String.Format("{0:#0000}", sf.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(String.Format("{0:#00}", sf.GetFileColumnNumber()));

                if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(String.Format("{0:#0000}", sf.GetILOffset()));
                }
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}
