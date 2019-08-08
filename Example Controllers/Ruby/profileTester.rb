require './iCUE.rb'

print "Game Name: "
game_name = gets.chomp
set_game(game_name)

while (true)
	print "State Name: "
	state_name = gets.chomp
	set_state(game_name, state_name)
	puts "Enter to clear..."
	gets
	clear_state(game_name, state_name)
end