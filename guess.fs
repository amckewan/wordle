( make a guess )

\ pick a random word from the working set
: random-guess ( -- w )
    #working random  #words 0 do
      i has if  1- dup 0< if drop i leave  then then
    loop  ww ;

: make-guess ( -- w )  random-guess ;
