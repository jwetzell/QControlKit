using System;
using System.Collections.Generic;
using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QGroupCue : QCue
    {
        public int cartColumns
        {
            get
            {
                return (int)propertyForKey(QOSCKey.CartColumns);
            }
            set
            {
                setProperty(value, QOSCKey.CartColumns);
            }
        }

        public int cartRows
        {
            get
            {
                return (int)propertyForKey(QOSCKey.CartRows);
            }
            set
            {
                setProperty(value, QOSCKey.CartRows);
            }
        }

        public List<QCue> children
        {
            get
            {
                return cues;
            }
        }

        public void go()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.Go}");
        }

        public int mode
        {
            get
            {
                return (int)propertyForKey(QOSCKey.Mode);
            }
            set
            {
                if(value > 0 && value < 5)
                {
                    setProperty(value, QOSCKey.Mode);
                }
            }
        }

        public void moveCartCue(QCue cart, int row, int column)
        {
            if(cart.type.Equals(QCueType.CueCart) || cart.type.Equals(QCueType.Cart))
            {
                workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.MoveCartCue}/{cart.uid}", row, column);
            }
        }

        public string playhead
        {
            get
            {
                return propertyForKey(QOSCKey.Playhead).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.Playhead);
            }
        }

        public string playheadId
        {
            get
            {
                return propertyForKey(QOSCKey.PlayheadId).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.PlayheadId);
            }
        }

        public string playbackPosition
        {
            get
            {
                return propertyForKey(QOSCKey.PlaybackPosition).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.PlaybackPosition);
            }
        }

        public string playbackPositionId
        {
            get
            {
                return propertyForKey(QOSCKey.PlaybackPositionId).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.PlaybackPositionId);
            }
        }
    }
}
