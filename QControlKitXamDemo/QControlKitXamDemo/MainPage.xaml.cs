using System;
using System.ComponentModel;
using Xamarin.Forms;
using QControlKit;
using Serilog;
using QControlKitXamDemo.ViewModels;
using Acr.UserDialogs;

namespace QControlKitXamDemo
{
    public partial class MainPage : ContentPage
    {
        private QBrowserViewModel qBrowserViewModel;
        public MainPage()
        {
            InitializeComponent();

            qBrowserViewModel = new QBrowserViewModel(new QBrowser());
            serverListView.BindingContext = qBrowserViewModel;
            serverListView.ItemSelected += QWorkspaceSelected;

        }

        async void QWorkspaceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            qBrowserViewModel.autoUpdate = false;
            QWorkspace selectedWorkspace = (e.SelectedItem as QWorkspaceViewModel).workspace;
            Log.Debug($"workspace: {selectedWorkspace.nameWithoutPathExtension} has been selected");
            ((ListView)sender).SelectedItem = null;

            if (selectedWorkspace.hasPasscode)
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    InputType = InputType.Number,
                    MaxLength = 4,
                    Title = "Workspace Requires Passcode",
                    OkText = "Connect",
                    IsCancellable = true,
                    OnTextChanged = args =>
                    {
                        args.IsValid = args.Value != null && !args.Value.Equals("") && args.Value.Length == 4;
                    },
                    OnAction = async (resp) =>
                    {
                        if (resp.Ok)
                        {
                            await Navigation.PushAsync(new CueListPage(selectedWorkspace, resp.Value));
                        }
                    }

                });
            }
            else
            {
                await Navigation.PushAsync(new CueListPage(selectedWorkspace));
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            qBrowserViewModel.autoUpdate = true;
        }
    }
}