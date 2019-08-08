import iCUE

game = input('Game Name: ')
iCUE.SetGame(game)

while True:
	state = input('State Name: ')
	iCUE.SetState(game, state)
	input('Enter to clear...')
	iCUE.ClearState(game, state)