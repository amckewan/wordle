( solver )

variable endgame    \ true to use endgame strategy
variable hidden     \ true to reduce working set to just hidden words

\ start easy...
use simple-guess
endgame on
hidden on

: init-solver  init-game  all-words  hidden @ if snip-hidden then ;

: make-guess ( -- w )
    remaining-hidden 1 = if ( only one left ) simple-guess exit then
    greens len = if ( we know it ) answer exit then
    endgame @ if endgame? if endgame-guess exit then then
    guesser execute ;

\ Prune the working set, removing words that wouldn't produce this score
: prune ( -- )  latest 2>r
    working begin dup @ dup while
        dup 2r@ rot cell+ prune? if  @ over !  else  nip  then
    repeat 2drop 2r> 2drop ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
variable fails
: .failed fails @ if cr .secret ." failed" .history then ;
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
: r round drop .history ;
: p prune remaining . ;
: try new w secret wmove solve ;
