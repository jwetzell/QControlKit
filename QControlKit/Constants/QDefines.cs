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
        public const string ActionElapsed = "actionElapsed";
        public const string AllowsEditingDuration = "allowsEditingDuration";
        public const string Armed = "armed";
        public const string AutoLoad = "autoLoad"; //implement on QCue

        public const string CartPosition = "cartPosition";
        public const string Children = "children";
        public const string ColorName = "colorName";
        public const string Cues = "cues";
        public const string CueTargetNumber = "cueTargetNumber";
        public const string CueTargetId = "cueTargetId"; //implement on QCue
        
        public const string CurrentCueTarget = "currentCueTarget";
        public const string CurrentFileTime = "currentFileTime";  //implement on QCue
        public const string ContinueMode = "continueMode";
        public const string CurrentDuration = "currentDuration";

        public const string DefaultName = "defaultName"; //implement on QCue
        public const string DisplayName = "displayName"; //implement on QCue
        public const string DuckLevel = "duckLevel";  //implement on QCue
        public const string DuckOthers = "duckOthers"; //implement on QCue
        public const string DuckTime = "duckTime"; //implement on QCue


        public const string FileTarget = "fileTarget";
        public const string Flagged = "flagged";
        public const string FadeAndStopOthers = "fadeAndStopOthers"; //implement on QCue
        public const string FadeAndStopOthersTime = "fadeAndStopOthersTime"; //implement on QCue


        public const string HasFileTargets = "hasFileTargets";
        public const string HasCueTargets = "hasCueTargets";

        public const string IsActionRunning = "isActionRunning"; //implement on QCue
        public const string IsBroken = "isBroken";
        public const string IsLoaded = "isLoaded";
        public const string IsOverridden = "isOverridden";
        public const string IsPanicking = "isPanicking";
        public const string IsPaused = "isPaused";
        public const string IsRunning = "isRunning";
        public const string IsTailingOut = "isTailingOut";

        public const string ListName = "listName";
        public const string LoadAt = "loadAt"; //implement on QCue
        public const string LoadActionAt = "loadActionAt"; //implement on QCue

        public const string MaxTimeInCueSequence = "maxTimeInCueSequence";  //implement on QCue


        public const string Name = "name";
        public const string Notes = "notes";
        public const string Number = "number";

        public const string PanicInTime = "panicInTime"; //implement on QCue
        public const string PreWait = "preWait";
        public const string PostWait = "postWait";
        public const string PercentPreWaitElapsed = "percentPreWaitElapsed";
        public const string PercentPostWaitElapsed = "percentPostWaitElapsed";
        public const string PercentActionElapsed = "percentActionElapsed";
        public const string PreWaitElapsed = "preWaitElapsed";
        public const string Parent = "parent";
        public const string PostWaitElapsed = "postWaitElapsed";

        
        public const string StartNextCueWhenSliceEnds = "startNextCueWhenSliceEnds";
        public const string StopTargetWhenSliceEnds = "stopTargetWhenSliceEnds";
        public const string SecondTriggerAction = "secondTriggerAction"; //implement on QCue
        public const string SecondTriggerOnRelease = "secondTriggerOnRelease";  //implement on QCue
        public const string SoloCueInTime = "soloCueInTime";  //implement on QCue

        
        public const string TempCueTargetNumber = "tempCueTargetNumber"; //implement on QCue
        public const string TempCueTargetId = "tempCueTargetId"; //implement on QCue
        public const string TempDuration = "tempDuration"; //implement on QCue
        public const string Type = "type";

        public const string UID = "uniqueID";
        public const string UniqueID = "uniqueID";
        public const string ValuesForKeys = "valuesForKeys";
        public const string ValuesForKeysWithArguments = "valuesForKeysWithArguments";

        //Group Cue Keys
        public const string CartColumns = "cartColumns";
        public const string CartRows = "cartRows";
        public const string GroupMode = "mode";
        public const string PlaybackPositionId = "playbackPositionId";
        public const string PlaybackPosition = "playbackPosition"; //Implement on Group Cue
        public const string PlayheadId = "playheadId"; //Implement on Group Cue
        public const string Playhead = "playhead"; //Implement on Group Cue

        //Audio Cue Keys
        public const string DoFade = "doFade"; //Implement on Audio Cue and Video Cue
        public const string DoPitchShift = "doPitchShift"; //Implement on Audio Cue and Video Cue
        public const string EndTime = "endTime";  //Implement on Audio Cue and Video Cue
        public const string Gang = "gang"; //Implement on Audio Cue and Video Cue
        public const string InfiniteLoop = "infiniteLoop"; //Implement on Audio Cue and Video Cue
        public const string Level = "level"; //Implement on Audio Cue and Video Cue
        public const string Levels = "levels"; //Implement on Audio Cue and Video Cue
        public const string LiveAverageLevel = "liveAverageLevel"; //Implement on Audio Cue and Video Cue
        public const string LockFadeToCue = "lockFadeToCue"; //Implement on Audio Cue and Video Cue
        public const string NumChannelsIn = "numChannelsIn"; //Implement on Audio Cue and Video Cue
        public const string PatchList = "patchList"; //Implement on Audio Cue and Video Cue
        public const string PlayCount = "playCount"; //Implement on Audio Cue and Video Cue
        public const string LiveRate = "liveRate"; //Implement on Audio Cue and Video Cue
        public const string SliceMarkers = "sliceMarkers"; //Implement on Audio Cue and Video Cue
        public const string SliceMarker = "sliceMarker"; //Implement on Audio Cue and Video Cue
        public const string Time = "time"; //Implement on Audio Cue related to slicemarker and Video Cue
        public const string LastSlicePlayCount = "lastSlicePlayCount"; //Implement on Audio Cue and Video Cue
        public const string LastSliceInfiniteLoop = "lastSliceInfiniteLoop"; //Implement on Audio Cue and Video Cue
        public const string SliderLevel = "sliderLevel"; // Implement on Video Cue 
        public const string SliderLevels = "sliderLevels"; //Implement on Audio Cue and Video Cue
        public const string StartTime = "startTime"; // Implement on Audio Cue and Video Cue

        //Mic Cue Keys
        public const string ChannelOffset = "channelOffset"; //Implement on Mic Cue
        public const string Channels = "channels"; //Implement on Mic Cue

        //Video Cue Keys
        public const string CueSize = "cueSize"; //Implement on Video Cue
        public const string DoEffect = "doEffect"; //Implement on Video Cue
        public const string EffectIndex = "effectIndex"; //Implement on Video Cue
        public const string EffectSet = "effectSet"; //Implement on Video Cue
        public const string liveEffectSet = "liveEffectSet"; //Implement on Video Cue
        public const string FullSurface = "fullSurface"; //Implement on Video Cue
        public const string HoldLastFrame = "holdLastFrame"; //Implement on Video Cue
        public const string Layer = "layer"; //Implement on Video Cue
        public const string Opacity = "opacity"; //Implement on Video Cue
        public const string OriginX = "originX"; //Implement on Video Cue
        public const string OriginY = "originY"; //Implement on Video Cue
        public const string Origin = "origin";  //Implement on Video Cue
        public const string PreserveAspectRatio = "preserveAspectRatio"; //Implement on Video Cue
        public const string Quaternion = "quaternion"; //Implement on Video Cue
        public const string RotateZ = "rotateZ"; //Implement on Video Cue
        public const string RotateY = "rotateY"; //Implement on Video Cue
        public const string RotateX = "rotateX"; //Implement on Video Cue
        public const string LiveRotation = "liveRotation"; //Implement on Video Cue
        public const string XAxis = "Xaxis"; //Implement on Video Cue
        public const string YAxis = "Yaxis"; //Implement on Video Cue
        public const string ZAxis = "Zaxis"; //Implement on Video Cue
        public const string X = "x"; //Implement on Video Cue
        public const string Y = "y"; //Implement on Video Cue
        public const string Z = "z"; //Implement on Video Cue
        public const string ScaleX = "scaleX"; //Implement on Video Cue
        public const string LiveScaleX = "liveScaleX"; //Implement on Video Cue
        public const string ScaleY = "scaleY"; //Implement on Video Cue
        public const string LiveScaleY = "liveScaleY"; //Implement on Video Cue
        public const string Scale = "scale"; //Implement on Video Cue
        public const string LiveScale = "liveScale"; //Implement on Video Cue
        public const string SurfaceID = "surfaceID"; //Implement on Video Cue
        public const string SurfaceList = "surfaceList"; //Implement on Video Cue
        public const string SurfaceName = "surfaceName"; //Implement on Video Cue
        public const string SurfaceSize = "surfaceSize"; //Implement on Video Cue
        public const string TranslationX = "translationX";
        public const string LiveTranslationX = "liveTranslationX"; //Implement on Video Cue
        public const string TranslationY = "translationY";
        public const string LiveTranslationY = "liveTranslationY"; //Implement on Video Cue
        public const string Translation = "translation"; //Implement on Video Cue
        public const string LiveTranslation = "liveTranslation"; //Implement on Video Cue

        //Camera Cue Keys (Extension of Video Cue
        public const string CameraPatch = "cameraPatch";

        //Text Cue Keys
        public const string FixedWidth = "fixedWidth"; //Implement on Text Cue
        public const string Text = "text"; //Implement on Text Cue
        public const string LiveText = "liveText"; //Implement on Text Cue
        public const string Format = "format"; //Implement on Text Cue
        public const string Alignment = "alignment"; //Implement on Text Cue
        public const string FontFamily = "fontFamily"; //Implement on Text Cue
        public const string FontStyle = "fontStyle"; //Implement on Text Cue
        public const string FontFamilyAndStyle = "fontFamilyAndStyle"; //Implement on Text Cue
        public const string FontName = "fontName"; //Implement on Text Cue
        public const string FontSize = "fontSize"; //Implement on Text Cue
        public const string LineSpacing = "lineSpacing"; //Implement on Text Cue
        public const string Color = "color"; //Implement on Text Cue
        public const string BackgroundColor = "backgroundColor"; //Implement on Text Cue
        public const string StrikethroughColor = "strikethroughColor"; //Implement on Text Cue
        public const string UnderlineColor = "underlineColor"; //Implement on Text Cue
        public const string StrikethroughStyle = "strikethroughStyle"; //Implement on Text Cue
        public const string UnderlineStyle = "underlineStyle"; //Implement on Text Cue
        public const string OutputSize = "outputSize"; //Implement on Text Cue

        //Light Cue Keys
        //Fade Cue Keys
        //Network Cue Keys
        //MIDI Cue Keys

        //Midi File Cue Keys
        public const string Patch = "patch"; //Implement on Audio Cue and Video Cue
        public const string Rate = "rate"; //Implement on Audio Cue and Video Cue

        //Devamp Cue Keys
        public const string startNextCueWhenSliceEnds = "startNextCueWhenSliceEnds";
        public const string stopTargetWhenSliceEnds = "stopTargetWhenSliceEnds";

        //Script Cue Keys
        public const string ScriptSource = "scriptSource";


        // v3
        public const string Duration = "duration";
        public const string FullScreen = "fullScreen"; //Implement on Video Cue

        

        
        
    }

    public static class QIdentifiers
    {
        // Identifiers for "fake" cues
        public const string RootCue = "__root__";
        public const string ActiveCues = "__active__";
        public const string NoSelection = "none";
        public const string RootCueUpdate = "[root group of cue lists]";
    }

    public static class QConnectionStatus {
        public const string BadPass = "badpass";
        public const string Ok = "ok";
        public const string Error = "error";
    }

    public static class QColorName
    {
        public const string Red = "red";
        public const string Orange = "orange";
        public const string Yellow = "yellow";
        public const string Green = "green";
        public const string Blue = "blue";
        public const string Indigo = "indigo";
        public const string Purple = "purple";
        public const string None = "none";
        public const string Default = "default"; //is this even an option?
    }

    enum QTriggerAction
    {
        Nothing = 0,
        Panics = 1,
        Stops = 2,
        Hard_Stops = 3,
        Hard_Stops_And_Restarts = 4
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

    enum QMSCCommand
    {
        GO = 1,
        STOP = 2,
        RESUME = 3,
        TIMED_GO = 4,
        LOAD = 5,
        SET = 6,
        FIRE = 7,
        ALL_OFF =8,
        RESTORE = 9,
        RESET = 10,
        GO_OFF = 11,
        GO_OR_JAM_CLOCK = 16,
        STANDBY_PLUSS = 17,
        STANDBY_MINUS = 18,
        SEQUENCE_PLUS = 19,
        SEQUENCE_MINUS = 20,
        START_CLOCK = 21,
        STOP_CLOCK = 22,
        ZERO_CLOCK = 23,
        SET_CLOCK = 24,
        MTC_CHASE_ON = 25,
        MTC_CHASE_OFF = 26,
        OPEN_CUE_LIST = 27,
        CLOSE_CUE_LIST = 28,
        OPEN_CUE_PATH = 29,
        CLOSE_CUE_PATH = 30,
    }

    enum QMSCCommandFormat
    {
        All_Types = 127,
        Lighting_General = 1,
        Moving_Lights = 2,
        Color_Changers = 3,
        Strobes = 4,
        Lasers = 5,
        Chasers = 6,
        Sound_General = 16,
        Music = 17,
        CD_Players = 18,
        EPROM_Playback = 19,
        Audio_Tape_Machines = 20,
        Intercoms = 21,
        Amplifiers = 22,
        Audio_Effects_Devices = 23,
        Equalizers = 24,
        Machinery_General = 32,
        Rigging = 33,
        Flys = 34,
        Lifts = 35,
        Turntables = 36,
        Trusses = 37,
        Robots = 38,
        Animation = 39,
        Floats = 40,
        Breakaways = 41,
        Barges = 42,
        Video_General = 48,
        Video_Tape_Machines = 49,
        Video_Cassette_Machines = 50,
        Video_Disc_Players = 51,
        Video_Switchers = 52,
        Video_Effects = 53,
        Video_Character_Generators = 54,
        Video_Still_Stores = 55,
        Video_Monitors = 56,
        Projection_General = 64,
        Film_Projectors = 65,
        Slide_Projectors = 66,
        Video_Projectors = 67,
        Dissolvers = 68,
        Shutter_Controls = 69,
        Process_Control_General = 80,
        Hydraulic_Oil = 81,
        H2O = 82,
        CO2 = 83,
        Compressed_Air = 84,
        Natural_Gas = 85,
        Fog = 86,
        Smoke = 87,
        Cracked_Haze = 88,
        Pyrotechnics_General = 96,
        Fireworks = 97,
        Explosions = 98,
        Flame = 99,
        Smoke_Pots = 100
    }

    enum QNetworkMessageType
    {
        QLAB = 1,
        OSC = 2,
        UDP = 3
    }

    enum QNetworkQLabCommand
    {
        start = 1,
        stop = 2,
        hardStop = 3,
        pause = 4,
        resume = 5,
        togglePause = 6,
        load = 7,
        preview = 8,
        reset = 9,
        panic = 10,
        number = 11,
        name = 12,
        notes = 13,
        cueTargetNumber = 14,
        preWait = 15,
        duration = 16,
        postWait = 17,
        continueMode = 18,
        flagged = 19,
        armed = 20,
        colorName = 21
    }

    enum QRotationType
    {
        THREE_D = 0,
        X = 1,
        Y = 2,
        Z = 3
    }

    enum QVideoEffect
    {
        Color_Controls = 1,
        Exposure = 2,
        Gamma = 3,
        Sepia_Monochrome = 4,
        Min_Max_Invert = 5,
        White_point = 6,
        Box_Disc_Gaussian_Blurs = 7,
        Motion_Blur = 8,
        Sharpen_Luminance = 9,
        Unsharp_Mask = 10,
        Zoom_Blur = 11,
        Pixellation = 12,
        Screen = 13,
        Bloom_and_Gloom = 14,
        CMYK_Halftone = 15,
        Color_Posterize = 16,
        Crystallize_and_Pointillize = 17,
        Edge_Work = 18,
        Kaleidoscope = 1,
        Median_and_Comic_Effect = 19,
        Noise_Reduction = 20,
        Circle_Splash_Hole_Distortion = 21,
        Pinch_Bump_Distortion = 22,
        Torus_Lens_Distortion = 23,
        Twirl_Circular_Wrap_Vortex = 24,
        Glass_Lozenge = 25,
        Op_Tile = 26,
        Perspective_Tile = 27,
        Quad_Tiles = 28,
        Reflected_Tiles = 29,
        Rotated_Tiles = 30,
        Titles = 31
    }
}
