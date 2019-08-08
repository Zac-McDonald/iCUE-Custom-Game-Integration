# iCUE Unofficial Game Integration
## By Zac McDonald

This project provides the source for custom programs used to utilise the game-integration capabilities of Corsair's iCUE Software. This integration has been used in commercial AAA-games such as Metro Exodus and Far Cry 5. The project aims to provide easy, cross-language and versatile access to this alongside some extensions.

### Download

See [the Releases page](https://github.com/Zac-McDonald/iCUE-Custom-Game-Integration/releases) for downloading the pre-built programs.

**NOTE:** Run with, "iCUE HTTP Server.exe" with administrator privileges. This is needed due to permission requirements of hosting a .NET HTTP Server (used by controllers to utilise game-integration) and a Pipe Server (used to interact between "iCUE HTTP Server.exe" and "iCUE CgSDK Handler.exe").

### Build

If building yourself, ensure all the relevant files are built and moved to the same directory.
Required files are as follows:
* Newtonsoft.Json.dll (located in ./iCUE HTTP Server/bin/Debug)
* CgSDK_Interop.dll (located in ./iCUE HTTP Server/bin/Debug)
* CgSDK.x64_2015.dll (located in ./iCUE HTTP Server/bin/Debug or your Far Cry 5/Metro Exodus install)
* iCUE HTTP Server.exe (built file)
* iCUE CgSDK Handler.exe (built file)

The debug versions will run fine in Visual Studio, the release versions will not, these will need to be moved into the same directory.

### Setup

When the server is first run, it will create a file, "settings.json" which contains an example process setup (showing all available settings) and a default setting for turning off game-integration while iCUE is focussed.

For game-integration to be setup, 3 things are required.
1. The required process must be in "settings.json". For this, use the name of the running executable.
2. A controller must be programmed to interact with the HTTP Server to perform the logic for the integration (see the [documentation](https://zac-mcdonald.github.io/iCUE-Custom-Game-Integration/) for more information). Using the "AutoSetGame" settings is the only instance where a controller is not required.
3. The appropriate game folder, exported iCUE profiles and "priorities.cfg" must be present in the "GameSdkEffects" folder (C:\Program Files (x86)\Corsair\CORSAIR iCUE Software\GameSdkEffects).

The "settings.json" file defines how and when the controllers will be launched for each process as well as if the server should automatically call "SetGame" on process focus/unfocus. It should be noted that **calling "SetGame" with the game name "iCUE" will revert to default iCUE lighting**. Controller examples using Python and the [requests library](https://pypi.org/project/requests/) can be found in the "Example Controllers" folder, do note that the hotkey example (deceitHotkey.py) requires additional external libraries to capture input while unfocussed. Example controllers also contains profileCycle.py (for looping through all profiles associated with a game, use it to preview the FarCry5 and Metro lighting profiles) and profileTester.py (for testing individual lighting profiles).

Regarding "GameSdkEffects", this is the folder where are your lighting profiles should be saved (after exporting from iCUE). The subfolder names are the "game names" used by the SDK, with each saved profile in the folders as the potential events/states. A profile can be regarded as a state or event (or either), however events should be made to be timed in iCUE. A state is a profile that will play when "SetState" is called and terminate when "ClearState" is called. An event will run when "SetEvent" is called and will terminate once the effect finishes (an example is FC_AnimalAttack from FarCry5). The priorities.cfg file is most critical of all, it contains a list of all the available profiles and their given priority [0-255]. iCUE will only ever display a single lighting profile, and this will be the highest priority active state or event, thus it is crucial priorities are setup correctly. The highest priority, active profile will be played. For examples, see the Far Cry 5/Metro Exodus priorities.cfg files.

### Support

Consult the documentation for information on controllers and process settings: https://zac-mcdonald.github.io/iCUE-Custom-Game-Integration/
The documentation also contains JavaScript for testing each function from the browser (requires the HTTP Server to be running).

If you have any questions about usage of the software, use [this thread](https://forum.corsair.com/v3/showthread.php?t=189370) on the Corsair forums.

### Behind the Scenes

This software works by utilising an alternate Corsair SDK called the CgSDK. It's dll file can be found in any integrated game installs (i.e. Far Cry 5 or Metro Exodus). It's a C++ SDK with some common functions to the [publicly available SDK](https://forum.corsair.com/v3/forumdisplay.php?f=300) but with game-integrations specific functions instead. I've implemented most functions except for "CgSDKSetStateWithKey", "CgSDKClearStateWithKey", "CgSDKSetEventWithKey", "CgSDKHideProgressBar", "CgSDKShowProgressBar", "CgSDKSetProgressBarValue". More information on these functions would be greatly appreciated, as the others were implemented with pure guesswork to their input/output. To utilise the SDK with C#, I created a rather primitive C# interoperability dll using C++. More information on this can be located in [an earlier forum post](https://forum.corsair.com/v3/showthread.php?t=181358&page=2). My final note is on the separation of the HTTP Server and the CgSDK Handler programs. This was necessary as calling "SetGame" multiple times will crash the SDK, and thus I exclusively use the Handler to interact with the SDK and restart it whenever "SetGame" is called.