( Wordle words )

\ There are two lists of words, those that can be solutions and those that can be guesses but not solutions.

\ Build an array of valid Wordle words, 5 chars per entry.
\ Wordle words are identified by the index into this array.

\ Wordle words are 5 characters long. When passing strings, we only need the address.
\ This is 'wa' in stack diagrams (word address).

5 CONSTANT LEN

: W. ( w -- )  LEN TYPE SPACE ;

( literals )
: W ( -- w )  BL PARSE DROP ;
: [W] ( -- w at runtime )  BL PARSE  POSTPONE SLITERAL  POSTPONE DROP ; IMMEDIATE

\ Add the next wordle word to the dictionary (to build the word lists).
: WW,  HERE LEN ALLOT  BL PARSE ( dest src # )  ROT SWAP CMOVE ;

\ Create the list of valid wordle solutions (sorted).
\ In stack diagrams, a 'w' is a word number, which is an offset into this array. NO NO
CREATE WORDLE-WORDS
  INCLUDE wordle-words.fs
HERE CONSTANT WORDLE-WORDS-END

WORDLE-WORDS-END WORDLE-WORDS - LEN / CONSTANT #WORDS

\ Create the list of possible guesses. These are allowed as guesses but won't be solutions.
CREATE GUESS-WORDS
  INCLUDE guess-words.fs
HERE CONSTANT GUESS-WORDS-END

GUESS-WORDS-END GUESS-WORDS - LEN / CONSTANT #GUESS-WORDS

\ get wordle word from word #
: WW ( w# -- w )  LEN * WORDLE-WORDS + ;

: .WW ( w# -- )  WW LEN TYPE SPACE ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;

: WMOVE ( w1 w2 -- )  LEN CMOVE ; ( useful )


\ Search a sorted list of words, returning the word and true if found, else false.
\ The w returned is in the list, not necessarily (and most likely not) the w passed in.
\ We do that so the returned word is static and we can keep it around if needed.

: WCOMPARE ( w1 w2 -- -1/0/1 )  LEN SWAP LEN COMPARE ;
: MID ( low high -- mid )  OVER - LEN / ( low n )  2/ LEN * + ;

: FIND-WORD ( w words #words -- w' true | false)   ( binary search )
    ROT >R  LEN * OVER + ( low high )
    BEGIN 2DUP U< WHILE
        2DUP MID  R@ OVER WCOMPARE ( low high mid n )
        ?DUP 0= IF ( found ) NIP NIP TRUE   R> DROP EXIT THEN
        0< IF ( bottom half ) NIP  ELSE ( top half ) ROT DROP  LEN + SWAP  THEN
    REPEAT  2DROP FALSE   R> DROP ;

: FIND-WORDLE-WORD ( w -- w' true | false)  WORDLE-WORDS #WORDS FIND-WORD ;
: FIND-GUESS-WORD  ( w -- w' true | false)  GUESS-WORDS #GUESS-WORDS FIND-WORD ;


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

: expect-found ( w -- )  test
  dup test-words #test-words find-word
  if 2drop else fail ." Expected to find " W. then ;

: expect-not-found ( w -- )  test
  dup test-words #test-words find-word
  if fail ." Expected not to find " drop W. else drop then ;

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
