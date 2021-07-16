using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QMidiFileCue :QCue
    {
        public bool patch
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.Patch);
            }
            set
            {
                setProperty(value, QOSCKey.Patch);
            }
        }

        public bool rate
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.Rate);
            }
            set
            {
                setProperty(value, QOSCKey.Rate);
            }
        }
    }
}
