using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using QSharp;
using QSharpXamDemo.ViewModels;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QSharpXamDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CueListPage : ContentPage
    {
        private QWorkspace connectedWorkspace;
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
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                                cueListsGrid.Children.Add(cueGrid, 0, aCue.sortIndex);
                            });
                        });

                        cueAddTask.Start();
                        cueAddTasks.Add(cueAddTask);
                    }
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
                VerticalTextAlignment = TextAlignment.Center
            };
            cueLabel.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);
            var cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.75,
                HasShadow = false,
                CornerRadius = 0,
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