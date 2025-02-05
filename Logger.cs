using Coflnet;
using Microsoft.Extensions.Logging;

namespace dev
{
    public class Logger
    {
        public static Logger Instance { get; }
        public static ILoggerFactory Factory { get; }

        static Logger()
        {
            Instance = new Logger();
            Factory = LoggerFactory.Create(b => b.AddConsole());
        }

        public void Log(string message)
        {
            System.Console.WriteLine("Info: " + message);
            OpenTracing.Util.GlobalTracer.Instance.ActiveSpan?.Log(message);
        }

        public void Error(string message)
        {
            System.Console.WriteLine("Error: " + message);
            OpenTracing.Util.GlobalTracer.Instance.ActiveSpan?.Log(message);
            OpenTracing.Util.GlobalTracer.Instance.ActiveSpan?.SetTag("error",true);
        }

        public void Info(string message)
        {
            Log(message);
        }


        public void Error(System.Exception error, string message = null)
        {
            if (message != null)
                Error(message);
            Error(error.ToString());
        }
    }
}