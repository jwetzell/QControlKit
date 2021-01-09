using System;
using System.ComponentModel;
using Xamarin.Forms;
using QControlKit;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using QControlKit.Events;
using System.Threading.Tasks;

namespace QControlKitXamDemo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ServerGroup> servers = new ObservableCollection<ServerGroup>();
        public MainPage()
        {
            InitializeComponent();
            serverListView.ItemsSource = servers;
            QBrowser browser = new QBrowser();
            browser.ServerLost += Browser_ServerLost;
            browser.ServerFound += Browser_ServerFound;
            serverListView.ItemSelected += ServerListView_ItemSelected;

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                Task.Run(async () =>
                {
                    browser.ProbeForQLabInstances();
                });
                return true;
            });

        }

        private void Browser_ServerFound(object source, QServerFoundArgs args)
        {
            List<ServerGroup> serverGroups = servers.ToList();

            bool serverFound = false;
            foreach (var serverGroup in serverGroups)
            {

                if (serverGroup.host == args.server.host)
                {
                    serverFound = true;
                    
                }
            }

            if (!serverFound)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ServerGroup serverGroup = new ServerGroup(args.server.name);
                    args.server.WorkspaceAdded += Server_WorkspaceAdded;
                    args.server.WorkspaceRemoved += Server_WorkspaceRemoved;
                    serverGroup.host = args.server.host;
                    servers.Add(serverGroup);
                });
            }
        }

        private void Server_WorkspaceRemoved(object source, QServerWorkspaceChangedArgs args)
        {
            foreach (var server in servers)
            {
                if (server.host == args.server.host)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        server.Remove(args.workspace);
                    });
                }
            }
        }

        private void Server_WorkspaceAdded(object source, QServerWorkspaceChangedArgs args)
        {
            foreach(var server in servers)
            {
                if(server.host == args.server.host)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        server.Add(args.workspace);
                    });
                }
            }
        }

        private void Browser_ServerLost(object source, QServerLostArgs args)
        {

            List<ServerGroup> serverGroups = servers.ToList();
            foreach(var serverGroup in serverGroups)
            {
                if(serverGroup.host == args.server.host)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        servers.Remove(serverGroup);
                    });
                }
            }
        }

        async void ServerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                QWorkspace selectedWorkspace = e.SelectedItem as QWorkspace;
                Console.WriteLine($"[demo] Workspace <{selectedWorkspace.nameWithoutPathExtension}> has been selected");
                if (selectedWorkspace.hasPasscode)
                {
                    string passcode = await DisplayPromptAsync("Workspace has passcode", "Enter Passcode", maxLength: 4, keyboard: Keyboard.Numeric);
                    if (passcode != null)
                    {

                        await Navigation.PushAsync(new CueListPage(selectedWorkspace, passcode));
                    }
                }
                else
                {
                    await Navigation.PushAsync(new CueListPage(selectedWorkspace));

                }
                serverListView.SelectedItem = null;
            }
        }
    }

    public class ServerGroup : ObservableCollection<QWorkspace>{
        public string name { get; set; }
        public string host { get; set; }
        public ServerGroup(string name)
        {
            this.name = name;
        }


    }
}
