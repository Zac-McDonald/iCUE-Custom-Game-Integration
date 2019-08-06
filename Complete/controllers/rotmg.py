import iCUE
import requests
import time
import json

url = 'https://nightfirec.at/realmeye-api/'
playerName = 'TomIsSoGay'

prefix = ''
classes = { 'Rogue', 'Archer', 'Wizard', 'Priest', 'Warrior', 'Knight', 'Paladin', 'Assassin', 'Necromancer', 'Huntress', 'Mystic', 'Trickster', 'Sorcerer', 'Ninja', 'Samurai' }

iCUE.SetGame('RotMG')

while (True):
	info = { 'player': playerName }
	response = requests.get(url, params = info)

	data = json.loads(response.text)

	j = 0
	for i in range(0, len(data['characters'])):
		if (data['characters'][i]['last_seen'] > data['characters'][j]['last_seen']):
			j = i

	lastPlayedClass = data['characters'][j]['class']

	iCUE.ClearAllStates('RotMG')
	iCUE.SetState('RotMG', prefix + lastPlayedClass)

	time.sleep(15)