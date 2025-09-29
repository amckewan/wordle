( solver )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each score by removing words that wouldn't get that score.

create working  #words allot  \ one byte per word, 0=absent, 1=present

: init-solver  init-game  working #words 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

working #words bounds 2constant for-working ( -- limit index )

: w# ( i -- w# )  working - ;
: >ww ( i -- w )  working - ww ;

: #working ( -- n )  0 for-working do  i c@ +  loop ;

: .working  0 for-working do  i c@ if i >ww w. 1+ then  loop . ;


( ===== TESTS ===== )

TESTING #WORKING
T{ init-solver #working -> #words }T
T{ working 10 erase  #working -> #words 10 - }T
T{ 0 working #words + 1- c!  #working -> #words 11 - }T
T{ working #words erase  #working -> 0 }T
