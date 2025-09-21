( end-game strategies )

\ Before every round we get a chance to enter the endgame where we can
\ use special strategies to finish the game. Here we handle the situation
\ where there are 4 greens, but we don't have enough guesses left to try
\ all possibilities.

: endgame? ( -- f )  greens 4 =   #guesses guesses - #working < and ;

\ Find the letters that could satisfy the remaining position
create possibles 32 allot
: unknown ( -- pos ) 0 begin answer over get while 1+ repeat ;
: find-letters ( from working set )  possibles 32 erase  unknown
    #words 0 do i has if  i ww over get possibles +  1 swap c!  then loop
    guess swap get possibles +  0 swap c! ( clear mismatching letter ) ;

\ : .possibles  a-z do i possibles + c@ if i l>c emit space then loop ;

: #letters ( w -- n ) \ how many of the possible letters in this word
    possibles pad 32 move ( make a copy, we change it )
    0 len 0 do ( w n )
        over i get pad +  dup c@ if 0 swap c! 1+ else drop then
    loop nip ;

\ Find the word that has the most of those letters
: endgame-guess ( -- w )
    find-letters  0 0 ( w# n )  #words 0 do
        i ww #letters  2dup < if  nip nip i swap  else  drop  then
    loop drop ww ;


TESTING ENDGAME
T{ w -AAAA to answer unknown -> 0 }T
T{ w AA-AA to answer unknown -> 2 }T
T{ w AAAA- to answer unknown -> 4 }T
