( Pruning the working set )
: -green ( char pos w -- char pos )
    >r  2dup r@ + c@ <> if  r@ working remove  then  r> drop ;

: pg ( char pos -- )  ['] -green working for-each  2drop ;

: prune-green ( char pos -- )
    #words 0 do
        i contains if
            2dup i ww + c@ <> if  i remove  then
        then
    loop 2drop ;


( The working set has one byte per word, 0=absent, 1=present )
create working  #words allot

: all-words  working #words 1 fill ;

: contains ( n -- f )  working + c@ ;
( todo remove ) : has ( n -- f ) contains ;
: remove ( n -- )  0 swap working + c! ;

: remove-all  working #words erase ;

: #working  ( -- n )  0  #words 0 do  i has +  loop ;
: .working  #words 0 do  i has if  i ww w.  then  loop ;


( iterator, no wrap )
: next ( n -- n' true | false )
  begin  1+  dup #words < while
    dup working + c@ if  true exit  then
  repeat  drop false ;

: first ( -- n true | false )  -1 next ;


: remove# ( n -- )  0 swap working + c! ;

\ Remove words from the working set that don't have the green letter.
\ ex: we know the first letter is 'A', remove words that don't start with 'A'
: prune-green ( char pos -- )
    #words 0 do
        i contains if
            2dup i ww + c@ <> if  i remove#  then
        then
    loop 2drop ;

\ Check if a word has this letter in any position, ignoring green letters.
\ We use 'score' to determine whether a letter is green.
: has-letter ( char w -- f )
    len 0 do
        i green? not if
            2dup i + c@ = if ( found one ) 2drop true  unloop exit then
        then
    loop 2drop false ;

: missing-letter  has-letter not ;

\ Remove any words that do or don't have this letter (ignoring greens)
: prune-if ( char xt -- )
    #words 0 do
        i contains if
            2dup i ww swap execute if  i remove#  then
        then
    loop 2drop ;

: prune-yellow ( char -- )  ['] missing-letter prune-if ;
: prune-grey   ( char -- )  ['] has-letter     prune-if ;


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


: expect-has ( w char -- )  test  swap 2dup has-letter not
  if fail ." expected " w. ." to contain " emit  else 2drop then ;

: expect-has-not ( w char -- )  test  swap 2dup has-letter
  if fail ." expected " w. ." not to contain " emit  else 2drop then ;

: test-has-letter
  cr ." Testing has-letter..." begin-unit-tests

  clear-score ( to avoid side effects )
  [w] ABCDE [char] A expect-has
  [w] ABCDE [char] B expect-has
  [w] ABCDE [char] E expect-has
  [w] ABCDE [char] M expect-has-not

  \ make sure we skip greens
  green score c! ( score the first letter green )
  [w] ABCDE [char] A expect-has-not
  [w] ABCDA [char] A expect-has
  [w] ABCDE [char] B expect-has

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

