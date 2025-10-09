( working set )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each guess by removing words that wouldn't get that score.
\
\ The working set always contains at least the secret (which is never pruned).
\
\ We implement the set as a linked list. Each entry in workset contains the
\ address of the next entry or 0 to end the list.

create workset   #words cells allot
 
variable working    ( head of the working set linked list )
variable hidden     ( true if the solver can use the hidden word list )

: >w ( working -- w )  workset - [ 1 cells ] literal / ;

: #working ( -- n )  hidden @ if #hidden else #words then ;

: init-working ( add words to the working set )
    workset dup working !
    #working 1 do  dup cell+  dup rot !  loop
    0 swap ! ( terminate the list ) ;

: remaining ( -- n )  0  working @ begin  swap 1+ swap  @ ?dup 0= until ;
: .working  0 working @ begin  dup >w w.  swap 1+ swap  @ ?dup 0= until . ;

\ Prune the working set, removing words that wouldn't produce this score.
: prune ( -- )  latest 2>r
    working begin dup @ dup while
        dup 2r@ rot >w prune? if  @ over !  else  nip  then
    repeat 2r> 2drop 2drop ;



( ===== TESTS ===== )
testing remaining
t{ hidden on  init-working remaining -> #hidden }t
t{ hidden off init-working remaining -> #words  }t
t{ workset 3 cells + working ! remaining -> #words 3 - }t
t{ workset working ! 0 workset ! remaining -> 1 }t
