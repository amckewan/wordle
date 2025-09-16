( List of Wordle words )

\ There are two lists of words, those that can be solutions (wordle words)
\ and those that can be guesses but not solutions.

: W, ( "w" -- )  W , ;

CREATE WORDLE-WORDS
INCLUDE wordle-words.fs
HERE
INCLUDE guess-words.fs
HERE

WORDLE-WORDS - 1 CELLS / CONSTANT #GUESS-WORDS
WORDLE-WORDS - 1 CELLS / CONSTANT #WORDLE-WORDS

\ get wordle word from word #
: WW ( w# -- w )  CELLS WORDLE-WORDS + @ ;

\ print all possible solutions
: .SOLUTIONS  #WORDLE-WORDS 0 DO  I WW W.  LOOP ;

\ check if a guess is valid, in one of the two word lists
: VALID-GUESS ( w -- f )
    WORDLE-WORDS #GUESS-WORDS CELLS BOUNDS DO
      DUP I @ = IF  0<> UNLOOP EXIT  THEN
    CELL +LOOP  DROP FALSE ;


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

