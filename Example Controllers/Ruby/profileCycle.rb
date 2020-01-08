require_relative './iCUE'

print "Game Name: "
$game_name = gets.chomp
profiles = ICUE.list_profiles($game_name)

ICUE.set_game($game_name)

i = 0
while (true)
	state_name = profiles[i].gsub('.cueprofile', '')

	puts "State Name: #{state_name}"
	ICUE.set_state($game_name, state_name)
	puts "Enter to continue..."
	gets
	ICUE.clear_state($game_name, state_name)

	i = (i + 1) % profiles.length
end