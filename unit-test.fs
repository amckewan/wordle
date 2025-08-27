( unit test harness )

marker forget-unit-tests

variable tests
variable fails

: begin-unit-tests ( a n -- )
    cr ." TESTING " type ." ... "  0 tests !  0 fails ! ;

: test  1 tests +! ;
: fail  cr ." [FAIL] "  1 fails +! ;

: report-unit-tests  cr ." ==> " tests ? ." TESTS " 
  fails @ if fails ? ." FAIL " else ." PASS " then ;

\ common tests
: expect-true  ( f -- )  test not if fail ." Expected TRUE "  then ;
: expect-false ( f -- )  test     if fail ." Expected FALSE " then ;
: expect-equal ( n expected --  )
    test  2dup <> if fail ." Expected ". ." got ". else 2drop then ;


( === How to use ===

include unit-test.fs

: expect-true  test  not if fail ." expected true" then ;

: test-my-module
  s" my module" begin-unit-tests

  4 5 < expect-true
  ...
  answer 42 expect-equal

  report-unit-tests ;

test-my-module

forget-unit-tests
)
