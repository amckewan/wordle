( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )
    0 ww  for-working do i c@ if drop i >ww leave then loop ;

\ Pick a random word from the hidden word list (otherwise it's terrible)
: random-guess ( -- w )
    0 ww  #working remaining random  for-working do
        i c@ if  1- dup 0< if drop i >ww swap leave then  then
    loop drop ;

\ Try different algorithms
' simple-guess value guesser
: use  ' to guesser ;
