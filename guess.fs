( take a guess )

\ Pick the first word from the working set
: simple-guesser ( -- w )
    0  #words 0 do  i has if drop i leave then  loop  ww ;

\ Pick a random word from the working set
: random-guesser ( -- w )
    #working random  #words 0 do
      i has if  1- dup 0< if drop i leave then  then
    loop ww ;

\ Try different algorithms
' simple-guesser value guesser
: use  ' to guesser ;

\ Make a guess using the chosen algorithm
: make-guess ( -- w )  guesser execute ;
