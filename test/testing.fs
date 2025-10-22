( testing )

use entropy-guess
hidden off
endgame off
fence off
allon2 off

fails on
timing on

.solver

\ It takes 2.5 minutes to solve all puzzles with the entropy guesser.
\ To reduce the experiment time, find which words require 5 or more guesses.
\ Now it takes about 30 seconds to go through those ~400 words.
include entropy-guesses.fs

: tsolver ( n -- ) \ only consider words that took n or more guesses
    results #guesses 1+ cells erase
    #hidden 0 do
        i entropy-guesses + c@ over >=
        if  init  i to secret  solve? +results  then
    loop .results drop ;

\ vary endgame and fence
: tryit ( endgame fence -- ) fence ! endgame ! .solver cr solver ;
: tryem
    0 0 tryit      0 30 tryit      0 50 tryit
    3 0 tryit      3 30 tryit      3 50 tryit
    4 0 tryit      4 30 tryit      4 50 tryit ;
