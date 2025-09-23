( save history of guesses and scores )

\ The history has 6 entries of guess+score
create histbuf  #guesses 2* cells allot

: history ( n -- a )  2* cells histbuf + ;

: .history  guesses 0 ?do  i history 2@  cr i 1+ . w.  cr 2 spaces w.  loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    swap guesses history 2!  guesses 1+ to guesses ;

\ Submit a guess to the game, add to history, return the score
: make-guess ( guess -- score )
    secret over score-guess  swap over 2dup +history +answer ;



TESTING HISTORY
0 to guesses
T{ w RAISE w GG-YY +history guesses -> 1 }T
T{ w COUNT w YY-GG +history guesses -> 2 }T
T{ 0 history @ -> w RAISE }T
T{ 1 history cell+ @ -> w YY-GG }T

TESTING MAKE-GUESS
init-game w AABBC to secret
T{ w DEFGH make-guess -> w ----- }T
T{ w AxBxC make-guess -> w G-G-G }T
T{ w AABBC make-guess -> w GGGGG }T
T{ guesses -> 3 }T
T{ w BBAAC make-guess -> w YYYYG }T
T{ w ABABA make-guess -> w GYYG- }T
T{ w CCAAB make-guess -> w Y-YYY }T
T{ guesses -> 6 }T
