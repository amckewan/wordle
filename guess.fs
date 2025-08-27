( make a guess )

\ pick a random word from the working set
: random-guess ( -- ww )
    #working random  #words 0 do
        i has if  1- dup 0< if  drop i ww leave  then then
    loop ;

: make-guess ( -- w )  random-guess ;
