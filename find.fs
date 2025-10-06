( Find words using binary search )
( Assumes that both lists are sorted )

: search ( a start# end# -- w# t | f )  rot >r
    begin 2dup < while
        2dup + 2/ ( low high mid )
        dup ww  r@ len  rot len compare
        dup 0= if ( found ) r> 2drop  nip nip true exit then
        0< if ( bottom half ) nip else ( top half ) rot drop  1+ swap then
    repeat  2drop false  r> drop ;

\  : find-hidden ( a -- w# t | f )  0 #hidden search ;
\  : find-guess  ( a -- w# t | f )  #hidden #words search ;
\  : find-word   ( a -- w# t | f )  dup find-hidden if nip true exit then find-guess ;

: find-word ( a -- w# t | f )  \ find word in either list
    dup 0 #hidden search if nip true exit then  #hidden #words search ;

: >ww ( a -- w )  find-word 0= abort" Not in word list" ww ;

: >hidden ( a -- w )  0 #hidden search 0= abort" Not in hidden list" ww ;
\  : secret! ( a -- )    >hidden to secret ;

\ word literals that must be actual wordle words
: x ( -- w )  bl parse drop >ww ;
: [x]  x postpone literal ; immediate



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
