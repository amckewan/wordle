( Stop the guessing game )

\ After pruning...

\ Sometimes we get 3 or 4 greens that can be filled in various ways,
\ but we don't have enough guesses left to try them all.

\ Preconditions: 4 guesses, 4 greens, > 2 possible answers
\ Strategy: use guess 5 to eliminate as many of the candidates as possible

: endgame? ( -- f )
    greens @ 4 =  #guesses guesses @ - #working < and ;

\ Collect the letters that could satisfy the remaining position
: pos ( -- pos ) 0 begin dup answer l@ grey <> while 1+ repeat ;

: find-letters ( from working set )  clear-letters  pos
    #words 0 do i has if  1 over i ww l@ >letter !  then loop
    guess l@ >letter 0 swap ! ( clear mismatching letter ) ;

: #letters ( w -- n ) \ how many matches to letters
    letters pad 26 cells move
    0 swap len bounds do
        i c@ A - cells pad +  dup @ if 0 swap ! 1+ else drop then
    loop ;

\ Find the word that has the most of those letters
: endgame-guess ( -- w )
    find-letters  0 ww 0 ( w n )
    #words 0 do
        i ww #letters 2dup < if nip nip i ww swap else drop then
    loop drop ;
