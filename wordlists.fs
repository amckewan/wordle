( Wordle word lists )
\ There are two lists of words. The first list contains all words that are
\ allowed as guesses, currently about 14,000 words (ALL-WORDS).
\ The second list is a subset (SOLUTION-WORDS) that contains the possible
\ Wordle solutions.

: W, ( "w" -- )  W , ;

CREATE ALL-WORDS
INCLUDE data/wordlist_nyt20220830_all.fs
HERE ALL-WORDS - 1 CELLS / CONSTANT #ALL-WORDS

CREATE SOLUTION-WORDS
INCLUDE data/wordlist_nyt20220830_hidden.fs
HERE SOLUTION-WORDS - 1 CELLS / CONSTANT #SOLUTION-WORDS

\ For now we only use the hidden list (currently 2309 words).
 SOLUTION-WORDS CONSTANT WORDLE-WORDS
#SOLUTION-WORDS CONSTANT #WORDS

\ get wordle word from word # (only works on the solution list!)
: WW ( w# -- w )  CELLS WORDLE-WORDS + @ ;

\ print all possible solution words
: .SOLUTIONS  #WORDS 0 DO  I WW W.  LOOP ;

\ Check for a valid guess (in the all-words list)
: VALID-GUESS ( w -- f )
    ALL-WORDS #ALL-WORDS CELLS BOUNDS DO
      DUP I @ = IF  0<> UNLOOP EXIT  THEN
    CELL +LOOP  DROP FALSE ;


TESTING VALID-GUESS
( solution words )
T{ w ABACK VALID-GUESS -> true }T
T{ w RAISE VALID-GUESS -> true }T
T{ w ZONAL VALID-GUESS -> true }T
( obscure words )
T{ w SALET VALID-GUESS -> true }T
T{ w MILCH VALID-GUESS -> true }T
T{ w TELEX VALID-GUESS -> true }T
( invalid words )
T{ w XXXXX VALID-GUESS -> false }T
T{ w ABACC VALID-GUESS -> false }T
