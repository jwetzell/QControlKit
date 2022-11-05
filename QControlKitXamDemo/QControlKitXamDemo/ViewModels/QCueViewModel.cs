using QControlKit;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Serilog;
using QControlKit.Events;
using QControlKit.Constants;
using System;

namespace QControlKitXamDemo.ViewModels
{
    public class QCueViewModel : INotifyPropertyChanged
    {
        private ILogger _log = Log.Logger.ForContext<QCueViewModel>();

        QCue cue;
        bool isSelected = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public QCueViewModel(QCue cue, bool checkPlayback)
        {
            this.cue = cue;
            if (checkPlayback)
            {
                this.cue.workspace.CueListChangedPlaybackPosition += Workspace_CueListChangedPlaybackPosition;
            }
            this.cue.CuePropertiesUpdated += OnCuePropertiesUpdated;
        }

        private void OnCuePropertiesUpdated(object source, QCuePropertiesUpdatedArgs args)
        {
            _log.Debug($"properties updated from cue");

            foreach (var property in args.properties)
            {
                _log.Debug($"property <{property}> has been updated for cue {name}.");
                if (property.Equals(QOSCKey.Name) || property.Equals(QOSCKey.ListName))
                {
                    OnPropertyChanged("name");
                }else if (property.Equals(QOSCKey.ColorName))
                {
                    OnPropertyChanged("color");
                }else if (property.Equals(QOSCKey.ActionElapsed))
                {
                    OnPropertyChanged("actionElapsed");
                }else if (property.Equals(QOSCKey.IsBroken) || property.Equals(QOSCKey.IsRunning))
                {
                    OnPropertyChanged("status");
                }
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Workspace_CueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            IsSelected = args.cueID == cue.uid;
        }

        public string name { 
            get
            {
                return cue.listName;
            }

            set
            {
                cue.name = value;
            }
        }

        public string number
        {
            get
            {
                return cue.number;
            }

            set
            {
                cue.number = value;
            }
        }

        public Color color
        {
            get
            {
                return Color.FromHex(cue.color.Hex);
            }
            set
            {
                cue.color = new QColor("none");
            }
        }

        public string actionElapsed
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(cue.actionElapsed);
                
                return time.ToString(@"mm\:ss\:ff");
            }
        }

        public string status
        {
            get
            {
                if (cue.IsRunning)
                {
                    return "running";
                }else if (cue.IsBroken)
                {
                    return "broken";
                }
                else
                {
                    return " ";
                }
            }
        }

        public bool IsSelected {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
