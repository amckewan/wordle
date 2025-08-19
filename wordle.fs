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

\ for yellow, we want to remove words that don't have that letter,
\  except if that letter is already a greeen.



( current answer, char if green else 0 )
create answer len allot
: clear-answer  answer len erase ; clear-answer

: .answer  len 0 do  i answer + c@  ?dup 0= if [char] ? then emit  loop space ;

\ remove all words that don't contain the letter, ignoring greens

\ check if word n has this letter, ignoring green letters
: has-letter ( char n -- f )
  false  len 0 do
    i answer + c@ ( green? )
    0= if ( char n f ) over ww i + c@ 3 pick = ( yuk) or  then 
  loop nip nip ;

: REMOVE-YELLOW ( char -- )
  first begin while ( char n )
    2dup has-letter 0= if  dup remove  then
    next
  repeat drop ;


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

: expect-has ( n char -- )  swap 2dup has-letter 0=
  if fail ." expected " .ww ." to contain " emit
  else 2drop then ;
: expect-has-not ( n char -- )  swap 2dup has-letter
  if fail ." expected " .ww ." to not contain " emit
  else 2drop then ;

: test-yellow
  ." Testing yellow..." begin-unit-tests

  ( first word is ABACK )
  0 [char] A expect-has
  0 [char] B expect-has
  0 [char] C expect-has
  0 [char] D expect-has-not

  
  report-unit-tests ;

test-green
test-yellow
\ forget-unit-tests

