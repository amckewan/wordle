( save history of guesses and scores )

\ The history has 6 entries of score+guess
create histbuf  #guesses 2* cells allot

: history ( n -- a )  2* cells histbuf + ;

: .history  guesses 0 ?do i history 2@ swap  cr i 1+ . w.  cr 2 spaces s. loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses history 2!  guesses 1+ to guesses ;

\ Submit a guess to the game, add to history, return the score
: make-guess ( guess -- score )
    secret over score-guess  swap over 2dup +history +answer ;



( ===== TESTS ===== )

TESTING HISTORY
0 to guesses
T{ w RAISE s GG-YY +history guesses -> 1 }T
T{ w COUNT s YY-GG +history guesses -> 2 }T
T{ 0 history cell+ @ -> w RAISE }T
T{ 1 history @ -> s YY-GG }T

TESTING MAKE-GUESS
init-game w AABBC to secret
T{ w DEFGH make-guess -> s ----- }T
T{ w AxBxC make-guess -> s G-G-G }T
T{ w AABBC make-guess -> s GGGGG }T
T{ guesses -> 3 }T
T{ w BBAAC make-guess -> s YYYYG }T
T{ w ABABA make-guess -> s GYYG- }T
T{ w CCAAB make-guess -> s Y-YYY }T
T{ guesses -> 6 }T
