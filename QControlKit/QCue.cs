using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using QControlKit.Events;
using QControlKit.Constants;

namespace QControlKit
{
    public class QCue : IComparable
    {
        private ILogger _log = Log.Logger.ForContext<QCue>();

        public QWorkspace workspace;
        public Dictionary<string, object> cueData;
        public List<QCue> childCues;
        public Dictionary<string, QCue> childCuesUIDMap;


        public int sortIndex;
        public int nestLevel;

        private bool needsSortChildCues;//?
        private bool needsNotifyCueUpdated;//?

        public bool ignoreUpdates;

        public event QCuePropertiesUpdatedHandler CuePropertiesUpdated;

        public QCue()
        {
            init();
        }

        public QCue(QWorkspace workspace)
        {
            init();
            this.workspace = workspace;
        }

        public QCue(JToken dict, QWorkspace workspace)
        {
            init();
            this.workspace = workspace;
            updatePropertiesWithDictionary(dict);
        }

        public QCue(JToken dict, QWorkspace workspace, int nestLevel)
        {
            init();
            this.workspace = workspace;
            this.nestLevel = nestLevel + 1;
            updatePropertiesWithDictionary(dict);
        }

        public void init()
        {
            cueData = new Dictionary<string, object> {
                { QOSCKey.Flagged, false },
                { QOSCKey.Armed, true },
                { QOSCKey.PreWait, 0.0 },
                { QOSCKey.PercentPreWaitElapsed, 0.0 },
                { QOSCKey.PercentActionElapsed, 0.0 },
                { QOSCKey.PostWait, 0.0 },
                { QOSCKey.PercentPostWaitElapsed, 0.0 },
                { QOSCKey.IsPanicking, false },
                { QOSCKey.IsTailingOut, false },
                { QOSCKey.IsRunning, false },
                { QOSCKey.IsLoaded, false },
                { QOSCKey.IsPaused, false },
                { QOSCKey.IsBroken, false },
                { QOSCKey.IsOverridden, false },
                { QOSCKey.ContinueMode, 0 }
            };

            childCues = new List<QCue>();
            childCuesUIDMap = new Dictionary<string, QCue>();
        }

        public string description
        {
            get
            {
                return $"(Cue: {this}) name: {name} [id:{uid} number:{number} type: {type}";
            }
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return isEqualToCue((QCue)obj);
            }
        }

        public override int GetHashCode()
        {
            return uid.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
                return sortIndex.CompareTo(((QCue)obj).sortIndex);
            else
                return 0;
        }

        public bool isEqualToCue(QCue cue) {
            return cue.uid.Equals(uid);
        }

        public void setWorkspace(QWorkspace workspace)
        {
            if(this.workspace != workspace)
            {
                this.workspace = workspace;
            }
        }

        public void addChildCue(QCue cue)
        {
            string childUid = propertyForKey(QOSCKey.UID).ToString();
            addChildCue(cue, childUid);
        }

        public void addChildCue(QCue cue, string uid)
        {
            if (uid.Length == 0)
                return;
            childCues.Add(cue);
            childCuesUIDMap.Add(uid, cue);
            //some sorting of the childCues needs to be done? //TODO
            //reset sorting index?
        }

        public void removeChildCue(QCue cue) {
            if (cue.uid.Length == 0)
                return;

            childCues.Remove(cue);
            childCuesUIDMap.Remove(cue.uid);
        }

        public void removeAllChildCues() {
            childCuesUIDMap.Clear();
            childCues.Clear();
        }

        public void removeChildCuesWithIDs(List<string> uids) {
            foreach (string aUid in uids)
            {
                if (!childCuesUIDMap.ContainsKey(aUid))
                    continue;

                QCue cue = childCuesUIDMap[aUid];

                childCues.Remove(cue);
                childCuesUIDMap.Remove(aUid);
            }
        }
        
        //copied: yes
        //implemented: no
        //TODO
        #region Class Methods
        public string iconForType(string type) { throw new NotImplementedException(); }
        public List<string> fadeModeTitles() { throw new NotImplementedException(); }
        #endregion

