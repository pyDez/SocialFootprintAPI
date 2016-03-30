using System;
using PostSharp.Aspects;
using System.Diagnostics;
using PostSharp.Serialization;
using System.Web;

namespace iRocks.AI.Helpers.Loging
{


    [Serializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs event_args)
        {

            Logging.SetStartTime();
            Logging.SetParameter("method_name", event_args.Method.Name);
            Logging.SetParameter("class_name", event_args.Instance.GetType().ToString());
            Logging.SetParameter("user_name", HttpContext.Current.User.Identity.Name);

            //for analysis, we want to be able to identify individual executions
            event_args.MethodExecutionTag = Guid.NewGuid();

            Logging.PushContext(event_args.MethodExecutionTag);
            Logging.SetParameter("parameters", ParametersToString(event_args));

            Logging.Info("Called " + event_args.Method);
        }

        public override void OnExit(MethodExecutionArgs event_args)
        {
            Logging.SetParameter("parameters", ParametersToString(event_args));
            Logging.SetParameter("method_name", event_args.Method.Name);
            Logging.SetParameter("class_name", event_args.Instance.GetType().ToString());
            Logging.SetParameter("user_name", HttpContext.Current.User.Identity.Name);
            Logging.Info("Finished " + event_args.Method);

            Logging.PopContext();
        }

        public override void OnException(MethodExecutionArgs event_args)
        {
            Logging.SetParameter("parameters", ParametersToString(event_args));
            Logging.SetParameter("method_name", event_args.Method.Name);
            Logging.SetParameter("class_name", event_args.Instance.GetType().ToString());
            Logging.SetParameter("user_name", HttpContext.Current.User.Identity.Name);
            Logging.Error( "Error Encountered in " + event_args.Method, event_args.Exception);
        }

        //helpers
        private static String ParametersToString(MethodExecutionArgs event_args)
        {
            String output = "";
            if (event_args.Arguments.Count > 0)
            {
               
                for (int i = 0; i < event_args.Arguments.Count; ++i)
                {
                    var value = event_args.Arguments[i]!=null?event_args.Arguments[i].ToString() : "null";
                    output += String.Format("[{0} = {1}]", event_args.Method.GetParameters()[i].Name, value);
                }
            }
            return output;
        }
    }

}
