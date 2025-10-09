( testing )

use entropy-guess
hidden off
endgame off
fails off
timing on

\ It takes 2.5 minutes to solve all puzzles with the entropy guesser.
\ To reduce the experiment time, find which words require 5 or more guesses.
\ Now it takes about 30 seconds to go through those ~400 words.
include entropy-guesses.fs

: tsolver ( ignore words that are solved in 5 or fewer guesses )
    results #guesses 1+ cells erase
    #hidden 0 do
        i entropy-guesses + c@ 5 >
        if  init  i ww secret!  solve? +results  then
    loop .results ;

: failing ( just failing words )
    results #guesses 1+ cells erase
    #hidden 0 do
        i entropy-guesses + c@ 6 >
        if  init  i ww secret!  solve? +results  then
    loop .results ;

0 [if]
=== baseline, entropy guess, all words
endgame off solver 
    0 Solved in 1 
   53 Solved in 2 
  732 Solved in 3 
 1093 Solved in 4 
  345 Solved in 5 
   75 Solved in 6 
   17 Failed 
Average: 3.82    155.110 sec

endgame on solver 
    0 Solved in 1 
   53 Solved in 2 
  691 Solved in 3 
 1085 Solved in 4 
  389 Solved in 5 
   88 Solved in 6 
    9 Failed 
Average: 3.88    148.794 sec

With endgame, fewer failures but higher average.

=== just looking at words that took > 5 guesses
endgame off solver 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    0 Solved in 4 
    0 Solved in 5 
   75 Solved in 6 
   17 Failed 
Average: 0.19

endgame on solver 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    4 Solved in 4 
   32 Solved in 5 
   51 Solved in 6 
    5 Failed 
Average: 0.20  ok

=== just looking at words that failed
endgame on failing 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    0 Solved in 4 
    3 Solved in 5 
   11 Solved in 6 
    3 Failed 
Average: 0.03

[then]
