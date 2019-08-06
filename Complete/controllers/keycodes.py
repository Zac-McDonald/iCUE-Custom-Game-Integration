from pyHook import HookManager
from win32gui import PumpMessages, PostQuitMessage

class Keystroke_Watcher(object):
    def __init__(self):
        self.hm = HookManager()
        self.hm.KeyDown = self.on_keyboard_event
        self.hm.HookKeyboard()


    def on_keyboard_event(self, event):
        try:
            print(event.KeyID)
        finally:
            return True

    def shutdown(self):
        PostQuitMessage(0)
        self.hm.UnhookKeyboard()

watcher = Keystroke_Watcher()
PumpMessages()