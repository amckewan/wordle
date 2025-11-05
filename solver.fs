( solver )

variable hidden     \ true if the solver can use the hidden word list
variable fails      \ true to show failures

\ config to solve all puzzles
use entropy-guess
hidden off
4 endgame !
100 fence !
fails off
timing on

: init-solver  init-game  all-words  hidden @ if prune-hidden then ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
: .failed fails @ if cr secret w. ." failed " fails @ 0> if .history cr then then ;
: round ( -- f )  guess submit solved ;
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
;
