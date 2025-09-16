( Stop the guessing game )

\ After pruning...

\ Sometimes we get 3 or 4 greens that can be filled in various ways,
\ but we don't have enough guesses left to try them all.

\ Preconditions: 4 guesses, 4 greens, > 2 possible answers
\ Strategy: use guess 5 to eliminate as many of the candidates as possible

: endgame? ( -- f )  greens 5 = if true exit then
    greens 4 =  #guesses guesses - #working < and  guesses 5 < and ;

\ Collect the letters that could satisfy the remaining position
: unknown ( -- pos ) 0 begin answer over get while 1+ repeat ;
: find-letters ( from working set )  clear-tallies  unknown
    #words 0 do i has if  i ww over get 0 >tally  1 swap !  then loop
    guess swap get 0 >tally  0 swap ! ( clear mismatching letter ) ;

: #letters ( w -- n ) \ how many matches to letters
    tallies pad 32 cells move ( work on a copy )
    0 len 0 do ( w n )
        over i get cells pad +  dup @ if 0 swap ! 1+ else drop then
    loop nip ;

\ Find the word that has the most of those letters
: endgame-guess ( -- w )
    greens 5 = if answer exit then
    find-letters  0 ww 0 ( w n )
    #words 0 do
        i ww #letters 2dup < if nip nip i ww swap else drop then
    loop drop ;

\ 3 greens
: endgame2? ( -- f )
    greens 3 =  #guesses guesses - #working < and  guesses 5 < and ;

: endgame2-guess ( -- w )
    find-letters  0 ww 0 ( w n )
    #words 0 do
        i ww #letters 2dup < if nip nip i ww swap else drop then
    loop drop ;
