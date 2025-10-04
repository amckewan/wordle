( preamble )

: not 0= ;
\ : bounds over + swap ; ( gforth has )

variable timing \ turn on to show elapsed times
: timestamp ( -- us ) utime drop ; \ gforth
: .### ( n -- )  0 <# # # # '.' hold #s #> type space ;
: .elapsed ( us -- )  dup 1000 < if . ." us" else
    dup 1000000 < if .### ." ms " else 1000 / .### ." sec " then then ;
: time  timestamp >r  ' execute  timestamp r> - .elapsed ;

include utils/tester.fs
include utils/random.fs
