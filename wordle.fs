( Wordle solver )

( 1. Build an array of valid Wordle words, 5 chars per entry )

5 CONSTANT LEN

: WW,  HERE LEN ALLOT  BL WORD COUNT  ( a' a # )  ROT SWAP CMOVE ;

CREATE WORDLE-WORDS   INCLUDE wordle-words.fs

HERE WORDLE-WORDS - LEN / CONSTANT #WORDS

: WW ( n -- a )  LEN * WORDLE-WORDS + ;

: .WW ( n -- )  WW LEN TYPE SPACE ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;


( 2. Lookup a word using binary search )
: WW? ( a n -- false | w# true )
  LEN <> IF DROP FALSE EXIT THEN  >R  0 #WORDS ( low high )
  BEGIN 2DUP < WHILE
    2DUP + 2/ ( low high mid )
    DUP WW  R@ LEN  ROT LEN COMPARE
    DUP 0= IF ( found ) R> 2DROP NIP NIP TRUE EXIT  THEN
    0< IF ( bottom half ) NIP  ELSE ( top half ) ROT DROP  1+ SWAP  THEN
  REPEAT
  2DROP R> DROP FALSE ;

( with factoring )
: MORE? ( low high -- f )  2DUP < ;
: MID ( low high -- mid )  + 2/ ;
: MATCH ( a a -- n )  LEN SWAP LEN COMPARE ;
: FOUND ( low high mid -- mid true )    NIP NIP TRUE ;
: LOWER ( low high mid -- low mid )     NIP ;
: UPPER ( low high mid -- mid+1 high )  ROT DROP  1+ SWAP ;

: WW2?  ( a n -- false | w# true )
  LEN = IF  >R  0 #WORDS ( low high )
  BEGIN MORE? WHILE
    2DUP MID  R@ OVER WW MATCH
    ?DUP 0= IF FOUND  R> DROP EXIT  THEN
         0< IF LOWER  ELSE UPPER  THEN
  REPEAT
  2DROP R>  THEN DROP FALSE ;


: (TEST-WW?)  ( xt -- )  >R
  ( valid words )
  S" ABACK" R@ EXECUTE 0= ABORT" can't find ABACK" DROP
  S" ABASE" R@ EXECUTE 0= ABORT" can't find ABASE" DROP
  S" CADET" R@ EXECUTE 0= ABORT" can't find CADET" DROP
  S" RANCH" R@ EXECUTE 0= ABORT" can't find RANCH" DROP
  S" ZESTY" R@ EXECUTE 0= ABORT" can't find ZESTY" DROP
  S" ZONAL" R@ EXECUTE 0= ABORT" can't find ZONAL" DROP
  ( invalid words )
  S" XXXXX" R@ EXECUTE ABORT" found XXXXX"
  S" ABAC"  R@ EXECUTE ABORT" found ABAC"
  S" ABACKX" R@ EXECUTE ABORT" found ABACKX"
  R> DROP ;

: TEST-WW?  ['] WW? (TEST-WW?)  ['] WW2? (TEST-WW?) ;
