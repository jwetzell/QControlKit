using QSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
namespace QSharpXamDemo.ViewModels
{
    public class QCueViewModel : INotifyPropertyChanged
    {
        QCue cue;
        bool isSelected = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public QCueViewModel(QCue cue)
        {
            this.cue = cue;
            this.cue.workspace.CueListChangedPlaybackPosition += Workspace_CueListChangedPlaybackPosition;
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
                return cue.displayName;
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
