import pastebin
import json

settings = {}

# Read update.json
with open('update.json', 'r') as f:
	settings = json.loads(f.read())

client = pastebin.Pastebin(settings['dev_key'])
client.user_key = settings['user_key']

print('Dev key is: {dev}\nUser key is: {user}'.format(dev = client.dev_key, user = client.user_key))
#print(client.edit_paste('9vFNJAT5', 'Test'))