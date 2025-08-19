( unit test harness )

( how to use:

include unit-test.fs

: test-my-module
  begin-unit-tests
  ...
  report-unit-tests ;

test-my-module

unit-tests

)

marker unit-tests

variable tests
variable fails

: begin-unit-tests  0 tests !  0 fails ! ;

: test  1 tests +! ;
: fail  cr ." [FAIL] "  1 fails +! ;

: report-unit-tests  ."  ==> tests: " tests ? ." fail: " fails ? ;
