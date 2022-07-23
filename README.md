# QControlKit
This is an unofficial .NET library port of Figure53's [QLabKit](https://github.com/Figure53/QLabKit.objc).


## Components
- **QBrowser**: Searches for an maintains a list of QLab "instances" using bonjour
- **QClient**: Wrapper over underlying TCP/UDP connection as well as providing event handlers for common QLab replies (i.e. cue needs updated, workspace need updated, etc.)
- **QCue**: The most basic object in QLab, contains shared properties and methods for a cue in a cuelist (cuelists are also a cue) is extended for other cue types (Text, Midi, etc.)
- **QMessage**: Wrapper over underlying OSC Message, has methods to quickly determine the message type and parts (Workspace info, address, arguments, etc.)
- **QServer**: This class represents a computer that is running QLab, it has host information (IP, Port) as well as discovered workspaces on that computer.
- **QWorkspace**: This class represents a workspace on a QServer, it contains methods and properties to interact with a workspace as well as access the underlying QCues and QClient that is facilitating communication


## Todo: give a basic example of the workflow


### Disclaimer
Figure 53® and QLab® are registered trademarks of Figure 53, LLC. QControlKit is not affiliated with with Figure 53, LLC and this application has not been reviewed nor is it approved by Figure 53, LLC
