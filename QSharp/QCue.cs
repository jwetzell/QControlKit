using System.Collections.Generic;

namespace QSharp
{
    public class QCue
    {
        Dictionary<string,object> cueData = new Dictionary<string,object>();
        public QCue()
        {
        }

        public object propertyForKey(string key)
        {
            if (cueData.ContainsKey(key))
                return cueData[key];
            return null;
        }
    }
}
