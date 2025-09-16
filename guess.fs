( make a guess )

\ A copy of the working set that we whittle down to pick a guess
create guessing  #words allot   guessing #words 1 fill ( for test )

\ Initialize for each round of guessing
: start-guess  working guessing #words cmove ( copy working set ) ;

: guess? ( n -- f )  guessing + c@ ;
: -guess ( n -- )    guessing + 0 swap c! ;

: #guessing ( -- n )  0 #words 0 do i guess? + loop ;
: .guessing  0  #words 0 do i guess? if i ww w. 1+ then loop  . ." words " ;

\ FIXED-GUESS: pick the first word (for repeatable tests)
: fixed-guess ( -- w )
    0 #words 0 do i guess? if drop i leave then loop ww ;

\ RANDOM-GUESS: pick a random word from the guessing set
: random-guess ( -- w )
    #guessing random  #words 0 do
      i guess? if  1- dup 0< if drop i leave  then then
    loop  ww ;

\ Try different algorithms
' random-guess value guesser
: use  ' to guesser ;

\ Make a guess using the chosen algorithm
: make-guess ( -- w )  start-guess  guesser execute ;
