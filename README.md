# iCUE Unofficial Game Integration
## By Zac McDonald

This project provides the source for custom programs used to utilise the game-integration capabilities of Corsair's iCUE Software. This integration has been used in commercial AAA-games such as Metro Exodus and FarCry5. The project aims to provide easy, cross-language and versatile access to this alongside some extensions.

### Download

See [the Releases page](https://github.com/Zac-McDonald/iCUE-Custom-Game-Integration/releases) for downloading the pre-built programs.

**NOTE:** Run with, "iCUE HTTP Server.exe" with administator privileges. This is needed due to permission requirements of hosting a .NET HTTP Server (used by controllers to utilise game-integration) and a Pipe Server (used to interact between "iCUE HTTP Server.exe" and "iCUE CgSDK Handler.exe").

### Build

If building yourself, ensure all the relevant files are built and moved to the same directory.
Required files are as follows:
* Newtonsoft.Json.dll (located in ./iCUE HTTP Server/bin/Debug)
* CgSDK_Interop.dll (located in ./iCUE HTTP Server/bin/Debug)
* CgSDK.x64_2015.dll (located in ./iCUE HTTP Server/bin/Debug or your FarCry5/Metro Exodus install)
* iCUE HTTP Server.exe (built file)
* iCUE CgSDK Handler.exe (built file)

The debug versions will run fine in Visual Studio, the release versions will not, these will need to be moved into the same directory.

### Setup

When the server is first run, it will create a file, "settings.json" which contains an example process setup (showing all available settings) and a default setting for turning off game-integration while iCUE is focussed.
For game-integration to be setup, 3 things are required.
1. The required process must be in "settings.json". For this, use the name of the running executable.
2. A controller must be programmed to interact with the HTTP Server to perform the logic for the integration (see the [documentation](https://zac-mcdonald.github.io/iCUE-Custom-Game-Integration/) for more information).
3. The appropriate game folder, exported iCUE profiles and "priorities.cfg" must be present in the "GameSdkEffects" folder (C:\Program Files (x86)\Corsair\CORSAIR iCUE Software\GameSdkEffects).



Consult the documentation for information on controllers and process settings: https://zac-mcdonald.github.io/iCUE-Custom-Game-Integration/