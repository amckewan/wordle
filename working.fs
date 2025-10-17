( working set )

\ The working set contains the words that could be the solution.
\ We start the game with all the wordle words then prune the set
\ after each guess by removing words that wouldn't get that score.
\
\ The working set always contains at least the secret (which is never pruned).
\
\ We implement the set as a linked list. Each entry in workset contains the
\ word # of the next entry or 0 to end the list.

create workset   #words cells allot
 
variable working    ( head of the working set linked list )

: all-words  ( add all words to the working set )
    0 working !  workset #words 1 do i over ! cell+ loop  0 swap ! ;

: next  ( w1 -- w2 )        cells workset + @ ;
: next? ( w1 -- w2 f | t )  next ?dup 0= ;

: remaining ( -- n )  0  working @ begin  swap 1+ swap  next? until ;

: .working  working @ begin  dup w.  next? until  remaining . ;

\ Prune the working set to just the hidden words
: prune-hidden ( -- )
    working @ begin  cells workset + dup @  dup #hidden u< and while @ repeat
    0 swap ! ( snip ) ;

\ Prune the working set, removing words that wouldn't produce this score.
: prune ( -- )  latest 2>r
    working begin  dup @ ( next )
        dup 2r@ rot prune? if  next over !  else  cells workset +  nip  then
    dup @ 0= until drop  2r> 2drop ;



( ===== TESTS ===== )
testing remaining
t{ all-words remaining -> #words }t
t{ 3 working ! remaining -> #words 3 - }t
t{ 0 working ! 0 workset ! remaining -> 1 }t

testing prune-hidden
t{ all-words remaining -> #words }t
t{ prune-hidden remaining -> #hidden }t
t{ prune-hidden remaining -> #hidden }t

