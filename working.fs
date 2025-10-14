( working set )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each guess by removing words that wouldn't get that score.
\
\ The working set always contains at least the secret (which is never pruned).
\
\ We implement the set as a linked list. Each entry in workset contains the
\ word # of the next entry or -1 to end the list.

create workset   #words cells allot
 
variable working    ( head of the working set linked list )
variable hidden     ( true if the solver can use the hidden word list )

: next  ( w1 -- w2 )        cells workset + @ ;
: next? ( w1 -- w2 f | t )  next ?dup 0= ;

: #working ( -- n )  hidden @ if #hidden else #words then ;

: init-working ( add words to the working set )
    workset  #working 1 do  i over ! cell+  loop  0 swap !  0 working ! ;

: remaining ( -- n )  0  working @ begin  swap 1+ swap  next? until ;
: .working  working @ begin  dup w.  next? until  remaining . ;

\ Prune the working set, removing words that wouldn't produce this score.
: prune ( -- )  latest 2>r
    working begin  dup @ ( next )
        dup 2r@ rot prune? if  next over !  else  cells workset +  nip  then
    dup @ 0= until drop  2r> 2drop ;



( ===== TESTS ===== )
testing remaining
t{ hidden on  init-working remaining -> #hidden }t
t{ hidden off init-working remaining -> #words  }t
t{ 3 working ! remaining -> #words 3 - }t
t{ 0 working ! 0 workset ! remaining -> 1 }t
