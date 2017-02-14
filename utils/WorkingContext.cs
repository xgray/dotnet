namespace Bench
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Interface to specify necessary properties and methods in a task-oriented execution.
    /// </summary>
    public interface IWorkingContext
    {
        /// <summary>
        /// Current time stamp
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Working directory
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="component">exeuction component</param>
        /// <param name="level">diagnostic</param>
        /// <param name="category">log category</param>
        /// <param name="msg">format string</param>
        /// <param name="args">message arguments</param>
        void Log(string component, TraceLevel level, string category, string msg, params object[] args);
    }

    public class WorkingContextBase : IWorkingContext
    {
        public IWorkingContext Context { get; set; }

        public DateTime UtcNow
        {
            get { return this.Context.UtcNow; }
        }

        public string WorkingDirectory
        {
            get { return Context.WorkingDirectory; }
        }

        public void Log(string component, TraceLevel level, string category, string msg, params object[] args)
        {
            this.Context.Log(component, level, category, msg, args);
        }
    }

    /// <summary>
    /// Working context.
    /// </summary>
    public class WorkingContext : IWorkingContext
    {
        /// <summary>
        /// default current working context
        /// </summary>
        private static WorkingContext current = new WorkingContext();

        /// <summary>
        /// working directory
        /// </summary>
        private string workingDirector = System.IO.Directory.GetCurrentDirectory();

        /// <summary>
        /// console lock
        /// </summary>
        private object consoleLock = new object();

        /// <summary>
        /// Gets default working context
        /// </summary>
        public static IWorkingContext Current
        {
            get { return WorkingContext.current; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WorkingDirectory
        {
            get { return this.workingDirector; }
            set { this.workingDirector = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="level"></param>
        /// <param name="category"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Log(string component, TraceLevel level, string category, string msg, params object[] args)
        {
            Trace.WriteLine(string.Format("{0}:{1}:{2}:{3}", component, level, category, CommonUtils.Format(msg, args)));

            lock (this.consoleLock)
            {
                ConsoleColor oldColor = Console.ForegroundColor;

                try
                {
                    if (level == TraceLevel.Info)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (level == TraceLevel.Error)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else if (level == TraceLevel.Warning)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }

                    Console.WriteLine(msg, args);
                }
                finally
                {
                    Console.ForegroundColor = oldColor;
                }
            }
        }
    }
}
