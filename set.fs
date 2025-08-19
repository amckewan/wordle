( working set )

( one byte per word, 0=absent, 1=present )
create working  #words allot

: add-all  working #words 1 fill ;

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

( unit tests )
include unit-test.fs

: expect-has ( n -- )  test
  dup has 0= if  fail ." expect " dup . ." found" then drop ;
: expect-has-not ( n -- )  test
  dup has if  fail ." expect " dup . ." not found"  then drop ;

: test-working
  cr ." Testing working set..." begin-unit-tests

  remove-all
  0 expect-has-not
  1 expect-has-not
  #words 2/ expect-has-not
  #words 1- expect-has-not

  add-all
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

  report-unit-tests ;

test-working
unit-tests
