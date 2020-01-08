require_relative './iCUE'

print "Game Name: "
$game_name = gets.chomp
ICUE.set_game($game_name)

while (true)
	print "State Name: "
	state_name = gets.chomp
	ICUE.set_state($game_name, state_name)
	puts "Enter to clear..."
	gets
	ICUE.clear_state($game_name, state_name)
end