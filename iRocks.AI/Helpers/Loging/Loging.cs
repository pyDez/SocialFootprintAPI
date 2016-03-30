using System;
using log4net;

namespace iRocks.AI.Helpers.Loging
{
    public static class Logging
    {
        /// <summary>Log an Informational Message</summary>
        public static void Info(String message)
        {
            LogManager.GetLogger("INFO").Info(message);
        }

        /// <summary>Log an Error Message</summary>
        public static void Error(String message, Exception ex)
        {
            //for errors, use the ErrorLog that will write to a flat file (bufferless) rather than database (buffered)
            //LogManager.GetLogger("ErrorLogger").Error(message, ex);

            LogManager.GetLogger("ERROR").Error(message, ex);
        }

        /// <summary>Insert a parameter to MDC</summary>
        public static void SetParameter(String name, String value)
        {
            ThreadContext.Properties[name] = value;
        }

        /// <summary>Remove parameter from MDC</summary>
        public static void RemoveParameter(String name)
        {
            ThreadContext.Properties.Remove(name);
        }

        /// <summary>Add another context to NDC</summary>
        public static void PushContext(Object obj)
        {
            ThreadContext.Stacks["NDC"].Push(obj.ToString());
        }

        /// <summary>Clear all contexts from NDC</summary>
        public static void ClearContext()
        {
            ThreadContext.Stacks["NDC"].Clear();
        }

        /// <summary>Remove top context from NDC</summary>
        public static void PopContext()
        {
            ThreadContext.Stacks["NDC"].Pop();
        }

        /// <summary>Store context-specific start time for sharing across methods (within a thread)</summary>
        public static void SetStartTime()
        {
            ThreadContext.Properties["start_time"] = DateTime.Now.ToString();
        }
    }
}



 


