using Microsoft.Deployment.WindowsInstaller;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace RISA_CustomActionsLib
{
    // note that this this lib is built against .NET 4.5
    // - very low bar, most / all customer machines wil have this,
    //   eliminating the need to provision the customer machine for this lib
    //
    public partial class CustomActions
    {
        #region Stop / Start RISA32 Service

        //
        // The Services applet says the Name + Description are "RISA32 License Manager Service";
        //      the service name, used below, is RISA32
        //
        // Code was cribbed from LicensingDashboard, where it's implemented using async/await,
        //      however that version and this one simply:
        //          issue stop/start service,
        //          then: sleep / poll <-repeat until service is in desired state.
        //      Re-implement here using old school Thread.Sleep to avoid client interfacing problems with async/await
        //
        private const string _risaServiceName = "RISA32";
        private const int _sleep_millisecs = 500;
        private const int _num_SleepsPerSecond = 2;
        private const int _maxWaitForService_Seconds = 60;

        [CustomAction]
        public static  CustomActions ServiceControlStopRISA32(Session session)
        {
            session.Log("ServiceControlStopRISA32");
            var services = ServiceController.GetServices();

            var risaSvc = services.FirstOrDefault(x => x.ServiceName == _risaServiceName);

            if (risaSvc == null) return CustomActions.Success;                                   // service isn't installed
            if (risaSvc.Status != ServiceControllerStatus.Running) return CustomActions.Success; // service isn't running
            //
            // stop service
            //
            try
            {
                session.Log("Attempting to stop RISA32");
                risaSvc.Stop();             // asynchronous, call returns before service actually stops

                for (var nWaits = 0; nWaits < _maxWaitForService_Seconds * _num_SleepsPerSecond; nWaits++)
                {
                    Thread.Sleep(_sleep_millisecs);
                    risaSvc.Refresh();
                    if (risaSvc.Status == ServiceControllerStatus.Stopped) return CustomActions.Success;
                    session.Log("RISA32 service not stopped yet");
                }
            }
            catch
            {
                session.Log("Stopping RISA32 service: threw an exception");
            }
            session.Log("ServiceControlStopRISA32: Returning failue");
            return ActionResult.Failure;
        }

        [CustomAction]
        public static ActionResult ServiceControlStartRISA32(Session session)
        {
            // always return Success, and is fire n forget - don't wait for service to fully start
            //
            var services = ServiceController.GetServices();

            var risaSvc = services.FirstOrDefault(x => x.ServiceName == _risaServiceName);

            if (risaSvc == null) return ActionResult.Success;                                   // service isn't installed
            if (risaSvc.Status == ServiceControllerStatus.Running) return ActionResult.Success; // service is already running
            //
            // start service
            //
            try
            {
                session.Log("Attempting to start RISA32");
                risaSvc.Start();             // asynchronous, call returns before service actually starts

            //    for (var nWaits = 0; nWaits < _maxWaitForService_Seconds * _num_SleepsPerSecond; nWaits++)
            //    {
            //        Thread.Sleep(_sleep_millisecs);
            //        if (risaSvc.Status == ServiceControllerStatus.Running) return ActionResult.Success;
            //    }
            }
            catch
            {
            }
            return ActionResult.Success;
        }

        #endregion

    }
}
