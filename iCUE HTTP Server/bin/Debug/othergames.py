import iCUE
import sys

gameName = 'FarCry5'

iCUE.ClearAllStates(gameName)
iCUE.SetState(gameName, sys.argv[1])
iCUE.SetGame(gameName)