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
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.CollateAndStart}");
        }

        public void prune()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.Prune}");
        }

        public void pruneCommands()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.PruneCommands}");
        }

        public void removeLightCommandsMatching(string match)
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.RemoveLightCommandsMatching}", match);
        }

        public void replaceLightCommand(string oldCommand, string newCommand)
        {
            if(this.workspace.versionParts[0] == "4" && int.Parse(this.workspace.versionParts[1]) >= 4)
            {
                workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.ReplaceLightCommand}", oldCommand, newCommand);
            }
            else
            {
                workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.UpdateLightCommand}", oldCommand, newCommand);
            }
        }

        public void safeSort()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.SafeSort}");
        }

        public void safeSortCommands()
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.SafeSortCommands}");
        }

        public void setLight(string light, object setting)
        {
            workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.SetLight}", light, setting);
        }

        public void updateLightCommand(string oldCommand, string newCommand)
        {
            if (int.Parse(this.workspace.versionParts[0]) >= 4 && int.Parse(this.workspace.versionParts[1]) >= 4)
            {
                workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.ReplaceLightCommand}", oldCommand, newCommand);
            }
            else
            {
                workspace.sendMessage($"/cue_id/{this.uid}/{QOSCKey.UpdateLightCommand}", oldCommand, newCommand);
            }
        }

    }
}
