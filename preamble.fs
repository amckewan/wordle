( preamble )

: not 0= ;
\ : bounds over + swap ; ( gforth has )

variable timing \ turn on to show elapsed times
: timestamp ( -- us ) utime drop ; \ gforth
: .### ( n -- )  0 <# # # # '.' hold #s #> type space ;
: .elapsed ( us -- ) dup 999999 > if 1000 / .### ." sec " else .### ." ms " then ;
: time  timestamp >r  ' execute  timestamp r> - .elapsed ;

include utils/tester.fs
include utils/random.fs
