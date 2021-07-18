using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QMicCue : QCue
    {
        public int channelOffset
        {
            get
            {
                return (int)propertyForKey(QOSCKey.ChannelOffset);
            }
        }

        public int channels
        {
            get
            {
                return (int)propertyForKey(QOSCKey.Channels);
            }
	        set
	        {
                    setProperty(value, QOSCKey.Channels);
	        }
        }
        
	    public void setDefaultLevels()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/setDefaultLevels");
        }

        public void setSilientLevels()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/setSilentLevels");
        }
    }
}
