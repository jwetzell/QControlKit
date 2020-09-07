using System;
using System.ComponentModel;
using Xamarin.Forms;
using QControlKit;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
            browser.ServerUpdatedWorkspaces += Browser_ServerUpdatedWorkspaces;
            serverListView.ItemSelected += ServerListView_ItemSelected;

        }

        async void ServerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                QWorkspace selectedWorkspace = e.SelectedItem as QWorkspace;
                Console.WriteLine($"[demo] workspace: {selectedWorkspace.nameWithoutPathExtension} has been selected");
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

        private void Browser_ServerUpdatedWorkspaces(object source, QServerUpdatedArgs args)
        {
            Console.WriteLine(args.server.name);

            ServerGroup serverGroup = new ServerGroup(args.server.name);
            serverGroup.AddRange(args.server.workspaces);

            Device.BeginInvokeOnMainThread(() =>
            {
                servers.Add(serverGroup);

            });
        }
    }

    public class ServerGroup : List<QWorkspace>{
        public string name { get; set; }

        public ServerGroup(string name)
        {
            this.name = name;
        }
    }
}
