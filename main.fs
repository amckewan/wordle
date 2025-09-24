( Wordle game & solver - A.McKewan 2025 )

require preamble.fs

( game )
include words.fs
include wordlists.fs
include score.fs
include game.fs
include history.fs
include game-ui.fs

( solver )
include solver.fs
\  include prune.fs
\  include vowels.fs
\  include unique.fs
\  include guess.fs
\  include tally.fs
\  include endgame.fs
\  include solve.fs
