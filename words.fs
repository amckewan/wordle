( Wordle words )

\ There are two lists of words, those that can be solutions and those that can be guesses but not solutions.

\ Build an array of valid Wordle words, 5 chars per entry.
\ Wordle words are identified by the index into this array.
\ Use 'w' in stack diagrams.

5 CONSTANT LEN

: WW,  HERE LEN ALLOT  BL WORD COUNT  ( a' a # )  ROT SWAP CMOVE ;

CREATE WORDLE-WORDS   INCLUDE wordle-words.fs

HERE WORDLE-WORDS - LEN / CONSTANT #WORDS

\ Create the list of possible guess. These are allowed as guesses but won't be solutions.
CREATE GUESSES  INCLUDE guess-words.fs
HERE GUESSES - LEN / CONSTANT #GUESSES


: ASSERT-WORD ( w -- w )  DUP #WORDS U>= ABORT" invalid wordle word" ;

: WW ( w -- a )  LEN * WORDLE-WORDS + ;

: .WW ( w -- )  WW LEN TYPE SPACE ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;


( 2. Lookup a word using binary search )
: WORD? ( a n -- false | w true )
  LEN <> IF DROP FALSE EXIT THEN
  ( a ) >R  0 #WORDS ( low high )
  BEGIN 2DUP < WHILE
    2DUP + 2/ ( low high mid )
    DUP WW  R@ LEN  ROT LEN COMPARE
    DUP 0= IF ( found ) R> 2DROP NIP NIP TRUE EXIT  THEN
    0< IF ( bottom half ) NIP  ELSE ( top half ) ROT DROP  1+ SWAP  THEN
  REPEAT
  2DROP R> DROP FALSE ;

: MATCH ( a1 a2 -- n )  LEN SWAP LEN COMPARE ;

\ Need to refactor so we can use it for both words and guesses
: FIND-WORD ( a n words #words -- w true | false)
  ROT LEN = IF  SWAP >R  0 SWAP ( a low high )
    BEGIN 2DUP < WHILE
      2DUP + 2/ ( a low high mid )
      DUP LEN * R@ + ( a low high mid a' ) 4 PICK ( !) MATCH ( a low high mid f )
      DUP 0= IF ( found ) R> 2DROP NIP NIP NIP  TRUE EXIT  THEN
      0< IF ( bottom half ) NIP  ELSE ( top half ) ROT DROP  1+ SWAP  THEN
    REPEAT
    R> DROP
  THEN 2DROP DROP FALSE ;


( with factoring )
: MORE? ( low high -- f )  2DUP < ;
: MID ( low high -- mid )  + 2/ ;
: MATCH ( a a -- n )  LEN SWAP LEN COMPARE ;
: FOUND ( low high mid -- mid true )    NIP NIP TRUE ;
: LOWER ( low high mid -- low mid )     NIP ;
: UPPER ( low high mid -- mid+1 high )  ROT DROP  1+ SWAP ;

: WORD2?  ( a n -- false | w# true )
  LEN = IF  >R  0 #WORDS ( low high )
  BEGIN MORE? WHILE
    2DUP MID  R@ OVER WW MATCH
    ?DUP 0= IF FOUND  R> DROP EXIT  THEN
         0< IF LOWER  ELSE UPPER  THEN
  REPEAT
  2DROP R>  THEN DROP FALSE ;


( === unit tests === )
include unit-test.fs

variable word-finder ( so we can try both )  ' word? word-finder !
: check-valid  ( a n -- a n w t | a n f )  2dup word-finder @ execute ;

: expect-valid ( a n -- )  test
    check-valid 0= if  fail ." expect '" TYPE ." ' valid"
    else  drop 2drop  then ;
: expect-not-valid ( a n -- )  test
    check-valid if  drop fail  ." expect '" TYPE ." ' not valid"
    else  2drop  then ;

: (test-word?) ( xt -- )  word-finder !
  begin-unit-tests
  s" ABACK" expect-valid
  s" ABASE" expect-valid
  s" CADET" expect-valid
  s" RANCH" expect-valid
  s" ZESTY" expect-valid
  s" ZONAL" expect-valid

\  ( these fail to test expect... )
\  s" ZONAL" expect-not-valid
\  s" XXXXX" expect-valid

  s" XXXXX" expect-not-valid
  s" ABAC" expect-not-valid
  s" ABACKX" expect-not-valid

  report-unit-tests ;

: TEST-WORD?
  cr ." Testing (WORD?)..."  ['] WORD?  (test-word?)
  cr ." Testing (WORD2?)..." ['] WORD2? (test-word?) ;

test-word?
forget-unit-tests
