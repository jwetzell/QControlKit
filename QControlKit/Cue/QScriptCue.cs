using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QScriptCue : QCue
    {
        public string scriptSource
        {
            get
            {
                return propertyForKey(QOSCKey.ScriptSource).ToString();
            }
        }

        public void compileSource()
        {
            workspace.sendMessage($"/cue_id/{this.propertyForKey(QOSCKey.UID)}/compileSource");
        }
    }
}
