( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

\ The letters that could satisfy the remaining positions (a = 1)
create possibles  32 2* allot ( two copies )
: >possible ( c -- a )  31 and possibles + ;

\ 5 rows of 32 1-byte counts
create pos2  32 len * allot
: >pos2 ( c pos -- a )  32 * pos2 +  swap 31 and + ;

: .letter ( n c -- )  emit ."  (" 0 .r ." ) " ;
: .possibles
    len 0 do cr i 0 .r ." : " a-z do
        i j >pos2 c@ ?dup if i .letter then loop loop
    cr ." @: " a-z do i >possible c@ ?dup if i .letter then loop ;

\ mark all the possible missing letters for words in the working set
\ but don't mark the grey letters in the latest guess (we know they're wrong)
: c++ ( a -- ) dup c@ 1+ swap c! ;
: mark-pos ( pos -- )
    dup latest drop ww + c@ >r ( the grey letter )
    working @ begin
      2dup ww + c@  dup r@ - if dup 3 pick >pos2 c++  >possible c++ else drop then
    next? until r> 2drop ;
: mark-possibles ( -- )
    possibles 32 erase  pos2 32 len * erase
    greens  len 0 do  count '-' = if i mark-pos then  loop drop ;

\ calculate the value of a word, higher numbers mean more of the missing
\ letters are covered
\ add up the values for each position
\ - a possible letter but in the wrong position, value n
\ - a possible letter in the right position, value n+1
\ - a letter but in a green-scoring position, ignore it (no info)

\  : pos-value ( c pos -- n )
\      2dup greens + c@ - if

\      then 2drop 0 ;
\  : word-value ( w -- n )  0 swap ww
\      len 0 do  count i pos-value  rot + swap  loop drop ;

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
    working @ begin  dup >r  #possibles
        2dup < if  nip nip r@ swap  else  drop  then  r>
    next? until drop ;

: endgame-guess ( -- w )  mark-possibles
    guesses 5 < if find-most-all else find-most-working then ;

variable endgame ( 0=off, 3,4 greens, -1=4)
: endgame? ( -- w true | false )
    endgame @ 0= if false exit then
    guesses 2 5 within not if false exit then
    #guesses guesses - remaining >= if ( enough guesses left ) false exit then
    #greens endgame @ 7 and 4 min < if ( not yet ) false exit then
    \  cr ." endgame " guesses . greens len type space
    endgame-guess true ;

\ old endgame that solves all with hidden on (for comparison testing)
: endgame? ( -- w true | false )
    endgame @ 0= if false exit then
    #guesses guesses - remaining >= if ( enough guesses left ) false exit then
    #greens 3 < if false exit then
    endgame-guess true ;
