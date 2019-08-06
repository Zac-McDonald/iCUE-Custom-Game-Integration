import iCUE
from os import listdir
from os.path import isfile, join

game = input('Game Name: ')
iCUE.SetGame(game)

while True:
	state = input('State Name: ')
	iCUE.SetState(game, state)
	input('Enter to clear...')
	iCUE.ClearState(game, state)