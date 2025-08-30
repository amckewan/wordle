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
: expected ( a n -- )  fail ." Expected " type space ;
: expect-true  ( f -- )  test not if s" TRUE" expected  then ;
: expect-false ( f -- )  test     if s" FALSE" expected then ;
: expect-equal ( n expected --  )
    test  2dup <> if 0 0 expected . ." got " . else 2drop then ;


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
