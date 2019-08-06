from shutil import copyfile
import subprocess

WinRarDir = 'C:/Program Files/WinRAR'

#copyfile('../test.txt', './test.txt')
print('\nCopying Game Integration Files')
copyfile('../iCUE HTTP Server/bin/Release/iCUE HTTP Server.exe', './iCUE HTTP Server.exe')
copyfile('../iCUE CgSDK Handler/bin/Release/iCUE CgSDK Handler.exe', './iCUE CgSDK Handler.exe')
copyfile('../iCUE HTTP Server/bin/Release/CgSDK.x64_2015.dll', './CgSDK.x64_2015.dll')
copyfile('../iCUE HTTP Server/bin/Release/CgSDK_Interop.dll', './CgSDK_Interop.dll')
copyfile('../iCUE HTTP Server/bin/Release/Newtonsoft.Json.dll', './Newtonsoft.Json.dll')

print('\nCompressing Game Integration Files')
args = ' u "iCUE Custom Game Integration" @files.lst'
subprocess.call(WinRarDir + '/RAR.exe' + args)

print('\nCompressing Controller Files')
args = ' u "iCUE Custom Game Controllers" @controllerfiles.lst'
subprocess.call(WinRarDir + '/RAR.exe' + args)

input ('\nPress ENTER to close...')