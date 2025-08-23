( unit test harness )

( how to use:

include unit-test.fs

: expect-true  test  not if fail ." expected true" then ;

: test-my-module
  cr ." Testing my module " begin-unit-tests

  4 5 < expect-true
  ...

  report-unit-tests ;

test-my-module

forget-unit-tests

)


marker forget-unit-tests

variable tests
variable fails

: begin-unit-tests  0 tests !  0 fails ! ;

: test  1 tests +! ;
: fail  cr ." [FAIL] "  1 fails +! ;

: report-unit-tests  cr ." ==> " tests ? ." TESTS " 
  fails @ if fails ? ." FAIL " else ." PASS " then ;
