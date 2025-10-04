( try different algorithms )

\ exhaustive test
create results  #guesses 1+ cells allot  ( index 0 for failures )
: >result  cells results + ;
: average ( -- n*100 )
    0  #guesses 1+ 1 do  i >result @ i * +  loop  100 #hidden */ ;
: .## ( n -- )  0 <# # # '.' hold #s #> type space ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed "
    cr ." Average: " average .## ;

variable talking ( show progress as we go )
: +results ( solved? -- )
    talking @ if dup if guesses . else ." X " then then
    if guesses >result else results then 1 swap +! ;
: solve-all ( -- )
    results #guesses 1+ cells erase
    #hidden 0 do  i ww new-game-with  solve? +results  loop ;
: solver ( -- )
    timestamp  solve-all  timestamp  .results
    timing @ if swap - 3 spaces .elapsed else 2drop then ;

: solve-with ( xt -- )  to guesser solver cr ;
: try-all
    cr ." Using simple-guess "    ['] simple-guess  solve-with
    cr ." Using random-guess "    ['] random-guess  solve-with
    cr ." Using tally-guess "     ['] tally-guess   solve-with
    \  cr ." Using entropy-guess "   ['] entropy-guess solve-with
;
