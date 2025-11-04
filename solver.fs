( solver )

variable hidden     \ true if the solver can use the hidden word list
variable fails      \ true to show failures

\ config to solve all puzzles
use entropy-guess
hidden off
4 endgame !
100 fence !
allon2 off
fails off
timing on

: init-solver  init-game  all-words  hidden @ if prune-hidden then ;

: make-guess ( -- w )
    remaining 1 = if ( only one left ) simple-guess exit then
    #greens len = if ( we know it ) greens exit then
    endgame? if ( endgame guess ) exit then
    guesser execute ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
: .failed fails @ if cr secret w. ." failed " fails @ 0> if .history cr then then ;
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
: try init w to secret ;
: r round drop .history ;
: p prune remaining . ;
: rounds ( n -- )  0 do round if leave then prune loop ;

\ display solver state
: .guesser  guesser case
    ['] simple-guess  of ." simple-guess "  endof
    ['] random-guess  of ." random-guess "  endof
    ['] tally-guess   of ." tally-guess "   endof
    ['] entropy-guess of ." entropy-guess " endof
    dup . endcase ;
: .solver
    cr ." Guesser:  " .guesser
    cr ." Hidden:   " hidden ?
    cr ." Endgame:  " endgame ?
    cr ." Fence:    " fence ?
    cr ." Allon2:   " allon2 ?
    \ these don't affect the results, just output
    \  cr ." Fails:    " fails ?
    \  cr ." Timing:   " timing ?
;
