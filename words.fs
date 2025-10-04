( Wordle words )

\ Wordle words are 5 characters long. When passing strings, we only need
\ the address. This is 'w' in the stack diagrams.

5 constant len

: wordle    create len allot ;

: lower     bounds do i c@ bl or i c! loop ;
: w         bl parse len - abort" need 5 letters" dup len lower ;
: [w]       w len postpone sliteral postpone drop ; immediate
: w.        len type space ;

: wmove     len move ;
: wcompare  len swap len compare ;
: w=        wcompare 0= ;
: w,        here len dup allot move ;

: for-chars ( w -- limit index )  dup len + swap ; \ for do..loop over chars
\ for-each-letter ?

\ =========================================================================
\ There are two lists of words, those that can be solutions (hidden-words)
\ and those that can be guesses but not solutions (guess-words).
\ The lists are disjoint and we lay them down one after the other.
\ Each list is sorted so we can do a binary search.

\ list entry has a cell used by the solver followed by the 5-letters word
len aligned cell+ constant wsize

\ compile flag + word
: ww, ( "w" -- )  0 ,  bl parse drop  here len dup allot move  align ;

create wordle-words
include data/hidden-words.fs
here
include data/guess-words.fs
here

dup                    constant words-end
wordle-words - wsize / constant #words    ( all words )

dup                    constant hidden-end
wordle-words - wsize / constant #hidden   ( just the possible solutions )

\ iterate through words ( i returns word address ), use `wsize +loop`
words-end  cell+ wordle-words cell+ 2constant for-all-words
hidden-end cell+ wordle-words cell+ 2constant for-hidden-words

\ get wordle word from word #
: ww ( w# -- w )  wsize * wordle-words + cell+ ;

\ print all possible solutions
: .hidden  for-hidden-words do i w. wsize +loop ;

\ check if a guess is in one of the two word lists (linear search)
: valid-guess ( w -- f )
    false  for-all-words do
        over i w= if drop true leave then
    wsize +loop nip ;



( ===== TESTS ===== )
TESTING VALID-GUESS
( hidden words )
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
( lowercase )
T{ w aback valid-guess -> true }T
T{ w RaisE valid-guess -> true }T
T{ w zonal valid-guess -> true }T