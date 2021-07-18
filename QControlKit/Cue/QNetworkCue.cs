using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QNetworkCue : QCue
    {
        public string customString
        {
            get
            {
                return propertyForKey(QOSCKey.CustomString).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.CustomString);
            }
        }

        public QNetworkMessageType messageType
        {
            get
            {
                return (QNetworkMessageType)propertyForKey(QOSCKey.MessageType);
            }
            set
            {
                setProperty(value, QOSCKey.MessageType);
            }
        }

        public QNetworkQLabCommand qlabCommand
        {
            get
            {
                return (QNetworkQLabCommand)propertyForKey(QOSCKey.QLabCommand);
            }
            set
            {
                setProperty(value, QOSCKey.QLabCommand);
            }
        }

        public int patch
        {
            get
            {
                return (int)propertyForKey(QOSCKey.Patch);
            }
            set
            {
                setProperty(value, QOSCKey.Patch);
            }
        }

        public string qlabCueNumber
        {
            get
            {
                return propertyForKey(QOSCKey.QLabCueNumber).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.QLabCueNumber);
            }
        }

        public string qlabCueParameters
        {
            get
            {
                return propertyForKey(QOSCKey.QLabCueParameters).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.QLabCueParameters);
            }
        }

        public string rawString
        {
            get
            {
                return propertyForKey(QOSCKey.RawString).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.RawString);
            }
        }

        public string udpString
        {
            get
            {
                return propertyForKey(QOSCKey.UdpString).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.UdpString);
            }
        }

    }
}
