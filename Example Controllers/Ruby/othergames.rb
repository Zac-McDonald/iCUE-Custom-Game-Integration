require './iCUE.rb'

game_name = 'Other'

clear_all_states(game_name)
set_state(game_name, ARGV[0])
set_game(game_name)