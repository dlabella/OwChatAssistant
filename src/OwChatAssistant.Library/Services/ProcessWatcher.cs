using System.Diagnostics;
using System.Management;

namespace OwChatAssistant.Library.Services
{
    public class ProcessWatcher
    {
        private string name = string.Empty;
        private ManagementEventWatcher? startWatcher;
        private ManagementEventWatcher? stopWatcher;

        public event Action? OnStarted;
        public event Action? OnStopped;

        public void Start(string programName)
        {
            // 🎮 START
            var startQuery = new WqlEventQuery(
                "SELECT * FROM Win32_ProcessStartTrace");

            startWatcher =
                new ManagementEventWatcher(startQuery);

            startWatcher.EventArrived +=
                OnProcessStarted;

            startWatcher.Start();

            // ❌ STOP
            var stopQuery = new WqlEventQuery(
                "SELECT * FROM Win32_ProcessStopTrace");

            stopWatcher =
                new ManagementEventWatcher(stopQuery);

            stopWatcher.EventArrived +=
                OnProcessStopped;

            stopWatcher.Start();
        }

        private void OnProcessStarted(
            object sender,
            EventArrivedEventArgs e)
        {
            string processName =
                e.NewEvent.Properties["ProcessName"]
                    .Value?
                    .ToString() ?? "";

            if (string.Compare(processName, name, true) == 0)
            {
                OnStarted?.Invoke();
            }
        }
        private void OnProcessStopped(
            object sender,
            EventArrivedEventArgs e)
        {
            string processName =
                e.NewEvent.Properties["ProcessName"]
                    .Value?
                    .ToString() ?? "";

            if (string.Compare(processName,name,true)==0)
            {
                OnStopped?.Invoke();
            }
        }

        public void Stop()
        {
            startWatcher?.Stop();
            startWatcher?.Dispose();
            stopWatcher?.Stop();
            stopWatcher?.Dispose();
        }
    }
}
