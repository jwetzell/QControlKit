﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Events;
using Serilog;

namespace QControlKitXamDemo.ViewModels
{
    public class QServerViewModel : ObservableCollection<QWorkspaceViewModel>, INotifyPropertyChanged
    {
        private ILogger _log = Log.Logger.ForContext<QServerViewModel>();

        QServer server;
        public new event PropertyChangedEventHandler PropertyChanged;
        public string name
        {
            get => server.name;
            set => server.name = value;
        }

        public string host => server.host;

        public string version => server.version == null ? "" : $"v{server.version}";

        public string GroupName => $"{name} ({host})";

        public QServerViewModel(QServer server)
        {
            this.server = server;
            name = this.server.name;
            foreach (QWorkspace workspace in this.server.workspaces)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Add(new QWorkspaceViewModel(workspace));
                });
            }
            this.server.WorkspaceAdded += Server_WorkspaceAdded;
            this.server.WorkspaceRemoved += Server_WorkspaceRemoved;
            this.server.ServerUpdated += OnServerUpdated;
        }

        private void Server_WorkspaceRemoved(object source, QServerWorkspaceChangedArgs args)
        {
            QWorkspaceViewModel workspaceToRemove = null;

            foreach (QWorkspaceViewModel workspaceViewModel in this)
            {
                if (workspaceViewModel.workspace.uniqueID == args.workspace.uniqueID)
                {
                    workspaceToRemove = workspaceViewModel;
                    break;
                }
            }
            if (workspaceToRemove != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Remove(workspaceToRemove);
                });
            }

        }

        private void Server_WorkspaceAdded(object source, QServerWorkspaceChangedArgs args)
        {
            QWorkspaceViewModel workspaceViewModel = new QWorkspaceViewModel(args.workspace);
            Device.BeginInvokeOnMainThread(() =>
            {
                Add(workspaceViewModel);
            });
        }



        private void OnServerUpdated(object source, QServerUpdatedArgs args)
        {
            OnPropertyChanged("GroupName");
            OnPropertyChanged("version");
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
