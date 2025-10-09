( solver )

variable endgame    \ true to use endgame strategy
variable fails      \ true to show failures

use tally-guess
hidden on
endgame off
fails off
timing off

: init-solver  init-game  all-words  hidden @ if snip-hidden then ;

: make-guess ( -- w )
    remaining 1 = if ( only one left ) simple-guess exit then
    #greens len = if ( we know it ) greens exit then
    endgame @ if endgame? if exit then then
    guesser execute ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
: .failed fails @ if cr .secret ." failed" fails @ 0> if .history cr then then ;
: round ( -- f )  make-guess guess solved ;
: solve? ( -- f )
    init-solver
    begin round not while
        failed if .failed false exit then
        prune
    repeat true ;

: solve  solve? .history if ." Solved " else ." Failed " then ;

\ shorthand
: init init-solver ;
: try init w secret! ;
: r round drop .history ;
: p prune remaining . ;
: rounds ( n -- )  0 do round if unloop exit then prune loop ;
