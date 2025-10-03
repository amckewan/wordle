( Find words using binary search )
( Assumes that both lists are sorted )

: search ( w start end -- w# t | f )
    rot >r
    begin 2dup < while
        2dup + 2/ ( low high mid )
        dup ww  r@ len  rot len compare
        dup 0= if ( found ) r> 2drop nip nip true exit then
        0< if ( bottom half ) nip else ( top half ) rot drop  1+ swap then
    repeat
    2drop r> drop false ;

\ find a word in wordle words
: find-word ( w -- w# t | f )
    0 #hidden search ;

\ find a word in guess words
: find-guess ( w -- w# t | f )
    dup find-word if nip true else #hidden #words search then ;


\ Literal words as numbers
\  : w ( -- w# )
\      bl parse len - abort" Need 5 letters"
\      find-guess 0= abort" Not in word list" ;
\  : [w]  w  postpone literal ; immediate


( ===== TESTS ===== )
testing find-word
( wordle words )
t{ s" aback" drop find-word -> 0 true }t
t{ s" raise" drop find-word nip -> true }t
t{ s" zonal" drop find-word -> #hidden 1- true }t
( guess words)
t{ s" ablow" drop find-guess nip -> true }t
t{ s" pongo" drop find-guess nip -> true }t
t{ s" zymic" drop find-guess nip -> true }t
( invalid words )
t{ s" xxxxx" drop find-guess -> false }t
t{ s" abacc" drop find-guess -> false }t
