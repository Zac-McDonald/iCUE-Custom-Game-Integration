import iCUE
import re

game = input('Game Name: ')
profiles = iCUE.ListProfiles(game)

iCUE.SetGame(game)

i = 0
while True:
	state = profiles[i]
	state = re.sub('\.cueprofile$', '', state)

	print('State Name: {}'.format(state))
	iCUE.SetState(game, state)
	input('Enter to continue...')
	iCUE.ClearState(game, state)

	i = (i + 1) % len(profiles)