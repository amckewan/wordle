( Wordle game & solver - A.McKewan 2025 )

warnings off ( gforth )

require preamble.fs

( basics )
include words.fs
include score.fs
include prune.fs

( game )
include history.fs
include game.fs
include game-ui.fs

( solver )
include working.fs
include guess.fs
include solver.fs
