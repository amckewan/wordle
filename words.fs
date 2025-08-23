( Wordle words )

\ Wordle words are 5 characters long. When passing strings, we only need the address.
\ This is 'w' in the stack diagrams.

5 CONSTANT LEN

: W! ( w1 w2 -- )  LEN CMOVE ;
: W, ( w -- )  HERE LEN ALLOT W! ;
: W. ( w -- )  LEN TYPE SPACE ;

( literals )
: W ( -- w )  BL PARSE DROP ;
: [W] ( -- w [at runtime] )  BL PARSE  POSTPONE SLITERAL  POSTPONE DROP ; IMMEDIATE

: WCOMPARE ( w1 w2 -- -1/0/1 )  LEN SWAP LEN COMPARE ;
: W= ( w1 w2 -- f )  WCOMPARE 0= ;


\ There are two lists of words, those that can be solutions (WORDLE-WORDS)
\ and those that can be guesses but not solutions (GUESS-WORDS).
\ The lists are in alphabetical order to allow binary search.

\ Add the next wordle word to the dictionary (to build the word lists).
: WW,  W W, ;

\ Create the list of valid wordle solutions.
CREATE WORDLE-WORDS
INCLUDE wordle-words.fs

HERE WORDLE-WORDS - LEN / CONSTANT #WORDS

\ Create the list of possible guesses. These are allowed as guesses but won't be solutions.
CREATE GUESS-WORDS
INCLUDE guess-words.fs

HERE GUESS-WORDS - LEN / CONSTANT #GUESS-WORDS

\ get wordle word from word #
: WW ( w# -- w )  LEN * WORDLE-WORDS + ;

: .WW ( w# -- )  WW W. ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;


\ Search a sorted list of words, returning the word and true if found, else false.
\ The w returned is in the list, not necessarily (and most likely not) the w passed in.
\ We do that so the returned word is static and we can keep it around if needed.

: MID ( low high -- mid )  OVER - LEN / ( low n )  2/ LEN * + ;

: LOWER  ( low high mid -- low mid )  NIP ;
: HIGHER ( low high mid -- mid+1 high)  ROT DROP  LEN + SWAP ;

: FIND-WORD ( w words #words -- w' true | false)   ( binary search )
    ROT >R  LEN * OVER + ( low high )
    BEGIN 2DUP U< WHILE
        2DUP MID  R@ OVER WCOMPARE ( low high mid n )
        ?DUP 0= IF ( found ) NIP NIP TRUE   R> DROP EXIT THEN
        0< IF LOWER ELSE HIGHER THEN
    REPEAT  2DROP FALSE   R> DROP ;

: FIND-WORDLE-WORD ( w -- w' true | false)  WORDLE-WORDS #WORDS       FIND-WORD ;
: FIND-GUESS-WORD  ( w -- w' true | false)  GUESS-WORDS  #GUESS-WORDS FIND-WORD ;


( === unit tests === )
include unit-test.fs

create test-words
WW, ABACK
WW, BLANK
WW, CHOIR
WW, DELTA
WW, EMCEE
WW, FORTH
WW, GRILL
WW, HOUSE
WW, IGLOO
WW, JAPAN
here test-words - len / constant #test-words

\ : FIND-WORD ( w words #words -- w' true | false)
: find-test ( w -- w' t | f ) test-words #test-words find-word ;

: expect-found ( w -- )  test
  dup find-test not if fail ." Expected to find " W. else 2drop then ;

: expect-not-found ( w -- )  test
  find-test if fail ." Expected not to find " W. then ;

: test-find-word
  CR ." Testing FIND-WORD..." begin-unit-tests
  [W] ABACK expect-found
  [w] BLANK expect-found
  [w] CHOIR expect-found
  [w] JAPAN expect-found
  [w] ZESTY expect-not-found
  [w] XXXXX expect-not-found

  \ test an empty list
  test [w] ABACK test-words 0 find-word
  if fail ." Expect not to find in a empty table" drop then

  report-unit-tests ;

test-find-word

forget-unit-tests
