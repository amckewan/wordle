( Wordle words )

\ There are two lists of words, those that can be solutions (hidden-words)
\ and those that can be guesses but not solutions (guess-words).
\ The lists are sorted and we lay them down one after the other.
\ We identify a word by its offset into the list (`w` in stack diagrams).

5 constant len          ( Wordle words are 5 characters long )

: ww, ( "aword" -- )  bl parse drop  here len  dup allot move ;

create wordle-words
include data/hidden-words.fs here
include data/guess-words.fs here

wordle-words - len / constant #words    ( total number of words )
wordle-words - len / constant #hidden   ( number hidden words )

: ww ( w -- a )  len * wordle-words + ;
: w. ( w -- )    ww len type space ;


\ word literals
: -find ( a -- w f | t )
    wordle-words #words len * bounds do
        dup len i len compare 0= if ( found )
            drop  i wordle-words - len / false  unloop exit
        then
    len +loop  drop true ;

: ?len ( n -- )  len - abort" Need 5 letters" ;
: w ( -- w )  bl parse ?len  -find abort" Not in word list" ;
: [w]  w  postpone literal ; immediate