        #region Convenience Accessors
        public List<QCue> cues
        {
            get
            {
                List<QCue> cues = (List<QCue>)propertyForKey(QOSCKey.Cues);
                if (cues == null)
                    return new List<QCue>();
                return cues;
            }

            set
            {
                setProperty(value, QOSCKey.Cues);
            }
        }
        public string parentID
        {
            get
            {
                if(propertyForKey(QOSCKey.Parent) != null)
                {
                    return propertyForKey(QOSCKey.Parent).ToString();
                }
                else
                {
                    return "";
                }
            }
        }
        public string playbackPositionID { 
            get {
                if (propertyForKey(QOSCKey.PlaybackPositionId) == null)
                    return null;
                else
                    return propertyForKey(QOSCKey.PlaybackPositionId).ToString(); 
            }
            set
            {
                setProperty(value, QOSCKey.PlaybackPositionId);
            }
        }
        public string name
        {
            get
            {
                return propertyForKey(QOSCKey.Name).ToString();
            }

            set
            {
                setProperty(value, QOSCKey.Name);
            }
        }
        public string number
        {
            get
            {
                object num = propertyForKey(QOSCKey.Number);
                if (num != null)
                    return num.ToString();
                else
                    return "";
            }
            set
            {
                setProperty(value, QOSCKey.Number);
            }
        }
        public string uid
        {
            get
            {
                return propertyForKey(QOSCKey.UID).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.UID);
            }
        }
        public string listName
        {
            get
            {
                if(propertyForKey(QOSCKey.ListName) == null)
                {
                    return "";
                }
                else
                {
                    return propertyForKey(QOSCKey.ListName).ToString();
                }
            }
            set
            {
                setProperty(value, QOSCKey.ListName);
            }
        }
        public string type
        {
            get
            {
                return propertyForKey(QOSCKey.Type).ToString();
            }
            set
            {
                setProperty(value, QOSCKey.Type);
            }
        }
        public string notes
        {
            get
            {
                if (propertyForKey(QOSCKey.Notes) != null)
                    return propertyForKey(QOSCKey.Notes).ToString();
                else
                    return "";
            }
            set
            {
                setProperty(value, QOSCKey.Notes);
            }
        }

        //Check the bool casting on these?
        public bool IsFlagged
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.Flagged);
            }
            set
            {
                setProperty(value, QOSCKey.Flagged);
            }
        }
        public bool IsOverridden
        {
            get
            {
                if (workspace.connectedToQLab3 || workspace.isOlderThanVersion("4.2.0"))
                    return false;

                foreach (var cue in cues)
                {
                    if (cue.IsOverridden)
                        return true;
                }

                return getBoolForKey(QOSCKey.IsOverridden);
            }
        }
        public bool IsBroken
        {
            get
            {
                foreach (var cue in cues)
                {
                    if (cue.IsBroken)
                        return true;
                }

                return getBoolForKey(QOSCKey.IsBroken);
            }
        }
        public bool IsRunning
        {
            get
            {
                foreach (var cue in cues)
                {
                    if (cue.IsRunning)
                        return true;
                }

                return getBoolForKey(QOSCKey.IsRunning);
            }
        }
        public bool IsTailingOut
        {
            get
            {
                if (workspace.connectedToQLab3)
                    return false;

                foreach (var cue in cues)
                {
                    if (cue.IsTailingOut)
                        return true;
                }

                return getBoolForKey(QOSCKey.IsTailingOut);
            }
        }
        public bool IsPanicking
        {
            get
            {
                if (workspace.connectedToQLab3)
                    return false;

                foreach (var cue in cues)
                {
                    if (cue.IsPanicking)
                        return true;
                }

                return getBoolForKey(QOSCKey.IsPanicking);
            }
        }
        public bool IsGroup 
        {
            get
            {
                if (type.Equals(QCueType.Group))
                    return true;
                if (type.Equals(QCueType.CueList))
                    return true;
                if (type.Equals(QCueType.Cart))
                    return true;
                return false;
            }
        }
        public bool IsCueList
        {
            get
            {
                if ( type.Equals(QCueType.CueList))
                    return true;
                return false;
            }
        }
        public string displayName
        {
            get
            {
                string number = this.number;
                if(number != null)
                {
                    if (number.Length > 0)
                        return $"{number} \u00b7 {nonEmptyName}";
                    else
                        return nonEmptyName;
                }
                else
                    return nonEmptyName;
            }
        }
        public string nonEmptyName
        {
            get
            {
                string nonEmptyName;
                if (name != null && !name.Equals(""))
                    nonEmptyName = name;
                else if (listName != null && !listName.Equals(""))
                    nonEmptyName = listName;
                else
                    nonEmptyName = $"Untitled {type} Cue";

                return nonEmptyName;
            }
        }
        public string workspaceName{ get { return workspace.name; } }
        public double currentDuration
        {
            get
            {
                //try v4 current duration key first
                if (propertyForKey(QOSCKey.CurrentDuration) != null)
                    return (double)propertyForKey(QOSCKey.CurrentDuration);
                else
                    return (double)propertyForKey(QOSCKey.Duration);
            }
        }
        public string audioFadeModeName
        {
            get
            {
                //TODO
                if (type.Equals(QCueType.Fade))
                    return "";
                else
                    return null;
            }
        }
        public string geoFadeModeName
        {
            get
            {
                //TODO
                if (type.Equals(QCueType.Fade))
                    return "";
                else
                    return null;
            }
        }
        public string surfaceName{ 
            get {
                object property = propertyForKey("surfaceName");
                if (property == null)
                    return null;
                else
                    return propertyForKey("surfaceName").ToString(); 
            } 
        }
        public string patchName
        {
            get
            {
                object property = propertyForKey("patchDescription");
                if (property == null)
                    return null;
                else
                    return propertyForKey("patchDecription").ToString();
            }
        }

        public QColor color { 
            get {
                return new QColor(colorName);
            }

            set
            {
                colorName = value.name;
            }
        }

        public string colorName
        {
            get
            {
                object col = propertyForKey(QOSCKey.ColorName);
                if (col != null)
                    return col.ToString();
                else
                    return "none";
            }
            set
            {
                if (value == null)
                    setProperty("none", QOSCKey.ColorName);
                else
                    setProperty(value, QOSCKey.ColorName);
            }
        }


        public float actionElapsed
        {
            get
            {
                object secs = propertyForKey(QOSCKey.ActionElapsed);
                if (secs != null)
                {
                    return float.Parse(secs.ToString());
                }
                else
                {
                    return 0F;
                }
            }
        }

        public QTriggerAction secondTriggerAction
        {
            get
            {
                return (QTriggerAction)propertyForKey(QOSCKey.SecondTriggerAction);
            }
            set
            {
                setProperty(value, QOSCKey.SecondTriggerAction);
            }
        }

        public bool secondTriggerOnRelease
        {
            get
            {
                var val = (int)propertyForKey(QOSCKey.SecondTriggerOnRelease);
                if (val == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    setProperty(0, QOSCKey.SecondTriggerOnRelease);
                }
                else
                {
                    setProperty(1, QOSCKey.SecondTriggerOnRelease);
                }
            }
        }

        //TODO add quaternion property and setter
        public Size surfaceSize() { throw new NotImplementedException(); }
        public Size cueSize() { throw new NotImplementedException(); }
        public List<string> availableSurfaceName() { throw new NotImplementedException(); }
        public List<string> propertyKeys
        {
            get
            {
                return cueData.Keys.ToList();
            }
        }

        public bool hasChildren
        {
            get
            {
                return cues.Count > 0;
            }
        }

        public QCue firstCue
        {
            get
            {
                return cues.First();
            }
        }

        public QCue lastCue
        {
            get
            {
                return cues.Last();
            }
        }


        public void setIgnoreUpdates(bool ignoreUpdates)
        {
            if(this.ignoreUpdates != ignoreUpdates)
            {
                this.ignoreUpdates = ignoreUpdates;

                if (!this.ignoreUpdates)
                {
                    //TODO: This should be enough?
                    workspace.fetchBasicPropertiesForCue(this);
                }
            }
        }
        #endregion

        //Copied: Yes
        //Implemented: Not all
        #region Update Methods
        public bool updatePropertiesWithDictionary(JToken dict)
        {
            bool cueUpdated = false;

            //TODO: pretty sure this is done
            JObject dictObj = (JObject)dict;
            List<string> propertiesUpdated = new List<string>();
            foreach (var obj in dictObj)
            {
                JToken value = obj.Value;
                if (obj.Key.Equals(QOSCKey.Cues))
                {
                    if (value.Type != JTokenType.Array)
                        continue;
                    updateChildCuesWithPropertiesArray(value, false);
                }
                else if(obj.Key.Equals(QOSCKey.Children) && IsGroup)
                {
                    if (value.Type != JTokenType.Array)
                        continue;
                    updateChildCuesWithPropertiesArray(value, true);
                    propertiesUpdated.Add(obj.Key);
                    cueUpdated = true;
                }
                else
                {
                    bool didSetProptery = setProperty(value, obj.Key, false);
                    if (didSetProptery)
                    {
                        cueUpdated = true;
                        propertiesUpdated.Add(obj.Key);
                    }
                }
            }

            if (!cueUpdated)
                return false;
            OnCuePropertiesUpdated(propertiesUpdated);
            return true;
        }

        //enqueue updated notification? //TODO
        public bool updateChildCuesWithPropertiesArray(JToken value, bool removeUnused)
        {
            if (!workspace.connected)
                return false;

            List<string> previousUids = new List<string>();

            if (removeUnused)
            {
                previousUids = allChildCueUids();
            }

            int index = 0;

            foreach (var dict in value)
            {
                string childUid = (string)dict[QOSCKey.UID];
                if (childUid == null)
                    continue;

                if (removeUnused)
                {
                    previousUids.Remove(childUid);
                }

                QCue child = cueWithID(childUid, false);

                if (child != null)
                {
                    bool didUpdateProperties = child.updatePropertiesWithDictionary(dict);
                    if (didUpdateProperties)
                        needsNotifyCueUpdated = true;

                    if (child.sortIndex != index)
                    {
                        child.sortIndex = index;

                        needsSortChildCues = true;
                        needsNotifyCueUpdated = true;
                    }
                }
                else
                {
                    child = new QCue(dict, workspace, nestLevel);
                    child.sortIndex = index;
                    addChildCue(child, childUid);
                    needsNotifyCueUpdated = true;
                }
                index++;
            }


            if (previousUids.Count() > 0 && removeUnused)
            {
                removeChildCuesWithIDs(previousUids);
            }

            if (needsSortChildCues)
            {
                childCues.Sort((q1, q2)=> q1.sortIndex.CompareTo(q2.sortIndex));
                needsSortChildCues = false;
            }

            return needsNotifyCueUpdated;
        }
        #endregion


        #region Children Cues
        public List<string> allChildCueUids()
        {
            List<string> uids = new List<string>(childCuesUIDMap.Keys);
            return uids;
        }
        public QCue cueAtIndex(int index)
        {
            if (index < 0 || index >= cues.Count)
                return null;
            return cues[index];
        }
        public QCue cueWithID(string uid)
        {
            return cueWithID(uid, true);
        }
        public QCue cueWithID(string uid, bool includeChildren)
        {
            if(childCuesUIDMap.ContainsKey(uid))
                return childCuesUIDMap[uid];

            if (!includeChildren)
                return null;

            foreach(var cue in cues)
            {
                if (!cue.IsGroup)
                    continue;
                QCue childCue = cue.cueWithID(uid, includeChildren);
                if (childCue != null)
                    return childCue;
            }
            return null;
        }
        public QCue cueWithNumber(string number)
        {
            foreach(var cue in cues)
            {
                if (cue.number.Equals(number))
                    return cue;
                if (cue.IsGroup)
                {
                    QCue childCue = cue.cueWithNumber(number);
                    if (childCue != null)
                        return childCue;
                }
            }
            return null;
        }

        public bool getBoolForKey(string key)
        {
            if (propertyForKey(key) != null)
            {
                object prop = propertyForKey(key);
                //_log.Warning($"{listName} IsBroken: {propertyForKey(QOSCKey.IsBroken).ToString()} {prop.GetType().ToString()}");
                if(prop.GetType() == typeof(Boolean)){
                    return (bool)prop;
                }
                else if (prop.GetType() == typeof(JValue))
                {
                    JValue value = (JValue)prop;
                    return (bool)value.Value;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object propertyForKey(string key)
        {
            //TODO: implerment special key checks color, surfaceName, patchDescription, 
            if (key.Equals(QOSCKey.Cues))
            {
                return childCues;
            }
            else if (key.Equals("surfaceName"))
            {
                //TODO
            }
            else if (key.Equals("patchDescription"))
            {
                //TODO
            }
            else if (cueData.ContainsKey(key))
            {
                return cueData[key];
            }
            return null;
        }

        public bool setProperty(object value, string key)
        {
            return setProperty( value,  key, workspace.defaultSendUpdatesOSC);
        }

        public bool setProperty(object value, string key, bool osc)
        {
            object existingValue = null;
            if(cueData.ContainsKey(key))
                existingValue = cueData[key];

            if(existingValue != null)
            {
                if (existingValue == value || existingValue.Equals(value))
                    return false;


                if (workspace.connectedToQLab3 && key.Equals(QOSCKey.Type) && existingValue.Equals(QCueType.CueList) && value.Equals(QCueType.Group))
                {
                    return false;
                }
            }

            if (key.Equals(QOSCKey.Cues))
            {
                if (value.GetType() != typeof(List<QCue>))
                    return false;


                Dictionary<string,QCue> newChildCuesUIDMap = new Dictionary<string, QCue>();
                int index = 0;
                string childUid = null;

                foreach(var aCue in (List<QCue>)value)
                {
                    childUid = aCue.uid;
                    if (childUid == null)
                        continue;
                    aCue.sortIndex = index;
                    newChildCuesUIDMap.Add(childUid, aCue);
                    index++;
                }
                childCues = (List<QCue>)value;
                childCuesUIDMap = newChildCuesUIDMap;

            }
            else if (key.Equals(QOSCKey.Children))
            {
                _log.Debug($"Children Cues property updated for: {this.displayName} : {this.uid}");
            }
            else if(key.Equals(QOSCKey.PlaybackPositionId))
            {
                if (!IsCueList)
                    return false;
                if (value != null && value.GetType() != typeof(string))
                    return false;

                if (value.Equals("none"))
                    value = null;
                if (value != null)
                    cueData[key] = value;
                else
                    cueData.Remove(key);

            }
            else
            {
                if (value != null)
                    cueData[key] = value;
                else
                    cueData.Remove(key);
            }


            if (key.Equals(QOSCKey.Type))
            {
                //TODO: something icon related?
            }

            if (osc)
            {
                if (playbackPositionID == null && key.Equals(QOSCKey.PlaybackPositionId) && workspace.isOlderThanVersion("4.2.0"))
                    value = "none";
                workspace.updatePropertySend(this, value, key);
            }
            return true;
        }

        #endregion

        public void sendAllPropertiesToQLab() {
            List<string> allProperties = propertyKeys;
            foreach(var key in allProperties)
            {
                object property = propertyForKey(key);
                if (key.Equals(QOSCKey.Cues))
                {
                    foreach(var cue in (List<QCue>)property)
                    {
                        cue.sendAllPropertiesToQLab();
                    }
                }
                else
                {
                    workspace.updatePropertySend(this, property, key);
                }
            }
        
        }

        //TODO: I don't think I'll do this one
        public void pullDownPropertyForKey(string key) { }
        

        #region Actions
        public void start() { workspace.startCue(this);  }
        public void stop() { workspace.stopCue(this);  }
        public void pause() { workspace.pauseCue(this);  }
        public void reset() { workspace.resetCue(this);  }
        public void load() { workspace.loadCue(this);  }
        public void resume() { workspace.resumeCue(this);  }
        public void hardStop() { workspace.hardStopCue(this);  }
        public void hardPause() { workspace.hardPauseCue(this);  }
        public void togglePause() { workspace.togglePauseCue(this);  }
        public void preview() { workspace.previewCue(this);  }
        public void panic() { workspace.panicCue(this); }
        public void playheadNext()
        {
            if (this.type == QCueType.CueList)
            {
                workspace.sendMessageForCue(this, "playhead/next");
            }
        }
        public void playheadPrevious()
        {
            if (this.type == QCueType.CueList)
            {
                workspace.sendMessageForCue(this, "playhead/previous");
            }
        }
        #endregion



        #region Event Handling
        void OnCuePropertiesUpdated(List<string> properties)
        {
            _log.Verbose("OnCuePropertiesUpdated");
            if(parentID != null && parentID != "")
            {
                QCue parentCue = workspace.cueWithID(parentID);
                if(parentCue != null)
                {
                    workspace.fetchBasicPropertiesForCue(workspace.cueWithID(parentID));
                }
                else
                {
                    _log.Error($"parent cue with id: {parentID} can't be found this could be a problem");
                }
            }
            CuePropertiesUpdated?.Invoke(this, new QCuePropertiesUpdatedArgs { properties = properties });
        } 
        #endregion


        #region Printing
        public void Print()
        {
            Print(0);
        }

        public void Print(int level)
        {
            string indent = new string(' ', level*2);

            _log.Information($"{indent}\u00b7{displayName} - {uid}");
            if (IsGroup)
            {
                level++;

                if (cues.Count() > 0)
                {                    
                    foreach (var cue in cues)
                    {
                        cue.Print(level);
                    }
                }
            }
        }

        
        #endregion
    }
}
