( save history of guesses and scores )

6 constant #guesses ( and thus will ever be )
0 value guesses

: solved ( -- f )  guess secret = ;
: failed ( -- f )  guesses #guesses >= ; ( assuming not solved )

\ The history has 6 entries of guess+score
create histbuf  #guesses 2* cells allot
: history ( n -- a )  2* cells histbuf + ;

: clear-history  0 to guesses ;

: .history ( -- )
    guesses 0 ?do  i history 2@  cr i 1+ . w.  cr 2 spaces w.  loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    swap guesses history 2!  guesses 1+ to guesses ;

\ Add the most recent guess & score to the history
: add-history ( -- )  guess score +history ;


TESTING HISTORY
T{ clear-history guesses -> 0 }T
T{ w RAISE w GG-YY +history guesses -> 1 }T
T{ w COUNT w YY-GG +history guesses -> 2 }T
T{ 0 history @ -> w RAISE }T
T{ 1 history cell+ @ -> w YY-GG }T
T{ add-history -> }T
