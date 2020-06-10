using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace QSharp
{
    public class QCue
    {

        public QWorkspace workspace;
        public Dictionary<string, object> cueData;
        public List<QCue> childCues;
        public Dictionary<string, QCue> childCuesUIDMap;

        public int sortIndex;

        private bool needsSortChildCues;//?
        private bool needsNotifyCueUpdated;//?

        public bool ignoreUpdates;


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

            JToken children = dict[QOSCKey.Cues];
            if(children != null && children.Type == JTokenType.Array)
            {
                //Console.WriteLine($"[cue] new cue being created with {children.Count()} childCues");
                foreach (var aChildDict in children)
                {
                    string uid = (string)aChildDict[QOSCKey.UID];
                    //Console.WriteLine($"[cue] childDict with uid: {uid} being processed");
                    if (uid == null)
                        continue;
                    
                    QCue child = new QCue(aChildDict, workspace);

                    childCues.Add(child);
                    childCuesUIDMap.Add(uid, child);
                }
            }

            updatePropertiesWithDictionary(dict);
        }

        public void init()
        {
            cueData = new Dictionary<string, object> {
                { QOSCKey.Flagged, false },
                { QOSCKey.Armed, false },
                { QOSCKey.PreWait, false },
                { QOSCKey.PercentPreWaitElapsed, false },
                { QOSCKey.PercentActionElapsed, false },
                { QOSCKey.PostWait, false },
                { QOSCKey.PercentPostWaitElapsed, false },
                { QOSCKey.IsPanicking, false },
                { QOSCKey.IsTailingOut, false },
                { QOSCKey.IsRunning, false },
                { QOSCKey.IsLoaded, false },
                { QOSCKey.IsPaused, false },
                { QOSCKey.IsBroken, false },
                { QOSCKey.IsOverridden, false },
                { QOSCKey.ContinueMode, false }
            };
            //TODO init cueData with some keys

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
        
        //isEqual?
        //hash?
        //compare?
        public bool isEqualToCue(QCue cue) { throw new NotImplementedException(); }
        public void setWorkspace(QWorkspace workspace)
        {
            if(this.workspace != workspace)
            {
                this.workspace = workspace;
            }
        }
        public void addChildCue(QCue cue)
        {
            string uid = propertyForKey(QOSCKey.UID).ToString();
            addChildCue(cue, uid);
        }
        public void addChildCue(QCue cue, string uid)
        {
            if (uid.Length == 0)
                return;
            childCues.Add(cue);
            childCuesUIDMap.Add(uid, cue);
            //some sorting of the childCues needs to be done?
            //reset sorting index
        }
        public void removeChildCue(QCue cue) { throw new NotImplementedException(); }
        public void removeAllChildCues() { throw new NotImplementedException(); }
        public void removeChildCuesWithIDs(List<string> uids) { throw new NotImplementedException(); }
        
        //copied: yes
        //implemented: no
        #region Class Methods
        public string iconForType(string type) { throw new NotImplementedException(); }
        public bool cueTypeIsAudio(string type) { throw new NotImplementedException(); }
        public bool cueTypeIsVideo(string type) { throw new NotImplementedException(); }
        public bool cueTypeIsGroup(string type) { throw new NotImplementedException(); }
        public bool cueTypeIsCueList(string type) { throw new NotImplementedException(); }
        public List<string> fadeModeTitles() { throw new NotImplementedException(); }
        #endregion

        //KVC Compliance... do I need this?

        //copied: yes
        //implemented: not all
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
        }
        public string parentID
        {
            get
            {
                return propertyForKey(QOSCKey.Parent).ToString();
            }
        }
        public string playbackPositionID() { throw new NotImplementedException(); }
        public string name
        {
            get
            {
                return propertyForKey(QOSCKey.Name).ToString();
            }
        }
        public string number
        {
            get
            {
                return propertyForKey(QOSCKey.Number).ToString();
            }
        }
        public string uid
        {
            get
            {
                return propertyForKey(QOSCKey.UID).ToString();
            }
        }
        public string listName
        {
            get
            {
                return propertyForKey(QOSCKey.ListName).ToString();
            }
        }
        public string type
        {
            get
            {
                return propertyForKey(QOSCKey.Type).ToString();
            }
        }
        public string notes
        {
            get
            {
                return propertyForKey(QOSCKey.Notes).ToString();
            }
        }

        public bool IsFlagged
        {
            get
            {
                return (bool)propertyForKey(QOSCKey.Flagged);
            }
        }
        public bool IsOverridden
        {
            get
            {
                return (bool)propertyForKey("OSCNameKey");
            }
        }
        public bool IsBroken
        {
            get
            {
                return (bool)propertyForKey("OSCNameKey");
            }
        }
        public bool IsRunning
        {
            get
            {
                return (bool)propertyForKey("OSCNameKey");
            }
        }
        public bool IsTailingOut
        {
            get
            {
                return (bool)propertyForKey("OSCNameKey");
            }
        }
        public bool IsPanicking
        {
            get
            {
                return (bool)propertyForKey("OSCNameKey");
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
                if (number.Length > 0)
                    return $"{number} \u00b7 {nonEmptyName}";
                else
                    return nonEmptyName;
            }
        }
        public string nonEmptyName
        {
            get
            {
                //TODO
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
        public string surfaceName{ get { return propertyForKey("surfaceName").ToString(); } }
        public string patchName
        {
            get
            {
                //TODO
                return (string)propertyForKey("patchDecription");
            }
        }

        //TODO: THIS IS WHERE THINGS START TO GET TRICKY!

        //TODO add object color method
        public string color { get { return propertyForKey(QOSCKey.ColorName).ToString(); } }

        //TODO add quaternion property and setter

        public Size surfaceSize() { throw new NotImplementedException(); }
        public Size cueSize() { throw new NotImplementedException(); }
        public List<string> availableSurfaceName() { throw new NotImplementedException(); }
        public List<string> propertyKeys
        {
            get
            {
                //TODO add null check?
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
                    //TODO: update local with QLab 
                }
            }
        }
        #endregion

        //Copied: Yes
        //Implemented: Not all
        #region Mutators
        public void setCues(List<QCue> cues)
        {
            
        }
        public void setName() { }
        public void setNumber() { }
        public void setUid() { }
        public void setListName() { }
        public void setType() { }
        public void setFlagged() { }
        #endregion

        //Copied: Yes
        //Implemented: Not all
        #region Update Methods
        public bool updatePropertiesWithDictionary(JToken dict)
        {
            return updatePropertiesWithDictionary(dict, true);
        }
        public bool updatePropertiesWithDictionary(JToken dict, bool notify)
        {
            bool cueUpdated = false;

            //TODO
            JObject dictObj = (JObject)dict;
            foreach (var obj in dictObj)
            {
                //Console.WriteLine($"[cue] found property {obj.Key} ");
                JToken value = obj.Value;
                if (obj.Key.Equals(QOSCKey.Cues))
                {
                    if (value.Type != JTokenType.Array)
                        continue;
                    updateChildCuesWithPropertiesArray(value, false);
                }
                else
                {
                    bool didSetProptery = setProperty(value, obj.Key, false);
                    if (didSetProptery)
                        cueUpdated = true;
                }
            }

            if (!cueUpdated)
                return false;
            if (notify)
                Console.WriteLine("should notify cue updated.");
            //something about notifying cueUpdated ? OnCueUpdated event handler maybe?

            return true;
        }

        //enqueue updated notification?
        public void updateChildCuesWithPropertiesArray(JToken value, bool removeUnused)
        {
            if (!workspace.connected)
                return;

            List<string> previousUids = null;
            if (removeUnused)
                previousUids = allChildCueUids();

            int index = 0;

            foreach (var dict in value)
            {
                string uid = (string)dict[QOSCKey.UID];
                if (uid == null)
                    continue;

                if (removeUnused)
                    previousUids.Remove(uid);

                QCue child = cueWithID(uid, false);

                if (child != null)
                {
                    bool didUpdateProperties = updatePropertiesWithDictionary(dict);
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
                    child = new QCue(dict, workspace);
                    child.sortIndex = index;
                    addChildCue(child, uid);
                    needsNotifyCueUpdated = true;

                }
                index++;
            }


            //TODO: TEST?!
        }
        #endregion



        //Methods Copied: Yes
        //Methods Implemented: Not all
        #region Children Cues
        public List<string> allChildCueUids()
        {
            //TODO
            return new List<string>();
        }
        public QCue cueAtIndex(int index)
        {
            //TODO
            return new QCue();
        }
        public QCue cueWithID(string uid)
        {
            return cueWithID(uid, true);
        }
        public QCue cueWithID(string uid, bool includeChildren)
        {
            //Console.WriteLine($"[cue] searching for cue with id: {uid} - includeChildren: {includeChildren}");
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
            //TODO
            return new QCue();
        }
        public object propertyForKey(string key)
        {
            //TODO: implerment special key checks color, surfaceName, patchDescription, 
            if (key.Equals(QOSCKey.Cues))
            {
                return childCues;
            }
            if (cueData.ContainsKey(key))
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
            //TODO:
            object existingValue = null;
            if(cueData.ContainsKey(key))
                existingValue = cueData[key];

            if(existingValue != null)
            {
                if (existingValue == value || existingValue.Equals(value))
                {
                    //Console.WriteLine($"[cue] skipping set property: {key} with type: {value.GetType()} and value: {value}");
                    return false;
                }
            }

            //Console.WriteLine($"[cue] attempting set property: {key} with type: {value.GetType()} and value: {value}");



            if (workspace.connectedToQLab3 && key.Equals(QOSCKey.Type) && existingValue.Equals(QCueType.CueList) && value.Equals(QCueType.Group))
            {
                return false;
            }


            if (key.Equals(QOSCKey.Cues))
            {
                if (value.GetType() != typeof(List<QCue>))
                    return false;


                Dictionary<string,QCue> newChildCuesUIDMap = new Dictionary<string, QCue>();
                int index = 0;
                string uid = null;

                foreach(var aCue in (List<QCue>)value)
                {
                    uid = aCue.uid;
                    if (uid == null)
                        continue;
                    aCue.sortIndex = index;
                    newChildCuesUIDMap.Add(uid, aCue);
                    index++;
                }
                childCues = (List<QCue>)value;
                childCuesUIDMap = newChildCuesUIDMap;

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

            //Console.WriteLine($"[cue] set property: {key} with type: {value.GetType()} and value: {value}");
            //check for type key and update icon? do I want to do this here?
            return true;
        }

        #endregion

        //TODO
        public void sendAllPropertiesToQLab() { }

        //TODO
        public void pullDownPropertyForKey(string key) { }
        
        //TODO
        public void setPlaybackPositionID(string cueID, bool osc) { }

        //Methods Copied: Yes
        //Methods Implemented: Yes
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
        #endregion

        #region Printing
        public void Print()
        {
            if (IsGroup)
            {
                Console.WriteLine($"{displayName} : {color}");
                if (cues.Count() > 0)
                {
                    foreach(var cue in cues)
                    {
                        cue.Print();
                    }
                }
            }
            else
            {
                Console.WriteLine($"{displayName} : {color}");
            }
        }
        #endregion
    }
}
