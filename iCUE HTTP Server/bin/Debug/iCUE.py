import requests
from os import listdir
from os.path import isfile, join, splitext

url = 'http://127.0.0.1:25555/icue'
profileDir = 'C:/Program Files (x86)/Corsair/CORSAIR iCUE Software/GameSdkEffects/'

def ListProfiles (gameName):
	gameDir = profileDir + gameName
	profiles = [f for f in listdir(gameDir) if (isfile(join(gameDir, f)) and splitext(f)[1] == '.cueprofile')]
	return profiles

def GetGame (gameName = ''):
	info = { 'func': 'getgame'}
	if (gameName != ''):
		info['game'] = gameName
	response = requests.get(url, params = info)
	return response.content

def GetAllGames ():
	info = { 'func': 'getallgames' }
	response = requests.get(url, params = info)
	return response.content

def SetGame (gameName):
	info = { 'func': 'setgame', 'game': gameName }
	response = requests.get(url, params = info)

def ResetGame (gameName):
	info = { 'func': 'reset', 'game': gameName }
	response = requests.get(url, params = info)

def SetState (gameName, stateName):
	info = { 'func': 'setstate', 'game': gameName, 'state': stateName }
	response = requests.get(url, params = info)

def ClearState (gameName, stateName):
	info = { 'func': 'clearstate', 'game': gameName, 'state': stateName }
	response = requests.get(url, params = info)

def ClearAllStates (gameName):
	info = { 'func': 'clearallstates', 'game': gameName }
	response = requests.get(url, params = info)

def SetEvent (gameName, eventName):
	info = { 'func': 'setevent', 'game': gameName, 'event': eventName }
	response = requests.get(url, params = info)

def ClearAllEvents (gameName):
	info = { 'func': 'clearallevents', 'game': gameName }
	response = requests.get(url, params = info)