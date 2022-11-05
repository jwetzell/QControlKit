using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Events;
using System;
using System.Threading.Tasks;
using Serilog;

namespace QControlKitXamDemo.ViewModels
{
    public class QBrowserViewModel : INotifyPropertyChanged
    {
        private ILogger _log = Log.Logger.ForContext<QBrowserViewModel>();

        QBrowser browser;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool autoUpdate = false;
        public ObservableCollection<QServerViewModel> ServersGrouped { get; set; }

        bool connected = false;
        public QBrowserViewModel(QBrowser browser)
        {
            this.browser = browser;
            ServersGrouped = new ObservableCollection<QServerViewModel>();

            this.browser.ServerFound += OnServerFound;
            this.browser.ServerLost += OnServerLost;
            Device.StartTimer(TimeSpan.FromSeconds(4), () =>
            {
                Task.Run(() =>
                {
                    if (autoUpdate)
                    {
                        _log.Verbose($"Auto Update is enabled running probe");
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _log.Debug("QBrowser probe triggered");
                            this.browser.ProbeForQLabInstances();
                        });
                    }
                });
                return true;
            });

        }

        private void OnServerFound(object source, QServerFoundArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _log.Debug($"adding server: {args.server.description}");
                ServersGrouped.Add(new QServerViewModel(args.server));
            });
        }


        private void OnServerLost(object source, QServerLostArgs args)
        {
            QServerViewModel serverToRemove = null;

            foreach (QServerViewModel serverGroup in ServersGrouped)
            {
                if (serverGroup.host.Equals(args.server.host))
                {
                    serverToRemove = serverGroup;
                    break;
                }
            }

            if (serverToRemove != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _log.Debug($"removing server: {args.server.description}");
                    ServersGrouped.Remove(serverToRemove);
                });
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void InitiateScan()
        {
            if (!autoUpdate)
            {
                _log.Debug("Manual Scan Initiated");
                browser.ProbeForQLabInstances();
            }
        }
    }
}
