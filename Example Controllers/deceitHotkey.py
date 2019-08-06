import iCUE
import time
from pyHook import HookManager
from win32gui import PumpMessages, PostQuitMessage

lobbyKey = 111			# '/'' on numpad
infectedKey = 106		# '*' on numpad
innocentKey = 109		# '-' on numpad

class Keystroke_Watcher(object):
    def __init__(self):
        self.hm = HookManager()
        self.hm.KeyDown = self.on_keyboard_event
        self.hm.HookKeyboard()


    def on_keyboard_event(self, event):
        try:
            OnKeyPress(event.KeyID)
        finally:
            return True

    def shutdown(self):
        PostQuitMessage(0)
        self.hm.UnhookKeyboard()

def OnKeyPress (KeyID):
	if KeyID == lobbyKey:
		print('Lobby')
		iCUE.ClearAllStates('Deceit')
		iCUE.SetState('Deceit', 'Lobby')
	elif KeyID == infectedKey:
		print('Infected')
		iCUE.ClearAllStates('Deceit')
		iCUE.SetState('Deceit', 'Infected')
	elif KeyID == innocentKey:
		print('Innocent')
		iCUE.ClearAllStates('Deceit')
		iCUE.SetState('Deceit', 'Innocent')

iCUE.SetGame('Deceit')
iCUE.SetState('Deceit', 'Lobby')

watcher = Keystroke_Watcher()
PumpMessages()