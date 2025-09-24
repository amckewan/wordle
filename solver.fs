( solver )

\ The working set contains the words that could be the solution.
\ We start the game with all the hidden words then prune the set
\ after each score by removing words that wouldn't get that score.

#hidden constant working-size

create working  working-size allot  \ one byte per word, 0=absent, 1=present

: all-words  working working-size 1 fill ;

: has ( n -- f )   working + c@ ;
: remove ( n -- )  working + 0 swap c! ;

: #working ( -- n )  0 working working-size bounds do i c@ + loop ;

: .working 0 working-size 0 do i has if i hidden@ w. 1+ then loop . ." words " ;



( ===== TESTS ===== )

TESTING #WORKING
T{ all-words #working -> working-size }T
T{ working 10 erase  #working -> working-size 10 - }T
T{ 0 working working-size + 1- c!  #working -> working-size 11 - }T
T{ working working-size erase  #working -> 0 }T
