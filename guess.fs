( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )  working @ cell+ ;

\ Pick a random word from the hidden word list (otherwise it's terrible)
: random-guess ( -- w )  working remaining-hidden random -1 do @ loop cell+ ;

\ Try different algorithms
' simple-guess value guesser
: use  ' to guesser ;
