using System.Collections.Generic;
using System.Threading.Tasks;
using QControlKit;
using QControlKitXamDemo.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

using QControlKit.Events;
using QControlKit.Constants;

namespace QControlKitXamDemo
{
    public partial class CueListPage : ContentPage
    {
        private QWorkspace connectedWorkspace;
        private Dictionary<string, Grid> cueGridDict = new Dictionary<string, Grid>();
        public CueListPage(QWorkspace workspace, string passcode = null)
        {
            InitializeComponent();
            connectedWorkspace = workspace;
            connectedWorkspace.WorkspaceUpdated += Workspace_WorkspaceUpdated;
            connectedWorkspace.WorkspaceConnectionError += ConnectedWorkspace_WorkspaceConnectionError;
            connectedWorkspace.connect(passcode);
            connectedWorkspace.defaultSendUpdatesOSC = true;
            connectedWorkspace.CueListChangedPlaybackPosition += ConnectedWorkspace_CueListChangedPlaybackPosition;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            connectedWorkspace.disconnect();
            connectedWorkspace.WorkspaceUpdated -= Workspace_WorkspaceUpdated;
            connectedWorkspace.WorkspaceConnectionError -= ConnectedWorkspace_WorkspaceConnectionError;
            connectedWorkspace.CueListChangedPlaybackPosition -= ConnectedWorkspace_CueListChangedPlaybackPosition;
        }

        private async void ConnectedWorkspace_WorkspaceConnectionError(object source, QWorkspaceConnectionErrorArgs args)
        {
            System.Console.WriteLine("WorkspaceConnectionError Called");
            if (args.status.Equals("badpass"))
            {
                string passcode = await DisplayPromptAsync("Passcode was incorrect.", "Try Again", maxLength: 4, keyboard: Keyboard.Numeric);
                if (passcode != null)
                {
                    connectedWorkspace.connect(passcode);
                }
            }
        }

        void ConnectedWorkspace_CueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                selectedCueGrid.BindingContext = new QCueViewModel(connectedWorkspace.cueWithID(args.cueID), false);

                if (cueGridDict.ContainsKey(args.cueID))
                {
                    var cueGrid = cueGridDict[args.cueID]; //element to scroll to
                    cueListScrollView.ScrollToAsync(cueGrid, ScrollToPosition.Center, true);
                }
            });
        }

        void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            foreach (var cue in connectedWorkspace.cueLists)
            {
                if(cue.cues.Count > 0)
                {
                    List<Task> cueAddTasks = new List<Task>();
                    foreach(var aCue in cue.cues)
                    {
                        Grid cueGrid = cueToGrid(aCue);
                        cueGridDict.Add(aCue.uid, cueGrid);
                        MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            cueListsGrid.Children.Add(cueGrid, 0, aCue.sortIndex);
                        }).Wait();
                    }
                    connectedWorkspace.valueForKey(cue, QOSCKey.PlaybackPositionId); //fetch playback position for cueList once all cue loading is done.
                    break; //only load first cue list with cues.
                }
            }
        }

        Grid cueToGrid(QCue cue)
        {
            Grid cueGrid = new Grid { RowSpacing = 0 };
            QCueViewModel qCueViewModel = new QCueViewModel(cue, true);
            cueGrid.RowDefinitions = new RowDefinitionCollection();
            cueGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            var cueLabel = new Label
            {
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };
            cueLabel.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);
            var cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.50,
                HasShadow = false,
                CornerRadius = 0,
                BorderColor = Color.Black
            };
            cueBackground.SetBinding(BackgroundColorProperty, "color",BindingMode.OneWay);

            var cueSelectedIndicator = new Frame
            {
                BindingContext = qCueViewModel,
                BackgroundColor = Color.Blue,
                HasShadow = false
            };
            cueSelectedIndicator.SetBinding(IsVisibleProperty, "IsSelected");

            cueGrid.Children.Add(cueSelectedIndicator, 0, 0);
            cueGrid.Children.Add(cueBackground, 0, 0);
            cueGrid.Children.Add(cueLabel, 0, 0);

            int rows = 1;
            if (cue.cues.Count > 0)
            {
                foreach (var aCue in cue.cues)
                {
                    cueGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var aCueGrid = cueToGrid(aCue);
                    cueGridDict.Add(aCue.uid, aCueGrid);
                    aCueGrid.Margin = new Thickness(10, 0, 0, 0);
                    cueGrid.Children.Add(aCueGrid, 0, aCue.sortIndex + 1);
                    rows++;
                }

                var cueFrame = new Frame
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Black
                };

                cueGrid.Children.Add(cueFrame);
                Grid.SetRowSpan(cueFrame, rows);

            }
            return cueGrid;
        }
    }
}