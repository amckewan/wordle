( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

: endgame? ( -- f )
    guesses 3 5 within   #greens 2 > and  ; \ #guesses guesses - remaining-hidden < and ;

\ The letters that could satisfy the remaining positions (two copies)
create possibles  32 2* allot ( a = 1 )
: >possible ( c -- a )  31 and possibles + ;

\ mark all the possible letters at pos for words in the working set
\ but don't mark the letter in the latest guess (we know it's wrong)
: c++ ( a n -- ) over c@ + swap c! ;
: mark-letters ( pos -- )
    dup latest drop + c@ >r ( the grey letter )
    working @ begin
      2dup cell+ + c@  dup r@ - if >possible 1 swap c! else drop then
    @ ?dup 0= until r> 2drop ;

: mark-greens ( -- )  possibles 32 erase
    greens  len 0 do  count '-' = if i mark-letters then  loop drop ;

\ Find a word from the full list that has the most of the remaining letters
: #possibles ( w -- n ) \ how many of the possible letters in this word
    possibles dup 32 + 32 move ( make a copy, we change it )
    0 swap len bounds do ( n )
        i c@ >possible 32 + ( second copy )
        dup c@ if  0 swap c! ( avoid double count ) 1+  else  drop  then
    loop ;
: find-most ( -- w )  0 ww ( w )  0 ( #letters )
    for-all-words do
        i #possibles  2dup < if  nip nip i swap  else  drop  then
    wsize +loop drop ;


: endgame-guess ( -- w )  mark-greens  find-most ;

