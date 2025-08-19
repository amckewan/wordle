( working set )

( one byte per word, 0=absent, 1=present )
create working  #words allot

: all-words  working #words 1 fill ;

( no bounds check )
: has ( n -- f )   working + c@ ;
: remove ( n -- )  0 swap working + c! ;

: remove-all  working #words erase ;

: #working  ( -- n )  0  #words 0 do  i has +  loop ;
: .working  #words 0 do  i has if  i .ww  then  loop ;


( iterator, no wrap )
: next ( n -- n' true | false )
  begin  1+  dup #words < while
    dup has if true exit then
  repeat drop false ;

: first ( -- n true | false )  -1 next ;


( === unit tests === )
include unit-test.fs

: expect-has ( n -- )  test
  dup has 0= if  fail ." expect " dup . ." found" then drop ;
: expect-has-not ( n -- )  test
  dup has if  fail ." expect " dup . ." not found"  then drop ;

: test-working
  cr ." Testing working set..." begin-unit-tests

  remove-all
  #working if fail ." expected working empty" then
  0 expect-has-not
  1 expect-has-not
  #words 2/ expect-has-not
  #words 1- expect-has-not

  all-words
  #working #words - if fail ." expected working full" then
  0 expect-has
  1 expect-has
  #words 2/ expect-has
  #words 1- expect-has

  ( remove some )
  0 dup remove expect-has-not
  1 dup remove expect-has-not
  55 dup remove expect-has-not
  #words 1- dup remove expect-has-not
  2 expect-has
  #working 4 + #words - if fail ." expected working paritally full" then

  report-unit-tests ;

: expect ( [n'] f n -- )
  swap 0= if fail ." expected " . ." got none"
  else 2dup <> if fail ." expected " . ." got " . else 2drop then then ;
: expect-none ( [n] f -- ) if fail ." expected none, got " . then ;

: test-iterators
  cr ." Testing working set interators..."
  begin-unit-tests

  all-words
  first 0 expect
  0 next 1 expect
  100 next 101 expect

  0 remove 1 remove 2 remove
  first 3 expect
  0 next 3 expect
  1 next 3 expect
  3 next 4 expect

  101 remove 102 remove
  100 next 103 expect

  remove-all
  first expect-none
  0 next expect-none

  report-unit-tests ;


test-working
test-iterators
forget-unit-tests
