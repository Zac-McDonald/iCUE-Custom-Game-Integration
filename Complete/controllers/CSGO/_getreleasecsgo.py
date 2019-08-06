from shutil import copyfile
import subprocess

WinRarDir = 'C:/Program Files/WinRAR'

CSGO = 'E:/Users/Owner/Documents/Visual Studio 2015/Projects/CSGO_Game_Integration/CSGO_Game_Integration/bin/Release'

#copyfile('../test.txt', './test.txt')
print('\nCopying CSGO Files')

copyfile(CSGO + '/CSGO_Game_Integration.exe', './CSGO_Game_Integration.exe')
copyfile(CSGO + '/CSGSI.dll', './CSGSI.dll')
copyfile(CSGO + '/Newtonsoft.Json.dll', './Newtonsoft.Json.dll')

print('\nCompressing CSGO Controller Files')
args = ' f "CSGO" @files.lst'
subprocess.call(WinRarDir + '/RAR.exe' + args)

input ('\nPress ENTER to close...')