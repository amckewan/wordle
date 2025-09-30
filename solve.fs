( solver )

use tally-guesser ( best so far )

variable endgame ( turn it on and off )  endgame on

: solver-guess ( -- w )
    #working 1 = if ( only one left ) simple-guesser exit then
    greens len = if ( we know it ) answer exit then
    endgame @ if endgame? if endgame-guess exit then then
    make-guess ;

: round ( -- f )  solver-guess guess solved ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
: solve? ( -- f )
    init-solver
    begin round not while
        failed if false exit then
        prune
    repeat true ;

: solve  solve? .history if ." Solved " else ." Failed " then ;

\ shorthand
: init init-solver ;
: r round drop .history ;
: p prune #working . ;
: try new w secret wmove solve ;

\ exhaustive test
create results  #guesses 1+ cells allot
: >result  cells results + ;
: average ( -- n*100 )
    0  #guesses 1+ 1 do  i >result @ i * +  loop  100 #words */ ;
: .## ( n -- )  0 <# # # '.' hold #s #> type space ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed "
    cr ." Average: " average .## ;

: (solver) ( #words -- )
    results #guesses 1+ cells erase
    0 do
        i ww new-game-with
        solve? if guesses >result else results then 1 swap +!
    loop .results ;
: solver ( try all words ) #words (solver) ;

: solve-with ( xt -- )  to guesser solver cr ;
: try-all
    cr ." Using simple-guesser "    ['] simple-guesser  solve-with
    cr ." Using random-guesser "    ['] random-guesser  solve-with
    cr ." Using tally-guesser "     ['] tally-guesser   solve-with
;
