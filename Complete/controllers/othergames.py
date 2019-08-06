import iCUE
import sys

gameName = 'Other'

iCUE.ClearAllStates(gameName)
iCUE.SetState(gameName, sys.argv[1])
iCUE.SetGame(gameName)