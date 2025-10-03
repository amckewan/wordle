( solver )

variable endgame     \ true to use endgame strategy

use simple-guess
endgame on
hidden on

: make-guess ( -- w )
    #hidden ( #working? ) remaining 1 = if ( only one left ) simple-guess exit then
    greens len = if ( we know it ) answer exit then
    endgame @ if endgame? if endgame-guess exit then then
    guesser execute ;

: round ( -- f )  make-guess guess solved ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
variable fails
: .failed fails @ if cr .secret ." failed" .history then ;
: solve? ( -- f )
    init-solver
    begin round not while
        failed if .failed false exit then
        prune
    repeat true ;

: solve  solve? .history if ." Solved " else ." Failed " then ;

\ shorthand
: init init-solver ;
: r round drop .history ;
: p prune #hidden remaining . ;
: try new w secret wmove solve ;

\ exhaustive test
create results  #guesses 1+ cells allot
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
    cr ." Using entropy-guess "   ['] entropy-guess solve-with
;
