using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Timers;
using System.Web;
using System.Security;
using System.Security.Permissions;
namespace PlocyFlow.DAL.Service
{
    public interface IScheduleService
    {
        // Methods
        void Execute();
    }
    public interface ILoggerProvider
    {
        // Methods
        void Write(Exception exception);
        void Write(string message);
        void Write(Exception exception, string message);
    }

 

    internal static class ConfigUtility
    {
        // Fields
        private static TencentOASectionGroup section;

        // Methods
        public static T GetTencentOASection<T>(string elementName) where T : ConfigurationElement
        {
            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }
            var node = ConfigurationManager.GetSection(TencentOASectionGroup.TencentOASectionGroupName + "/" + elementName);
            T section = node as T;
            if (section == null)
            {
                throw new ConfigurationErrorsException("必须在配置组 '" + TencentOASectionGroup.TencentOASectionGroupName + "' 下指定配置节元素 '" + elementName + "'。");
            }
            return section;
        }

        internal static TencentOASectionGroup GetTencentOASectionGroup()
        {
            if (section == null)
            {
                section = (TencentOASectionGroup)ConfigurationManager.GetSection(TencentOASectionGroup.TencentOASectionGroupName);
            }
            if (section == null)
            {
                throw new ConfigurationErrorsException("必须在配置文件中指定配置节 '" + TencentOASectionGroup.TencentOASectionGroupName + "'。");
            }
            return section;
        }
    }
    public sealed class TencentOASectionGroup : ConfigurationSectionGroup
    {
        // Fields
        public static readonly string TencentOASectionGroupName = "tencent.OA";

        // Properties
        [ConfigurationProperty("loggers")]
        public LoggerProviderSection LoggerProviders
        {
            get
            {
                return (LoggerProviderSection)base.Sections["loggers"];
            }
        }

        [ConfigurationProperty("schedules")]
        public ScheduleServiceSection Schedules
        {
            get
            {
                return (ScheduleServiceSection)base.Sections["schedules"];
            }
        }
    }

    [ConfigurationCollection(typeof(LoggerProviderElement), CollectionType = ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class LoggerProviderElementCollection : ConfigurationElementCollection
    {
        // Methods
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerProviderElement();
        }

        public LoggerProviderElement GetElement(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentNullException("providerName");
            }
            return (LoggerProviderElement)base.BaseGet(providerName);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerProviderElement)element).LoggerProviderName;
        }
    }

    public class LoggerProviderElement : ConfigurationElement
    {
        // Fields
        private const string _namePropertyName = "name";
        private const string _typePropertyName = "type";
        private AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();

        // Properties
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string LoggerProviderName
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        public Type LoggerProviderType
        {
            get
            {
                return (Type)this.typeConverter.ConvertFrom(this.LoggerProviderTypeName);
            }
            set
            {
                this.LoggerProviderTypeName = this.typeConverter.ConvertToString(value);
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string LoggerProviderTypeName
        {
            get
            {
                return (string)base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }
    }

    public static class LoggerProviderFactory
    {
        // Fields
        private static LoggerProviderSection _configSection = ConfigUtility.GetTencentOASection<LoggerProviderSection>("loggers");

        // Methods
        private static ILoggerProvider CreateLoggerProvider(string providerName)
        {
            return (Activator.CreateInstance(_configSection.LoggerProviders.GetElement(providerName).LoggerProviderType) as ILoggerProvider);
        }

        public static ILoggerProvider GetLoggerProvider()
        {
            return GetLoggerProvider(_configSection.DefaultProvider);
        }

        public static ILoggerProvider GetLoggerProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentNullException("必须指定提供程序的名字，该名字与在.config文件的loggerProvider节下的日志提供程序对应。");
            }
            return CreateLoggerProvider(providerName);
        }
    }

 


    public static class Logger
    {
        // Fields
        private static volatile ILoggerProvider _provider;
        private static object _syncLockObject = new object();

        // Methods
        public static void Write(Exception exception)
        {
            LoggerProvider.Write(exception);
        }

        public static void Write(string message)
        {
            LoggerProvider.Write(message);
        }

        public static void Write(Exception exception, string message)
        {
            LoggerProvider.Write(exception, message);
        }

        // Properties
        public static ILoggerProvider LoggerProvider
        {
            get
            {
                if (_provider == null)
                {
                    lock (_syncLockObject)
                    {
                        if (_provider == null)
                        {
                            try
                            {
                                _provider = LoggerProviderFactory.GetLoggerProvider();
                            }
                            catch (Exception exception)
                            {
                                throw exception;
                            }
                        }
                    }
                }
                return _provider;
            }
        }
    }

 




    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class LoggerProviderSection : ConfigurationSection
    {
        // Fields
        private static readonly ConfigurationProperty _defaultProvider = new ConfigurationProperty("defaultProvider", typeof(string));
        private static readonly ConfigurationProperty _loggers = new ConfigurationProperty(null, typeof(LoggerProviderElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        internal const string LoggerSectionKey = "loggers";

        // Methods
        static LoggerProviderSection()
        {
            _properties.Add(_loggers);
            _properties.Add(_defaultProvider);
        }

        // Properties
        public string DefaultProvider
        {
            get
            {
                return (string)base[_defaultProvider];
            }
            set
            {
                base[_defaultProvider] = value;
            }
        }

        public LoggerProviderElementCollection LoggerProviders
        {
            get
            {
                return (LoggerProviderElementCollection)base[_loggers];
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }
    }

 


 

    [ConfigurationCollection(typeof(ScheduleServiceProviderElement), CollectionType = ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ScheduleServiceProviderCollection : ConfigurationElementCollection
    {
        // Methods
        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduleServiceProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ScheduleServiceProviderElement)element).ScheduleServiceName;
        }

        // Properties
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "task";
            }
        }
    }

    public class AssemblyQualifiedTypeNameConverter : ConfigurationConverterBase
    {
        // Methods
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = (string)value;
            if (!string.IsNullOrEmpty(str))
            {
                Type type = Type.GetType(str, false);
                if (type == null)
                {
                    throw new ArgumentException(string.Format("The type '{0}' cannot be resolved. Please verify the spelling is correct or that the full type name is provided.", str));
                }
                return type;
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                Type type = value as Type;
                if (type == null)
                {
                    throw new ArgumentException(string.Format("The AssemblyQualifiedTypeNameConverter can only convert values of type '{0}'.", typeof(Type).Name));
                }
                if (type != null)
                {
                    return type.AssemblyQualifiedName;
                }
            }
            return null;
        }
    }

 

    public class ScheduleServiceProviderElement : ConfigurationElement
    {
        // Fields
        private const string namePropertyName = "name";
        private AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();
        private const string typePropertyName = "type";

        // Methods
        public void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }

        // Properties
        [ConfigurationProperty("enableShutDown", DefaultValue = false)]
        public bool EnableShutDown
        {
            get
            {
                return (bool)base["enableShutDown"];
            }
            set
            {
                base["enableShutDown"] = value;
            }
        }

        [ConfigurationProperty("executingAbsoluteTime", DefaultValue = false)]
        public bool ExecuteAbsoluteTime
        {
            get
            {
                return (bool)base["executingAbsoluteTime"];
            }
            set
            {
                base["executingAbsoluteTime"] = value;
            }
        }

        [ConfigurationProperty("executeOnStart", DefaultValue = true)]
        public bool ExecuteOnStart
        {
            get
            {
                return (bool)base["executeOnStart"];
            }
            set
            {
                base["executeOnStart"] = value;
            }
        }

        [ConfigurationProperty("executingTime", DefaultValue = "02:00:00")]
        public DateTime ExecutingTime
        {
            get
            {
                return (DateTime)base["executingTime"];
            }
            set
            {
                base["executingTime"] = value;
            }
        }

        [ConfigurationProperty("interval")]
        public int Interval
        {
            get
            {
                return (int)base["interval"];
            }
            set
            {
                base["interval"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string ScheduleServiceName
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        public Type ScheduleServiceType
        {
            get
            {
                return (Type)this.typeConverter.ConvertFrom(this.ScheduleServiceTypeName);
            }
            set
            {
                this.ScheduleServiceTypeName = this.typeConverter.ConvertToString(value);
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string ScheduleServiceTypeName
        {
            get
            {
                return (string)base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }
    }

 

    public class ScheduleTask : IDisposable
    {
        // Fields
        private bool _absoluteExecuting;
        private bool _disposed;
        private bool _enableShutdown;
        private bool _executeOnServiceStart;
        private DateTime _executingTime;
        private DateTime? _firstExecuteTime;
        private int _interval;
        private DateTime? _lastestCompleteTime;
        private IScheduleService _scheduleService;
        private Timer _timer;

        // Methods
        public ScheduleTask(IScheduleService scheduleService, DateTime executingTime)
            : this(scheduleService, executingTime, 1, true, null)
        {
        }

        public ScheduleTask(IScheduleService scheduleService, int interval)
            : this(scheduleService, interval, null)
        {
        }

        public ScheduleTask(IScheduleService scheduleService, int interval, ScheduleServiceProviderElement setting)
            : this(scheduleService, DateTime.Now, interval, false, setting)
        {
        }

        private ScheduleTask(IScheduleService scheduleService, DateTime executingTime, int interval, bool absoluteExecuting, ScheduleServiceProviderElement setting)
        {
            this._timer = null;
            this._interval = 5;
            this._scheduleService = scheduleService;
            this._executingTime = executingTime;
            this._interval = interval;
            this._absoluteExecuting = absoluteExecuting;
            if (setting != null)
            {
                this._executeOnServiceStart = setting.ExecuteOnStart;
                this._enableShutdown = setting.EnableShutDown;
            }
            this._timer = new Timer();
            this._timer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._timer.Stop();
                    this._timer.Dispose();
                }
                this._disposed = true;
            }
        }

        private bool ExecutableTiming(DateTime left)
        {
            int num = 60;
            TimeSpan span = (TimeSpan)(left - DateTime.Now);
            long totalSeconds = (long)span.TotalSeconds;
            return ((totalSeconds > 0L) && (totalSeconds <= num));
        }

        private void Execute()
        {
            if (!this._absoluteExecuting || this.ExecutableTiming(this._executingTime))
            {
                if (!this._firstExecuteTime.HasValue)
                {
                    this._firstExecuteTime = new DateTime?(DateTime.Now);
                }
                this._scheduleService.Execute();
            }
        }

        ~ScheduleTask()
        {
            this.Dispose(false);
        }

        protected virtual void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            this.Execute();
            this._lastestCompleteTime = new DateTime?(e.SignalTime);
        }

        public void Start()
        {
            if (this._executeOnServiceStart)
            {
                this.Execute();
            }
            this._timer.Interval = this.Interval;
            this._timer.Start();
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        // Properties
        internal Timer ExecutiveHandle
        {
            get
            {
                return this._timer;
            }
        }

        public DateTime? FirstExecuteTime
        {
            get
            {
                return this._firstExecuteTime;
            }
        }

        protected virtual double Interval
        {
            get
            {
                return (this._interval * 60000.0);
            }
        }

        public DateTime? LastestCompleteTime
        {
            get
            {
                return this._lastestCompleteTime;
            }
        }

        public DateTime StartExecuteTime
        {
            get
            {
                return this._executingTime;
            }
        }
    }

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScheduleServiceSection : ConfigurationSection
    {
        // Fields
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propInterval = new ConfigurationProperty("interval", typeof(int), 5);
        private static readonly ConfigurationProperty _propSchedules = new ConfigurationProperty(null, typeof(ScheduleServiceProviderCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        internal const string ScheduleServiceSectionKey = "schedules";

        // Methods
        static ScheduleServiceSection()
        {
            _properties.Add(_propSchedules);
            _properties.Add(_propInterval);
        }

        // Properties
        public int Interval
        {
            get
            {
                return (int)base[_propInterval];
            }
            set
            {
                base[_propInterval] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }

        public ScheduleServiceProviderCollection ScheduleServiceProviders
        {
            get
            {
                return (ScheduleServiceProviderCollection)base[_propSchedules];
            }
        }
    }
 


    public class ScheduleServiceFactory
    {
        // Fields
        private DateTime _created = DateTime.Now;
        private DateTime? _firstExecute;
        private bool _isRunning;
        private DateTime? _lastestExecute;
        private DateTime? _started;
        private Dictionary<string, ScheduleTask> _taskDictionary = new Dictionary<string, ScheduleTask>();

        // Methods
        public void AddSchedule(string scheduleServiceName, IScheduleService scheduleService, int interval)
        {
            if (scheduleServiceName == null)
            {
                throw new ArgumentNullException("scheduleServiceName");
            }
            if (scheduleService == null)
            {
                throw new ArgumentNullException("scheduleService");
            }
            if (interval <= 0)
            {
                throw new ArgumentOutOfRangeException("interval");
            }
            ScheduleTask task = new ScheduleTask(scheduleService, interval);
            this._taskDictionary.Add(scheduleServiceName, task);
        }

        public bool ContainsSchedule(string scheduleServiceName)
        {
            return this._taskDictionary.ContainsKey(scheduleServiceName);
        }

        public void Continue()
        {
            if (!this._isRunning)
            {
                foreach (ScheduleTask task in this._taskDictionary.Values)
                {
                    task.ExecutiveHandle.Enabled = true;
                }
            }
        }

        private void InitializeTasks(bool start)
        {
            ScheduleServiceSection tencentOASection = ConfigUtility.GetTencentOASection<ScheduleServiceSection>("schedules");
            ScheduleServiceProviderCollection scheduleServiceProviders = tencentOASection.ScheduleServiceProviders;
            if (scheduleServiceProviders != null)
            {
                int interval = tencentOASection.Interval;
                foreach (ScheduleServiceProviderElement element in scheduleServiceProviders)
                {
                    ScheduleTask task;
                    IScheduleService scheduleService = Activator.CreateInstance(element.ScheduleServiceType) as IScheduleService;
                    if (element.ExecuteAbsoluteTime)
                    {
                        task = new ScheduleTask(scheduleService, element.ExecutingTime);
                    }
                    else
                    {
                        int num2 = element.Interval;
                        if (num2 == 0)
                        {
                            num2 = interval;
                        }
                        task = new ScheduleTask(scheduleService, num2, element);
                    }
                    this._taskDictionary[element.ScheduleServiceName] = task;
                    if (start)
                    {
                        task.Start();
                    }
                }
            }
        }

        public void Pause()
        {
            if (this._isRunning)
            {
                foreach (ScheduleTask task in this._taskDictionary.Values)
                {
                    task.ExecutiveHandle.Enabled = false;
                }
            }
        }

        private void Reset()
        {
            this._taskDictionary.Clear();
            this._isRunning = false;
            this._firstExecute = null;
            this._lastestExecute = null;
        }

        public void Start()
        {
            if (!this._isRunning)
            {
                if (!this._started.HasValue)
                {
                    this._started = new DateTime?(DateTime.Now);
                }
                foreach (ScheduleTask task in this._taskDictionary.Values)
                {
                    task.Start();
                }
                this.InitializeTasks(true);
                this._isRunning = true;
            }
        }

        public void Stop()
        {
            if (this._isRunning)
            {
                foreach (ScheduleTask task in this._taskDictionary.Values)
                {
                    task.ExecutiveHandle.Enabled = false;
                    task.Dispose();
                }
                this.Reset();
            }
        }

        public override string ToString()
        {
            if (this._isRunning)
            {
                return string.Format("Current has {0} Schedule taskes runing, First Created: {1}, Started: {2}, FirstExecute: {3}, LastExecute: {4}, IsRunning: {5}", new object[] { this._taskDictionary.Count, this._created, this._started, this._firstExecute, this._lastestExecute, this._isRunning });
            }
            return "Current Status, all task stoped.";
        }

        // Properties
        public DateTime CreatedDatetime
        {
            get
            {
                return this._created;
            }
        }

        public DateTime? FirstStartDatetime
        {
            get
            {
                return this._started;
            }
        }

        public DateTime? FirstTaskExecute
        {
            get
            {
                if (this._isRunning)
                {
                    if (this._firstExecute.HasValue)
                    {
                        return this._firstExecute;
                    }
                    foreach (ScheduleTask task in this._taskDictionary.Values)
                    {
                        if ((task.FirstExecuteTime.HasValue && !this._firstExecute.HasValue) || ((task.FirstExecuteTime.HasValue && this._firstExecute.HasValue) && (task.FirstExecuteTime.Value < this._firstExecute.Value)))
                        {
                            this._firstExecute = task.FirstExecuteTime;
                        }
                    }
                }
                return this._firstExecute;
            }
        }

        public DateTime? LastestTaskExecute
        {
            get
            {
                if (this._isRunning)
                {
                    foreach (ScheduleTask task in this._taskDictionary.Values)
                    {
                        if ((task.LastestCompleteTime.HasValue && !this._lastestExecute.HasValue) || ((task.LastestCompleteTime.HasValue && this._lastestExecute.HasValue) && (task.LastestCompleteTime.Value > this._lastestExecute.Value)))
                        {
                            this._lastestExecute = task.LastestCompleteTime;
                        }
                    }
                }
                return this._lastestExecute;
            }
        }
    }

 

 

    
}
