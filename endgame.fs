( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

\ The letters that could satisfy the remaining positions (two copies)
create possibles  32 2* allot ( a = 1 )
: >possible ( c -- a )  31 and possibles + ;

: .possibles  a-z do i >possible c@ if i emit space then loop ;

\ mark all the possible letters at pos for words in the working set
\ but don't mark the letter in the latest guess (we know it's wrong)
: c++ ( a -- ) dup c@ 1+ swap c! ;
: mark-letters ( pos -- )
    dup latest drop ww + c@ >r ( the grey letter )
    working @ begin
      2dup >w ww + c@  dup r@ - if >possible c++ else drop then
    @ ?dup 0= until r> 2drop ;

: mark-greens ( -- )  possibles 32 erase
    greens  len 0 do  count '-' = if i mark-letters then  loop drop ;

: #possibles ( w -- n ) \ how many of the possible letters in this word
    possibles dup 32 + 32 move ( make a copy, we change it )
    0 ( n )  swap ww len bounds do
        i c@ >possible 32 + ( second copy )
        dup c@ dup if  rot +  0 rot c! ( avoid double count ) else  2drop  then
    loop ;

\ Find a word from the full list that has the most of the remaining letters
: find-most-all ( -- w )  0 ( w )  0 ( #letters )
    #words 0 do
        i #possibles  2dup < if  nip nip i swap  else  drop  then
    loop drop ;

\ Just use the working set for the last guess (no chance otherwise)
: find-most-working ( -- w )  working @ ( w )  0 ( #letters )
    working @ begin  dup >r  >w #possibles
        2dup < if  nip nip r@ swap  else  drop  then  r>
    @ ?dup 0= until drop >w ;

: endgame-guess ( -- w )  mark-greens
    guesses 5 < if find-most-all else find-most-working then ;

: endgame? ( -- w true | false )
    guesses 2 < if ( a bit early ) false exit then
    #guesses guesses - remaining >= if ( enough guesses left ) false exit then
    \ todo: special handling with 2 guesses left?
    #greens 3 < if ( can't help yet ) false exit then
    endgame-guess true ;
