using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QLightCue : QCue
    {
        public bool alwaysCollate
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.AlwaysCollate);
            }
            set
            {
                setProperty(value, QOSCKey.AlwaysCollate);
            }
        }

        public string lightCommandText
        {
            get
            {
                return propertyForKey(QOSCKey.LightCommandText).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.LightCommandText);
            }
        }

        public void collateAndStart()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.CollateAndStart}");
        }

        public void prune()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.Prune}");
        }

        public void pruneCommands()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.PruneCommands}");
        }

        public void removeLightCommandsMatching(string match)
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.RemoveLightCommandsMatching}", match);
        }

        public void replaceLightCommand(string oldCommand, string newCommand)
        {
            if(this.workspace.versionParts[0] == "4" && int.Parse(this.workspace.versionParts[1]) >= 4)
            {
                workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.ReplaceLightCommand}", oldCommand, newCommand);
            }
            else
            {
                workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.UpdateLightCommand}", oldCommand, newCommand);
            }
        }

        public void safeSort()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.SafeSort}");
        }

        public void safeSortCommands()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.SafeSortCommands}");
        }

        public void setLight(string light, object setting)
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.SetLight}", light, setting);
        }

        public void updateLightCommand(string oldCommand, string newCommand)
        {
            if (int.Parse(this.workspace.versionParts[0]) >= 4 && int.Parse(this.workspace.versionParts[1]) >= 4)
            {
                workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.ReplaceLightCommand}", oldCommand, newCommand);
            }
            else
            {
                workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/{QOSCKey.UpdateLightCommand}", oldCommand, newCommand);
            }
        }

    }
}
