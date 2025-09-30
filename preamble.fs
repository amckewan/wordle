( preamble )

: not 0= ;
\ : bounds over + swap ; ( gforth has )

variable timing \ turn on to show elapsed times
: timestamp ( -- us ) utime drop ; \ gforth

include utils/tester.fs
include utils/random.fs
