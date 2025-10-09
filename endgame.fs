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
    dup latest drop + c@ >r ( the grey letter )
    working @ begin
      2dup cell+ + c@  dup r@ - if >possible c++ else drop then
    @ ?dup 0= until r> 2drop ;

: mark-greens ( -- )  possibles 32 erase
    greens  len 0 do  count '-' = if i mark-letters then  loop drop ;

: #possibles ( w -- n ) \ how many of the possible letters in this word
    possibles dup 32 + 32 move ( make a copy, we change it )
    0 swap len bounds do ( n )
        i c@ >possible 32 + ( second copy )
        dup c@ dup if  rot +  0 rot c! ( avoid double count ) else  2drop  then
    loop ;

\ Find a word from the full list that has the most of the remaining letters
: find-most-all ( -- w )  0 ww ( w )  0 ( #letters )
    for-all-words do
        i #possibles  2dup < if  nip nip i swap  else  drop  then
    wsize +loop drop ;

\ Just the working set for last guess
: find-most-working ( -- w )  working @ ( w' )  0 ( #letters )
    working @ begin  dup >r  cell+ #possibles
        2dup < if  nip nip r@ swap  else  drop  then  r>
    @ ?dup 0= until drop cell+ ;



: endgame-guess ( -- w )  mark-greens
    guesses 5 < if find-most-all else find-most-working then ;

: endgame? ( -- w true | false )
    #guesses guesses - remaining >= if ( enough guesses left ) false exit then
    #greens 3 < if false exit then
    endgame-guess true ;
