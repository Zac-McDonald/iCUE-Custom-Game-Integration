require_relative './iCUE'

$game_name = 'Other'

ICUE.clear_all_states($game_name)
ICUE.set_state($game_name, ARGV[0])
ICUE.set_game($game_name)