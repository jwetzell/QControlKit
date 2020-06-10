using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace QSharp
{
    public class QServerFoundArgs : EventArgs
    {
        public QServer server { get; set; }
    }
    public class QWorkspacesUpdatedArgs : EventArgs
    {
        public List<QWorkspaceInfo> Workspaces { get; set; }
    }

    public class QWorkspaceUpdatedArgs : EventArgs
    {

    }

    public class QWorkspaceSettingsUpdatedArgs : EventArgs
    {
        public string settingsType { get; set; }

    }

    public class QWorkspaceLightDashboardUpdatedArgs : EventArgs
    {

    }

    public class QWorkspaceConnectedArgs : EventArgs
    {

    }

    public class QWorkspaceDisconnectedArgs : EventArgs
    {

    }

    public class QWorkspaceConnectionErrorArgs : EventArgs
    {
        public string status { get; set; }
    }
    public class QQLabPreferencesUpdatedArgs : EventArgs
    {
        public string key { get; set; }

    }

    public class QCueUpdatedArgs : EventArgs
    {

    }

    public class QCueListsUpdatedArgs : EventArgs
    {
        public JToken data { get; set; }
    }

    public class QCueNeedsUpdatedArgs : EventArgs
    {
        public string cueID { get; set; }
    }

    public class QCueListChangedPlaybackPositionArgs : EventArgs
    {
        public string cueID { get; set; }
    }
}

