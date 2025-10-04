( working set )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each guess by removing words that wouldn't get that score.

\ The working set always contains at least the secret (which is never pruned),
\ so `working` always points to a hidden word in wordle-words.
 
variable working    ( head of the working set )

: all-words ( add all words to the working set )
    wordle-words begin   dup wsize +   dup rot !   dup words-end = until
    wsize - off ( terminate the list ) wordle-words working ! ;

: snip-hidden  0 hidden-end wsize - ! ; ( leave just the hidden words )

: remaining ( -- n )  0  working @ begin  swap 1+ swap  @ ?dup 0= until ;

: remaining-hidden ( -- n ) \ just consider hidden words
    0 working @ begin  dup hidden-end u< not if drop exit then  swap 1+ swap
    @ ?dup 0= until ;

t{ all-words remaining -> #words }t
t{ snip-hidden remaining -> #hidden }t
t{ remaining-hidden -> #hidden }t
t{ all-words remaining-hidden -> #hidden }t

: .entry  dup cell+ w.  swap 1+ swap  @ ?dup 0= ;
: .working  0 working @ begin .entry until . ;
: .hidden  0 working @ begin  dup hidden-end u< not if drop . exit then
    .entry until . ;
