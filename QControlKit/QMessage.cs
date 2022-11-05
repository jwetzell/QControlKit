using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SharpOSC;

namespace QControlKit
{
    public class QMessage
    {

        private OscMessage OSCMessage;

        public QMessage(OscMessage oscMessage)
        {
            OSCMessage = oscMessage;
        }

        public string description
        {
            get
            {
                //string join probably won't work might need to cast argument objects to strings first?
                return $"address: {address}, arguments: {string.Join(" - ", arguments)}";
            }
        }

        #region Message Type Checks
        public bool IsReply { get { return OSCMessage.Address.StartsWith("/reply"); } }

        // /reply/cue_id/1/action
        public bool IsReplyFromCue {
            get {
                if (!IsReply)
                {
                    return false;
                }
                //return OSCMessage.Address.StartsWith("/reply/cue_id");
                if (AddressParts.Length > 5)
                {
                    // /reply/workspace/*/cue_id/*/*
                    if (AddressParts[1].Equals("workspace") && AddressParts[3].Equals("cue_id")){
                        return true;
                    }
                }

                if(AddressParts.Length > 2)
                {
                    // /reply/cue_id/*/*
                    if (AddressParts[1].Equals("cue_id"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsReplyFromCueLists { get { return IsReply && OSCMessage.Address.EndsWith("cueLists"); } }

        public bool IsUpdate { get{ return OSCMessage.Address.StartsWith("/update"); } }

        public bool IsWorkspaceUpdate { 
            get {
                // /update/workspace/{workspace_id}
                string[] parts = AddressParts;
                return (parts.Length == 3 && parts[1].Equals("workspace"));
            }
        }

        public bool IsWorkspacesInfo { 
            get {
                // /workspaces
                string[] parts = AddressParts;
                return (parts.Length == 2 && parts[1] == "workspaces"); 
            } 
        }

        public bool IsWorkspaceSettingsUpdate { 
            get {
                // /update/workspace/{workspace_id}/settings/{settings_controller}
                string[] parts = AddressParts;
                return (parts.Length == 5 && parts[1].Equals("workspace") && parts[3].Equals("settings"));
            } 
        }

        public bool IsLightDashboardUpdate { 
            get {
                // /update/workspace/{workspace_id}/dashboard
                string[] parts = AddressParts;
                return (parts.Length == 4 && parts[1].Equals("workspace") && parts[3].Equals("dashboard"));
            } 
        }

        public bool IsCueUpdate { 
            get {
                // /update/workspace/{workspace_id}/cue_id/{cue_id}
                string[] parts = AddressParts;
                return (parts.Length == 5 && parts[1].Equals("workspace") && parts[3].Equals("cue_id"));
            } 
        }

        public bool IsPlaybackPositionUpdate { 
            get { 
                // /update/workspace/{workspace_id}/cueList/{cue_list_id}/playbackPosition {cue_id}
                string[] parts = AddressParts;
                return (parts.Length == 6 && address.EndsWith("/playbackPosition"));
            } 
        }

        public bool IsPreferencesUpdate { 
            get { 
                string[] parts = AddressParts;
                return (parts.Length == 4 && parts[3].Equals("preferences"));
            } 
        }

        public bool IsDisconnect {
            get {
                string[] parts = AddressParts;
                return (parts.Length == 4 && parts[3].Equals("disconnect"));
            }
        }

        public bool IsConnect
        {
            get
            {
                string[] parts = AddressParts;
                return (parts.Length == 4 && parts[3].Equals("connect"));
            }
        }

        #endregion

        //host method

        public string address { get { return OSCMessage.Address; } }

        public string replyAddress { get{ return IsReply ? address.Substring("/reply".Length) : address; } }


        public string[] AddressParts { get { return address.Split(new char[] { '/' },StringSplitOptions.RemoveEmptyEntries); } }

        public JToken response
        {
            get
            {
                JObject responseObj = JObject.Parse((string)arguments[0]);
                return responseObj["data"];
            }
        }

        public List<Object> arguments { get { return OSCMessage.Arguments; } }

        public string cueID
        {
            get
            {
                if (IsCueUpdate) {
                    return AddressParts[4];
                } else if (IsPlaybackPositionUpdate) {
                    //TODO: check string cast
                    return arguments.Count > 0 ? (string)arguments[0] : null;
                } else if (IsReplyFromCue) {

                    // /reply/workspace/*/cue_id/*/*
                    if (AddressParts.Length > 5)
                    {
                        return AddressParts[4];
                    }
                    // /reply/cue_id/*/*
                    return AddressParts[2];
                } else {
                    return null;
                }
            }
        }

        override
        public string ToString()
        {
            string stringified = "";

            stringified += $"\nAddress: {this.address}\nArguments: \n";
           

            for(int i = 0; i<this.arguments.Count; i++)
            {
                object argument = this.arguments[i];
                stringified += $"\targ {i + 1}: {argument.ToString()}\n";
            }
            return stringified;
        }

    }
}
