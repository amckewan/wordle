( List of Wordle words )

\ There are two lists of words, those that can be solutions (wordle words)
\ and those that can be guesses but not solutions.
\ Here we only use the first list (currently 2309 words).

: W, ( "w" -- )  W , ;

CREATE WORDLE-WORDS
INCLUDE wordle-words.fs
HERE WORDLE-WORDS - 1 CELLS / CONSTANT #WORDS

\ get wordle word from word #
: WW ( w# -- w )  CELLS WORDLE-WORDS + @ ;

\ print all the words
: .WORDS  #WORDS 0 DO  I WW W.  LOOP ;

\ check if a word is valid (in the list)
: VALID-WORD ( w -- f )
    WORDLE-WORDS #WORDS CELLS BOUNDS DO
      DUP I @ = IF  0<> UNLOOP EXIT  THEN
    CELL +LOOP  DROP FALSE ;



TESTING VALID-WORD
( wordle words )
T{ w ABACK VALID-WORD -> true }T
T{ w RAISE VALID-WORD -> true }T
T{ w ZONAL VALID-WORD -> true }T
( invalid words )
T{ w XXXXX VALID-WORD -> false }T
T{ w ABACC VALID-WORD -> false }T
