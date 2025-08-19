( Wordle solver )

include words.fs
include set.fs

( remove green letters )
( ex: we know the first letter is 'A', remove words that don't start with 'A' )

: remove-green ( char pos -- )
  first begin while ( char pos n)
    dup has if  >r  2dup r@ ww + c@ <> if r@ remove then  r>  then
    next
  repeat 2drop ;


( === Unit Tests === )
include unit-test.fs

: test-green
  ." Testing green..." begin-unit-tests
  all-words
  [char] Z 0 remove-green ( there are 3 Zs )
  #working 3 <> if fail ." expected 3 words starting with Z" then 
  
  ( there are 424 words ending in E )
  all-words
  [char] E 4 remove-green
  #working 424 <> if fail ." expected 424 words ending with E" then 
  
  report-unit-tests ;


\ unit-tests
