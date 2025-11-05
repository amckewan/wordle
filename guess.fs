( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )  working @ ;

\ Pick a random word from the working set
: random-guess ( -- w )  working @  remaining random 0 ?do next loop ;

\ Use different algorithms
' simple-guess value guesser
: use  ' to guesser ;

: guess ( -- w )
    remaining 1 =  if ( only one left ) simple-guess exit then
    #greens len =  if ( we know it )    greens       exit then
    endgame-guess? if ( endgame guess )              exit then
    guesser execute ;
