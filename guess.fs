( make a guess )

\ A copy of the working set that we whittle down to pick a guess
create guessing  #words allot  guessing #words 1 fill ( sane default )

: start-guess  working guessing #words cmove ; ( copy working set )

: guess? ( n -- f )  guessing + c@ ;
: -guess ( n -- )    guessing + 0 swap c! ;

: #guessing ( -- n )  0 #words 0 do i guess? + loop ;
: .guessing  0  #words 0 do i guess? if i ww w. 1+ then loop  . ." words " ;

\ GUESS: pick the first guessing word
: first-guess ( --- w )
    0  #words 0 do  i guess? if  drop i leave  then loop  ww ;

\ GUESS: pick a random word from the guessing set
: random-guess ( -- w )
    #guessing random  #words 0 do
      i guess? if  1- dup 0< if drop i leave  then then
    loop  ww ;

\ number of occurances of each letter
create letters  26 cells allot
: >letter ( n -- a )  cells letters + ;
: .letters  26 0 do  i A + emit ." ="  i >letter ?  loop ;

\ GUESS: pick the word with the largest letter tally
: count-word ( w -- )  len bounds do  1 i c@ A - >letter +!  loop ;
: count-all ( -- )  letters 26 cells erase
    #words 0 do  i guess? if  i ww count-word  then loop ;
: tally ( w -- n )  0 swap  len bounds do  i c@ A - >letter @ +  loop ;
: tally-guess ( -- w )
    0 ww 0  #words 0 do  i guess? if
        i ww tally 2dup < if ( replace ) nip nip i ww swap else drop then
    then loop drop ;

\ GUESS: trim guesses a letter at a time
: count-letters ( pos -- )  letters 26 cells erase
    #words 0 do  i guess? if  1 over i ww + c@ A - >letter +!  then loop drop ;
: most-popular ( -- c )  0 ( max )
    26 0 do  i >letter @ over >letter @ > if  drop i  then loop  A + ;
: trim ( pos -- )  dup count-letters most-popular
    #words 0 do  i guess? if
        ( pos c ) over i ww + c@  over <> if i -guess then
    then loop 2drop ;
: trim-guess ( -- w )  len 0 do i trim loop  first-guess ;


\ try different algorithms
variable guesser
: make-guess ( -- w )  start-guess  guesser @ execute ;

' first-guess guesser !
\ ' random-guess guesser !
\ ' tally-guess guesser !
\ ' trim-guess guesser !

: t all-words start-guess ;

