namespace QControlKit.Constants
{
    public static class QBonjour
    {
        public const string TCPService = "_qlab._tcp.local.";
        public const string UDPService = "_qlab._udp.local.";
    }

    public static class QCueType
    {
        public const string Cue = "Cue";
        public const string CueList = "Cue List";
        public const string Cart = "Cart";
        public const string Group = "Group";
        public const string Audio = "";
        public const string Mic = "Mic";
        public const string Video = "Video";
        public const string Camera = "Camera";
        public const string Text = "Text";
        public const string Light = "Light";
        public const string Fade = "Fade";
        public const string Network = "Network";
        public const string MIDI = "MIDI";
        public const string MIDIFile = "MIDI File";
        public const string Timecode = "Timecode";
        public const string Start = "Start";
        public const string Stop = "Stop";
        public const string Pause = "Paus";
        public const string Load = "Load";
        public const string Reset = "Reset";
        public const string Devamp = "Devamp";
        public const string Goto = "GoTo";
        public const string Target = "Target";
        public const string Arm = "Arm";
        public const string Disarm = "Disarm";
        public const string Wait = "Wait";
        public const string Memo = "Memo";
        public const string Script = "Script";
        public const string Stagetracker = "Stagetracker";

        //v3
        public const string OSC = "OSC";
        public const string Titles = "Titles";

    }

    public static class QOSCKey
    {
        public const string UID = "uniqueID";
        public const string Type = "type";
        public const string Parent = "parent";
        public const string Name = "name";
        public const string Number = "number";
        public const string Notes = "notes";
        public const string FileTarget = "fileTarget";
        public const string CueTargetNumber = "cueTargetNumber";
        public const string CurrentCueTarget = "currentCueTarget";
        public const string ColorName = "colorName";
        public const string Flagged = "flagged";
        public const string Armed = "armed";
        public const string ContinueMode = "continueMode";
        public const string PreWait = "preWait";
        public const string PostWait = "postWait";
        public const string CurrentDuration = "currentDuration";
        public const string PercentPreWaitElapsed = "percentPreWaitElapsed";
        public const string PercentPostWaitElapsed = "percentPostWaitElapsed";
        public const string PercentActionElapsed = "percentActionElapsed";
        public const string PreWaitElapsed = "preWaitElapsed";
        public const string PostWaitElapsed = "postWaitElapsed";
        public const string ActionElapsed = "actionElapsed";
        public const string GroupMode = "mode";
        public const string CartPosition = "cartPosition";
        public const string CartRows = "cartRows";
        public const string CartColumns = "cartColumns";
        public const string HasFileTargets = "hasFileTargets";
        public const string HasCueTargets = "hasCueTargets";
        public const string AllowsEditingDuration = "allowsEditingDuration";
        public const string IsPanicking = "isPanicking";
        public const string IsTailingOut = "isTailingOut";
        public const string IsRunning = "isRunning";
        public const string IsLoaded = "isLoaded";
        public const string IsPaused = "isPaused";
        public const string IsBroken = "isBroken";
        public const string IsOverridden = "isOverridden";
        public const string TranslationX = "translationX";
        public const string TranslationY = "translationY";
        public const string ScaleX = "scaleX";
        public const string ScaleY = "scaleY";
        public const string OriginX = "originX";
        public const string OriginY = "originY";
        public const string Quaternion = "quaternion";
        public const string SurfaceSize = "surfaceSize";
        public const string CueSize = "cueSize";
        public const string PreserveAspectRatio = "preserveAspectRatio";
        public const string Layer = "layer";
        public const string Patch = "patch";
        public const string PatchList = "patchList";
        public const string SurfaceList = "surfaceList";
        public const string Cues = "cues";
        public const string Children = "children";
        public const string ListName = "listName";
        public const string SurfaceID = "surfaceID";
        public const string FullSurface = "fullSurface";
        public const string Opacity = "opacity";
        public const string RotationZ = "rotationZ";
        public const string RotationY = "rotationY";
        public const string RotationX = "rotationX";
        public const string PlaybackPositionId = "playbackPositionId";
        public const string StartNextCueWhenSliceEnds = "startNextCueWhenSliceEnds";
        public const string StopTargetWhenSliceEnds = "stopTargetWhenSliceEnds";
        public const string SliderLevel = "sliderLevel";

        // v3
        public const string Duration = "duration";
        public const string FullScreen = "fullScreen";
    }

    public static class QIdentifiers
    {
        // Identifiers for "fake" cues
        public const string RootCue = "__root__";
        public const string ActiveCues = "__active__";
        public const string NoSelection = "none";
        public const string RootCueUpdate = "[root group of cue lists]";
    }


    enum QFadeMode
    {
        Absolute = 0,
        Relative = 1
    }

    enum QContinueMode
    {
        NoContinue = 0,
        AutoContinue = 1,
        AutoFollow = 2
    }
}
