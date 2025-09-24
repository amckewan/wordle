( Wordle word lists )

\ There are two lists of words. The first list contains all words that are
\ allowed as guesses, currently about 14,000 words (WORDLE-WORDS).
\ The second list is a subset (HIDDEN-WORDS) that contains the possible
\ Wordle solutions.

: W, ( "w" -- )  W , ;

\ All Wordle words allowed as guesses
CREATE WORDLE-WORDS
INCLUDE data/wordlist_nyt20220830_all.fs
HERE WORDLE-WORDS - 1 CELLS / CONSTANT #WORDS

\ get wordle word from word #
: WW ( w# -- w )  CELLS WORDLE-WORDS + @ ;

\ Check for a valid guess word
: VALID-GUESS ( w -- f )
    WORDLE-WORDS #WORDS CELLS BOUNDS DO
      DUP I @ = IF  0<> UNLOOP EXIT  THEN
    CELL +LOOP  DROP FALSE ;

\ Wordle answers are always from the hidden list (a subset of all words)
CREATE HIDDEN-WORDS
INCLUDE data/wordlist_nyt20220830_hidden.fs
HERE HIDDEN-WORDS - 1 CELLS / CONSTANT #HIDDEN

: HIDDEN@ ( n -- w )  CELLS HIDDEN-WORDS + @ ;
: .HIDDEN  #HIDDEN 0 DO  I HIDDEN@ W.  LOOP ;



( ===== TESTS ===== )
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
