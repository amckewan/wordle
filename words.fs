( Wordle words )

\ Wordle words are 5 characters long.
\ When passing strings, we only need the address. This is 'w' in the stack diagrams.
5 CONSTANT LEN

: W! ( w1 w2 -- )  LEN CMOVE ;
: W, ( w -- )  HERE LEN ALLOT W! ;
: W. ( w -- )  LEN TYPE SPACE ;

( literals )
: W   ( "w" -- w )               BL PARSE  DROP ;
: [W] ( "w" -- w [at runtime] )  BL PARSE  POSTPONE SLITERAL  POSTPONE DROP ; IMMEDIATE

: WCOMPARE ( w1 w2 -- -1/0/1 )  LEN SWAP LEN COMPARE ;
: W= ( w1 w2 -- f )  WCOMPARE 0= ;

\ Fetch and store the letter at pos
: L@ ( pos w -- c )  + C@ ;
: L! ( c pos w -- )  + C! ;

\ There are two lists of words, those that can be solutions (wordle words)
\ and those that can be guesses but not solutions.
: WW, ( "w" -- )  W W, ;

CREATE WORDLE-WORDS
INCLUDE wordle-words.fs
HERE
INCLUDE guess-words.fs
HERE

WORDLE-WORDS - LEN / CONSTANT #GUESS-WORDS
WORDLE-WORDS - LEN / CONSTANT #WORDLE-WORDS

\ get wordle word from word #
: WW ( w# -- w )  LEN * WORDLE-WORDS + ;

\ print all possible solutions
: .SOLUTIONS  #WORDLE-WORDS 0 DO  I WW W.  LOOP ;

\ check if a guess is valid, in one of the two word lists
: VALID-GUESS ( w -- f )
    #GUESS-WORDS 0 DO
      DUP I WW W= IF  DROP TRUE  UNLOOP EXIT  THEN
    LOOP  DROP FALSE ;


( === unit tests === )
TESTING VALID-GUESS

( wordle words )
T{ w ABACK valid-guess -> true }T
T{ w RAISE valid-guess -> true }T
T{ w ZONAL valid-guess -> true }T
( guess words)
T{ w ABLOW valid-guess -> true }T
T{ w PONGO valid-guess -> true }T
T{ w ZYMIC valid-guess -> true }T
( invalid words )
T{ w XXXXX valid-guess -> false }T
T{ w ABACC valid-guess -> false }T
