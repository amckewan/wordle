( solver )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each score by removing words that wouldn't get that score.

create working  #wordles allot  \ one byte per word, 0=absent, 1=present

: init-solver  init-game  working #wordles 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

working #wordles bounds 2constant for-working ( -- limit index )

: >ww ( i -- w )  working - ww ;

: #working ( -- n )  0 for-working do  i c@ +  loop ;

: .working  0 for-working do  i c@ if i >ww w. 1+ then  loop . ;


( ===== TESTS ===== )

TESTING #WORKING
T{ init-solver #working -> #wordles }T
T{ working 10 erase  #working -> #wordles 10 - }T
T{ 0 working #wordles + 1- c!  #working -> #wordles 11 - }T
T{ working #wordles erase  #working -> 0 }T
