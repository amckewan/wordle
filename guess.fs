( make a guess )

\ A copy of the working set that we whittle down to pick a guess
create guessing  #words allot  guessing #words 1 fill ( sane default )

: start-guess  working guessing #words cmove ; ( copy working set )

: guess? ( n -- f )  guessing + c@ ;
: -guess ( n -- )    guessing + 0 swap c! ;

: #guessing ( -- n )  0 #words 0 do i guess? + loop ;
: .guessing  0  #words 0 do i guess? if i ww w. 1+ then loop  . ." words " ;

\ number of occurances of each letter
create letters  26 cells allot
char A constant A
: >letter ( n -- a )  cells letters + ;
: .letters  26 0 do  i A + emit ." ="  i >letter ?  loop ;

\ count the occurances of each letter in the guessing words
: count-word ( w -- )  len bounds do  1 i c@ A - >letter +!  loop ;
: count-all ( -- )  letters 26 cells erase
    #words 0 do  i guess? if  i ww count-word  then loop ;

\ find the guess with the largest letter count
: tally ( w -- n )  0 swap  len bounds do  i c@ A - >letter @ +  loop ;
: largest-tally ( -- w tally )  0 ww 0 ( w tally )
    #words 0 do  i guess? if
        i ww tally 2dup < if ( replace ) nip nip i ww swap else drop then
    then loop ;

: tally-guess ( -- w ) largest-tally drop ;

\ 
: count-letters ( pos -- )  letters 26 cells erase
    #words 0 do  i guess? if  1 over i ww + c@ A - >letter +!  then loop drop ;
: find-max ( -- n )  0 ( max )
    26 0 do  i >letter @ over >letter @ > if  drop i  then loop ;

: trim-guesses ( pos -- )
    dup count-letters find-max A +
    #words 0 do  i guess? if
        ( pos c ) over i ww + c@  over <> if i -guess then
    then loop 2drop ;

: trim ( -- )  len 0 do  i trim-guesses  #guessing .  loop ;


: t all-words start-guess ;



\ pick a random word from the guessing set
: random-guess ( -- w )
    #guessing random  #words 0 do
      i guess? if  1- dup 0< if drop i leave  then then
    loop  ww ;

: first-guess ( --- w )
    0  #words 0 do
      i guess? if  drop i leave  then
    loop  ww ;


: make-guess ( -- w )
    start-guess
\    trim
\    first-guess
\    random-guess
    tally-guess
    ;
