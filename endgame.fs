( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

\  tight failed
\  1 aback 
\    ----- 
\  2 defer 
\    ----- 
\  3 ghost 
\    YY--G 
\  4 light - up to here ok
\    -GGGG 
\  5 �w%H� 
\    ----- 
\  6 might 
\    -GGGG 

: endgame? ( -- f )
    guesses 5 <   greens 4 = and   #guesses guesses - remaining-hidden < and ;

\ Find the letters that could satisfy the remaining position (two copies)
create possibles 32 2* allot ( a = 1 )
: >possible ( c -- a )  31 and possibles + ;
: unknown ( a -- pos )  dup begin count '-' = until 1- swap - ;
t{ w -aaaa unknown -> 0 }t
t{ w aa-aa unknown -> 2 }t
t{ w aaaa- unknown -> 4 }t

: find-letters ( -- )  ( from working @ set )
    possibles 32 erase  answer unknown ( pos )
    working @ begin  2dup + cell+ c@ >possible   1 swap c!   @ ?dup 0= until
    latest drop + c@ >possible  0 swap c! ( clear mismatching letter ) ;

: #letters ( w -- n ) \ how many of the possible letters in this word
    possibles dup 32 + 32 move ( make a copy, we change it )  0 ( n ) swap
    for-chars do
        i c@ 31 and possibles 32 + +
        dup c@ if  0 swap c! ( only count once ) 1+  else  drop  then
    loop ;

\ Find a word from the full list that has the most of the remaining letters
: endgame-guess ( -- w )
    find-letters  0 ww ( w )  0 ( #letters )
    for-all-words do
        i #letters  2dup < if  nip nip i swap  else  drop  then
    wsize +loop drop ;

