( Pruning the working set )

\ Remove words from the working set that don't have the green letter.
\ ex: we know the first letter is 'A', remove words that don't start with 'A'
: prune-green ( char pos -- )
  first begin while ( char pos w )
    >r  2dup r@ ww + c@ <> if r@ remove then  r>
    next
  repeat 2drop ;


\ For yellow, we want to remove words that don't have that letter,
\ except if that letter is already green.
\ Grey is similar except remove if a word _does_ have the letter

\ Check if a word has this letter in any position, ignoring green letters
: has-letter ( char w -- f )
  false  len 0 do
    i green? 0= if ( char w f ) over ww i + c@  3 pick = or  then 
  loop nip nip ;

: missing-letter  has-letter 0= ;

\ remove words that either do or don't have a letter, ignoring greens
: prune-if ( char xt -- )  >r
  first begin while ( char w )
    2dup r@ execute if  dup remove  then
    next
  repeat r> 2drop ;

\ Remove words that don't have this letter
: prune-yellow ( char -- )  ['] missing-letter prune-if ;

\ Remove words that _do_ have this letter
: prune-grey ( char -- )  ['] has-letter prune-if ;



( === Unit Tests === )
include unit-test.fs

: test-green
  cr ." Testing green..." begin-unit-tests
  all-words
  [char] Z 0 prune-green ( there are 3 Zs )
  test #working 3 <> if fail ." expected 3 words starting with Z" then 
  
  ( there are 424 words ending in E )
  all-words
  [char] E 4 prune-green
  test #working 424 <> if fail ." expected 424 words ending with E" then 
  
  report-unit-tests ;


: expect-has ( w char -- )  test  swap 2dup has-letter 0=
  if fail ." expected " .ww ." to contain " emit
  else 2drop then ;
: expect-has-not ( w char -- )  test  swap 2dup has-letter
  if fail ." expected " .ww ." to not contain " emit
  else 2drop then ;

: test-has-letter
  cr ." Testing has-letter..." begin-unit-tests

  ( first word is ABACK )
  0 [char] A expect-has
  0 [char] B expect-has
  0 [char] C expect-has
  0 [char] D expect-has-not

  ( check a random word )
  len 0 do  555  dup ww i + c@ expect-has  loop

  report-unit-tests ;

: expect-equal ( n n' -- )  test
  2dup <> if fail ." expected equal " swap . . else 2drop then ;

\ We know there are 29 words with a Q
: test-yellow-grey
  cr ." Testing prune-yellow/grey..." begin-unit-tests

  all-words
  [char] Q prune-yellow
  #working 29 expect-equal

  all-words
  [char] Q prune-grey
  #working #words 29 - expect-equal

  report-unit-tests ;

test-green
test-has-letter
test-yellow-grey

forget-unit-tests

