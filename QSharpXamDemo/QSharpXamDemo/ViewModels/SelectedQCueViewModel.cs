using QSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
namespace QSharpXamDemo.ViewModels
{
    public class SelectedQCueViewModel : INotifyPropertyChanged
    {
        QCue cue;
        public event PropertyChangedEventHandler PropertyChanged;

        public SelectedQCueViewModel(QCue cue)
        {
            this.cue = cue;
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string name { 
            get
            {
                return cue.nonEmptyName;
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
    }
}
