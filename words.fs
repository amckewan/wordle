( Wordle words )

\ Wordle words are 5 characters long. When passing strings, we only need
\ the address. This is 'w' in the stack diagrams.

5 constant len

: wordle    create len allot ;

: upper     bounds do i c@ dup 'a' < -33 or and i c! loop ;
: w         bl parse len - abort" need 5 letters" dup len upper ;
: [w]       w len postpone sliteral postpone drop ; immediate
: w.        len type space ;

: wmove     len move ;
: wcompare  len swap len compare ;
: w=        wcompare 0= ;
: w,        here len dup allot move ;
: ww,       w w, ;

: for-chars ( w -- limit index )  dup len + swap ; \ for do..loop over chars
\ for-each-letter ?

\ =========================================================================
\ There are two lists of words, those that can be solutions (wordle-words)
\ and those that can be guesses but not solutions (guess-words).
\ The lists are disjoint and we lay them down one after the other.

create wordle-words
include data/wordle-words.fs
here
include data/guess-words.fs
here

wordle-words - len / constant #guess-words
wordle-words - len / constant #words        ( just the possible solutions )

\ get wordle word from word #
: ww ( w# -- w )  len * wordle-words + ;

\ print all possible solutions
: .wordles  #words 0 do  i ww w.  loop ;

\ check if a guess is in one of the two word lists
: valid-guess ( w -- f )
    #guess-words 0 do
        dup i ww w= if  drop true  unloop exit  then
    loop  drop false ;



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