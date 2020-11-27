require 'json'
require 'net/http'

class ICUE
	@@url = 'http://127.0.0.1:25555/icue'
	@@profile_dir = 'C:/ProgramData/Corsair/CUE/GameSdkEffects/'

	def self.send_get_request (params)
		uri = URI(@@url)
		uri.query = URI.encode_www_form(params)
		response = Net::HTTP.get_response(uri)
		return response
	end

	def self.list_profiles (game_name)
		game_dir = @@profile_dir + game_name + '/'
		profiles = Dir[game_dir + '*.cueprofile'].map { |f| f.gsub(game_dir, '') }
		return profiles
	end

	def self.get_game (game_name = '')
		info = { :func => 'getgame' }
		if (game_name != '')
			info[:game] = game_name
		end
		response = send_get_request(info)
		return response.body
	end

	def self.get_all_games ()
		info = { :func => 'getallgames' }
		response = send_get_request(info)
		return response.body
	end

	def self.set_game (game_name)
		info = { :func => 'setgame', :game => game_name }
		response = send_get_request(info)
	end

	def self.reset_game (game_name)
		info = { :func => 'reset', :game => game_name }
		response = send_get_request(info)
	end

	def self.lock_game (game_name)
		info = { :func => 'lock', :game => game_name }
		response = send_get_request(info)
	end

	def self.unlock_game (game_name)
		info = { :func => 'unlock', :game => game_name }
		response = send_get_request(info)
	end

	def self.set_state (game_name, state_name)
		info = { :func => 'setstate', :game => game_name, :state => state_name }
		response = send_get_request(info)
	end

	def self.clear_state (game_name, state_name)
		info = { :func => 'clearstate', :game => game_name, :state => state_name }
		response = send_get_request(info)
	end

	def self.clear_all_states (game_name)
		info = { :func => 'clearallstates', :game => game_name }
		response = send_get_request(info)
	end

	def self.set_event (game_name, event_name)
		info = { :func => 'setevent', :game => game_name, :event => event_name }
		response = send_get_request(info)
	end

	def self.clear_all_events (game_name)
		info = { :func => 'clearallevents', :game => game_name }
		response = send_get_request(info)
	end
end
