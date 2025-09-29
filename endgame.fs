( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

: endgame? ( -- f )
    guesses 5 <   greens 4 = and   #guesses guesses - #working < and ;

\ Find the letters that could satisfy the remaining position
create possibles 32 allot
: >possible ( c -- a )  31 and possibles + ;
: unknown ( -- pos )  0 begin dup answer + c@ '-' - while 1+ repeat ;
: find-letters ( from working set )  possibles 32 erase  unknown ( pos )
    for-working do i c@ if  1 over i >ww + c@ >possible c!  then loop
    latest drop + c@ >possible  0 swap c! ( clear mismatching letter ) ;

: #letters ( w -- n ) \ how many of the possible letters in this word
    possibles pad 32 move ( make a copy, we change it )  0 ( n ) swap
    for-chars do
        i c@ 31 and pad +
        dup c@ if  0 swap c! ( only count once ) 1+  else  drop  then
    loop ;

\ Find a word from the full list that has the most of the remaining letters
wordle-words #guess-words len * bounds 2constant for-all-words
: endgame-guess ( -- w )
    find-letters  0 ww 0 ( w n )  for-all-words do
        i #letters  2dup < if  nip nip i swap  else  drop  then
    len +loop drop ;


TESTING ENDGAME
T{ w -AAAA answer wmove unknown -> 0 }T
T{ w AA-AA answer wmove unknown -> 2 }T
T{ w AAAA- answer wmove unknown -> 4 }T
