using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QDevampCue : QCue
    {
        public bool startNextCueWhenSliceEnds
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.startNextCueWhenSliceEnds);
            }
            set
            {
                setProperty(value, QOSCKey.startNextCueWhenSliceEnds);
            }
        }

        public bool stopTargetWhenSliceEnds
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.stopTargetWhenSliceEnds);
            }
            set
            {
                setProperty(value, QOSCKey.stopTargetWhenSliceEnds);
            }
        }
    }
}
