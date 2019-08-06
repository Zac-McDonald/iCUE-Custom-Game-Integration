import requests
import json
import re

login_url = 'https://pastebin.com/api/api_login.php'
raw_url = 'https://pastebin.com/raw/{paste}'
edit_url = 'https://pastebin.com/edit/{paste}'
post_url = 'https://pastebin.com/api/api_post.php'

user_prelogin_url = 'https://pastebin.com/login'
user_login_url = 'https://pastebin.com/login.php'

class Pastebin:
	def __init__(self, dev_key):
		self.dev_key = dev_key
		self.user_key = ''

	def set_user_key (self, user_key):
		self.user_key = user_key

	def generate_user_key (self, username, password):
		with requests.sessions.Session() as s:
			response = s.post(login_url, data = {'api_dev_key': self.dev_key, 'api_user_name': username, 'api_user_password': password})
			if not response.text.startswith('Bad'):
				# Successfully generated a user_key
				self.user_key = response.text
				return True
			else:
				# Failed to generate a user_key
				return False

	def create_paste (self, contents, user = False, name = '', syntax = '', privacy = 0, expire = 'N'):
		with requests.sessions.Session() as s:
			paste_info = {'api_dev_key': self.dev_key, 'api_option': 'paste', 'api_paste_code': contents, 'api_paste_private': privacy, 'api_paste_expire_date': expire}

			if user:
				paste_info['api_user_key'] = self.user_key
			if name != '':
				paste_info['api_paste_name'] = name
			if syntax != '':
				paste_info['api_paste_format'] = syntax

			response = s.post(post_url, data = paste_info)
			return response.text

	def get_paste (self, paste):
		with requests.sessions.Session() as s:		
			return s.get(raw_url.format(paste = paste)).text

	def edit_paste (self, paste, contents):
		with requests.sessions.Session() as s:
			edit_page = s.get(edit_url.format(paste = paste)).text
			csrf_token = re.findall(r'name=\"csrf_token_9vFNJAT5\" value=\"(.+)\"', edit_page)
			paste_info = {'submit_hidden': 'submit_hidden', 'item_key': paste, 'csrf_token_{}'.format(paste): csrf_token, 'post_key': paste, 'paste_code': contents}
			print(edit_page)
			print(paste_info)

	def delete_paste (self):
		pass

	def list_user_pastes (self, max_pastes = 50):
		with requests.sessions.Session() as s:
			response = s.post(post_url, data = {'api_dev_key': self.dev_key, 'api_user_key': self.user_key, 'api_option': 'list', 'api_results_limit': max_pastes})
			return response.text