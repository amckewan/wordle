( Wordle game & solver - A.McKewan 2025 )

require preamble.fs

( game )
include words.fs
include wordlists.fs
include game.fs
include score.fs
include history.fs
include game-ui.fs

( solver )
\  include prune.fs
\  include vowels.fs
\  include unique.fs
\  include guess.fs
\  include tally.fs
\  include endgame.fs
\  include solve.fs
