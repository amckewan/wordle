( make a guess )

\ A copy of the working set that we whittle down to pick a guess
create guessing  #words allot

: start-guess  working guessing #words cmove ; ( copy working set )

: guess? ( n -- f )  guessing + c@ ;
: -guess ( n -- )    guessing + 0 swap c! ;

: #guessing ( -- n )  0 #words 0 do i guess? + loop ;
: .guessing  0  #words 0 do i guess? if i ww w. 1+ then loop  . ." words " ;

\ find the most popular letter at a position
create letters  26 cells allot ( count for each letter )
char A constant A
: >letter ( n -- a )  cells letters + ;
: .letters  26 0 do  i A + emit ." ="  i >letter ?  loop ;

: count-letters ( pos -- )  letters 26 cells erase
    #words 0 do  i guess? if  1 over i ww + c@ A - >letter +!  then loop drop ;
: find-max ( -- n )  0 ( max )
    26 0 do  i >letter @ over >letter @ > if  drop i  then loop ;

: trim-guesses ( pos -- )
    dup count-letters find-max A +
    #words 0 do  i guess? if
        ( pos c ) over i ww + c@  over <> if i -guess then
    then loop 2drop ;

: trim ( -- )  len 0 do  i trim-guesses  loop ;


: t all-words start-guess ;






\ pick a random word from the working set
: random-guess ( -- w )
    #working random  #words 0 do
      i has if  1- dup 0< if drop i leave  then then
    loop  ww ;


: make-guess ( -- w )  start-guess random-guess ;
