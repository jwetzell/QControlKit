
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using QSharp;
using QSharpXamDemo.ViewModels;
using Serilog;
using Xamarin.Forms;

namespace QSharpXamDemo
{
    public partial class CueListPage : ContentPage
    {
        private QWorkspace connectedWorkspace;
        private Dictionary<string, Grid> cueGridDict = new Dictionary<string, Grid>();
        public CueListPage(QWorkspace workspace)
        {
            InitializeComponent();

            connectedWorkspace = workspace;
            connectedWorkspace.WorkspaceUpdated += Workspace_WorkspaceUpdated;
            connectedWorkspace.connectWithPasscode("1234");
            connectedWorkspace.defaultSendUpdatesOSC = true;
            connectedWorkspace.CueListChangedPlaybackPosition += ConnectedWorkspace_CueListChangedPlaybackPosition;
        }

        void ConnectedWorkspace_CueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                selectedCueGrid.BindingContext = new SelectedQCueViewModel(connectedWorkspace.cueWithID(args.cueID));

                if(cueGridDict.ContainsKey(args.cueID))
                    cueListScrollView.ScrollToAsync(cueGridDict[args.cueID].X, cueGridDict[args.cueID].Y, true);
            });
        }

        void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            Log.Debug("[demo] Workspace has been updated");
            foreach (var cue in connectedWorkspace.cueLists)
            {
                if(cue.cues.Count > 0)
                {
                    List<Task> cueAddTasks = new List<Task>();
                    foreach(var aCue in cue.cues)
                    {
                        var cueAddTask = new Task(() =>
                        {
                            Grid cueGrid = cueToGrid(aCue);
                            cueGridDict.Add(aCue.uid, cueGrid);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                                cueListsGrid.Children.Add(cueGrid, 0, aCue.sortIndex);
                            });
                        });

                        cueAddTask.Start();
                        cueAddTasks.Add(cueAddTask);
                    }
                    Task.WhenAll(cueAddTasks).ContinueWith((task) =>
                    {
                        connectedWorkspace.valueForKey(cue, QOSCKey.PlaybackPositionId); //fetch playback position for cueList once all cue loading is done.
                    });
                    break; //only load first cue list with cues.
                }
            }
        }

        Grid cueToGrid(QCue cue)
        {
            Grid cueGrid = new Grid { RowSpacing = 0 };
            QCueViewModel qCueViewModel = new QCueViewModel(cue);
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
                Opacity = 0.60,
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