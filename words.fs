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

\ There are two lists of words, those that can be solutions
\ and those that can be guesses but not solutions.

: WW,  W W, ;  \ add a word to the dictionary

HERE
INCLUDE wordle-words.fs
HERE
INCLUDE guess-words.fs
HERE

CONSTANT GUESS-END
CONSTANT WORDS-END
CONSTANT WORDLE-WORDS

WORDS-END WORDLE-WORDS - LEN / CONSTANT #WORDS

\ check if a guess is valid, in one of the two word lists
: VALID-GUESS ( w -- f )
    GUESS-END WORDLE-WORDS DO
      DUP I W= IF  DROP TRUE  UNLOOP EXIT  THEN
    LEN +LOOP  DROP FALSE ;


\ get wordle word from word #
: WW ( w# -- w )  LEN * WORDLE-WORDS + ;
: .WW ( w# -- )  WW W. ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;


( === unit tests === )
include unit-test.fs

: expect-valid ( w -- ) test
    dup valid-guess not if fail ." expect valid " W. then ;
: expect-not-valid ( w -- ) test
    dup valid-guess if fail ." expect not valid " W. then ;

: test-valid-guess
    cr ." Testing VALID-GUESS..." begin-unit-tests

    ( wordle words )
    [W] ABACK expect-valid
    [W] RAISE expect-valid
    [W] ZONAL expect-valid

    ( guess words )
    [W] ABLOW expect-valid
    [W] PONGO expect-valid
    [W] ZYMIC expect-valid

    ( invalid words )
    [W] XXXXX expect-not-valid
    [W] ABACC expect-not-valid

    report-unit-tests ;
  
test-valid-guess

forget-unit-tests
