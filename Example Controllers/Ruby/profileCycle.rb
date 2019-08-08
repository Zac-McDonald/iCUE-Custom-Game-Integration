require './iCUE.rb'

print "Game Name: "
game_name = gets.chomp
profiles = list_profiles(game_name)

set_game(game_name)

i = 0
while (true)
	state_name = profiles[i].gsub('.cueprofile', '')

	puts "State Name: #{state_name}"
	set_state(game_name, state_name)
	puts "Enter to continue..."
	gets
	clear_state(game_name, state_name)

	i = (i + 1) % profiles.length
end