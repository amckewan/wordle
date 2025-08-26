( A set of wordle words )

\ One byte per word, 0=absent, 1=present
: set  create  here #words  dup allot  erase ;

: clear-set ( set -- ) #words erase ;
: fill-set  ( set -- ) #words 1 fill ;


\ NOTE: These only work correctly for words in wordle-words (provided by for-each)
: contains ( w set -- f )  swap w# +  c@ negate ; ( needed? )
: remove   ( w set -- )    swap w# +  0 swap c! ;

\ Execute xt for each element in the set. xt does ( ... w --- ... )
: for-each ( ... xt set -- ... )
    #words 0 do
        dup i + c@ if  i ww  swap >r  swap dup >r execute  r> r>  then
    loop 2drop ;

\ : .set ( set -- )  0 swap  #words bounds do  i c@ if  dup ww w.  then  1+ loop drop ;
: .set ( set -- )  ['] w. swap for-each ;

: inc ( n w -- n+1 )  drop 1+ ;
: set-size ( set -- n )  0 ['] inc rot for-each ;


( === unit tests === )
include unit-test.fs

set testing

: expect-contains ( w -- )  test
    dup testing contains not if fail ." Expected to contain " dup w. then drop ;

: expect-contains-not ( w -- )  test
    dup testing contains if fail ." Expected not to contain " dup w. then drop ;

: test-set
    cr ." Testing set..." begin-unit-tests

    testing clear-set
    0 ww expect-contains-not
    100 ww expect-contains-not
    #words 1- ww expect-contains-not

    testing fill-set
    0 ww expect-contains
    100 ww expect-contains
    #words 1- ww expect-contains

    \ remove some
    10 ww  dup testing remove  expect-contains-not
    20 ww  dup testing remove  expect-contains-not
    30 ww  dup testing remove  expect-contains-not
    31 ww                      expect-contains

    report-unit-tests ;


: expect-size ( n -- )  test
    testing set-size 2dup <> if fail ." Expected " swap . ." got " . else 2drop then ;
  
: test-set-size
    cr ." Testing SET-SIZE..." begin-unit-tests

    testing clear-set  0 expect-size
    testing fill-set   #words expect-size

    111 ww testing remove
    222 ww testing remove
    333 ww testing remove
    #words 3 - expect-size

    report-unit-tests ;


23 constant #Qs \ There are 23 words starting with Q

: count-letters ( n c w -- n' c )  c@ over = if  swap 1+ swap  then ;

: expect-count ( c n -- )  test
    0 rot ['] count-letters testing for-each drop
    2dup <> if fail ." Expected " swap . ." got " . else 2drop then ;

: test-for-each
    cr ." Testing FOR-EACH..." begin-unit-tests
    
    \ There are 23 Qs and 3 Zs
    testing fill-set
    [char] Q 23 expect-count
    [char] Z  3 expect-count
    
    report-unit-tests ;

test-set
test-set-size
test-for-each

\ forget-unit-tests
