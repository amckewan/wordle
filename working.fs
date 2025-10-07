( working set )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each guess by removing words that wouldn't get that score.

\ The working set always contains at least the secret (which is never pruned),
\ so `working` always points to a hidden word in wordle-words.
 
variable working    ( head of the working set linked list )
variable hidden     ( true to use the hidden word list )

: all-words ( add all words to the working set )
    wordle-words begin   dup wsize +   dup rot !   dup words-end = until
    wsize - off ( terminate the list ) wordle-words working ! ;

: snip-hidden  0 hidden-end wsize - ! ; ( leave just the hidden words )

: remaining ( -- n )  0  working @ begin  swap 1+ swap  @ ?dup 0= until ;

: remaining-hidden ( -- n ) \ just consider hidden words
    0 working @ begin  dup hidden-end u< not if drop exit then  swap 1+ swap
    @ ?dup 0= until ;

: .entry  dup cell+ w.  swap 1+ swap  @ ?dup 0= ;
: .working  0 working @ begin .entry until . ;
: .hidden  0 working @ begin  dup hidden-end u< not if drop . exit then
    .entry until . ;

\ Prune the working set, removing words that wouldn't produce this score
: prune ( -- )  latest 2>r
    working begin dup @ dup while
        dup 2r@ rot cell+ prune? if  @ over !  else  nip  then
    repeat 2drop 2r> 2drop ;



( ===== TESTS ===== )
testing remaining
t{ all-words remaining -> #words }t
t{ snip-hidden remaining -> #hidden }t
t{ remaining-hidden -> #hidden }t
t{ all-words remaining-hidden -> #hidden }t
t{ wordle-words wsize 3 * + working ! remaining -> #words 3 - }t
t{ remaining-hidden -> #hidden 3 - }t
