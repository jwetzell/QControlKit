﻿//General information about a QLab Workspace retrieves by /workspaces
namespace QControlKit
{
    public class QWorkspaceInfo
    {
        public string version { get; set; }
        public string displayName { get; set; }
        public string uniqueID { get; set; }
        public bool hasPasscode { get; set; }
    }
}
