( testing )


\ It takes 2.5 minutes to solve all puzzles with the entropy guesser.
\ To reduce the experiment time, find which words require 5 or more guesses.
\ Now it takes about 30 seconds to go through those ~400 words.
include entropy-guesses.fs

: solver ( ignore words that are solved in 5 or fewer guesses )
    results #guesses 1+ cells erase
    #hidden 0 do
        i entropy-guesses + c@ 5 >
        if  init  i ww secret!  solve? +results  then
    loop .results ;

: failures ( just failing words )
    results #guesses 1+ cells erase
    #hidden 0 do
        i entropy-guesses + c@ 6 >
        if  init  i ww secret!  solve? +results  then
    loop .results ;

0 [if]
Interestingly, more are solved in 5 without endgame...
endgame off solver 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    0 Solved in 4 
  345 Solved in 5 
   75 Solved in 6 
   17 Failed

endgame on solver 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    8 Solved in 4 
  289 Solved in 5 
  127 Solved in 6 
   13 Failed 

If we only look at the words that took 6+ guesses:
endgame on solver 
    0 Solved in 1 
    0 Solved in 2 
    0 Solved in 3 
    2 Solved in 4 
   18 Solved in 5 
   60 Solved in 6 
   12 Failed

Here are the 12 failing words and how many greens we had when
boxer   3@3
cover   3@3
foyer   3@3
hound   3@4
hover   3@3
joker   3@3
jolly   4@4
maker   3@3 4@4
mound   3@4
nudge   3@3 4@4
rover   3@3
swell   3@4

31 words match -o-er:
    boxer cover cower foyer goner homer hover joker mover mower
    roger rover rower wooer boner bower comer cooer coyer fouer
    gofer gomer honer koker moner roker vomer vower woker yoker
    zoner

1st letters:    b c f g h j k m r v w y z
3rd letters:    f h k m n o u v w x y z

[then]
