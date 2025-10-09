( Wordle words )

\ There are two lists of words, those that can be solutions (hidden-words)
\ and those that can be guesses but not solutions (guess-words).
\ The lists are disjoint and we lay them down one after the other.
\ We identify a word by its offset into the list (`w` in stack diagrams).

5 constant len          ( Wordle words are 5 characters long )

: ww, ( "aword" -- )  bl parse drop  here len  dup allot move ;

create wordle-words
include data/hidden-words.fs here
include data/guess-words.fs here

wordle-words - len / constant #words    ( number of words )
wordle-words - len / constant #hidden   ( just the possible solutions )

\ get wordle-word string from word #
: ww ( w -- a )  len * wordle-words + ;
: w. ( w -- )    ww len type space ;

\ find a word using in one of the lists using a binary search
: search ( a start end -- w t | f )  rot >r
    begin 2dup < while
        2dup + 2/ ( low high mid )
        dup ww  r@ len  rot len compare
        dup 0= if ( found ) r> 2drop  nip nip true exit then
        0< if ( bottom half ) nip else ( top half ) rot drop  1+ swap then
    repeat  2drop false  r> drop ;
: find-word ( a -- w t | f )  \ find word in either list
    dup 0 #hidden search if nip true exit then  #hidden #words search ;

\ word literals
: ?len ( n -- )  len - abort" need 5 letters" ;
: w ( -- w )  bl parse ?len find-word 0= abort" Not in word list" ;
: [w]  w  postpone literal ; immediate


( ===== TESTS ===== )
testing find-word
( wordle words )
t{ s" aback" drop find-word -> 0 true }t
t{ s" raise" drop find-word nip -> true }t
t{ s" zonal" drop find-word -> #hidden 1- true }t
( guess words)
t{ s" ablow" drop find-word nip -> true }t
t{ s" pongo" drop find-word nip -> true }t
t{ s" zymic" drop find-word nip -> true }t
( invalid words )
t{ s" xxxxx" drop find-word -> false }t
t{ s" abacc" drop find-word -> false }t

testing w
t{ w aback -> 0 }t
t{ 0 ww len s" aback" compare -> 0 }t
t{ w zonal -> #hidden 1- }t
