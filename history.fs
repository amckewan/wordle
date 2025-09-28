( save history of guesses and scores )

\ The history has 6 entries of score+guess
create histbuf  #guesses len 1+ * allot

: >hist ( n -- a )  len 1+ * histbuf + ;
: hist@ ( n -- guess score )  >hist count ;
: hist! ( guess score n -- )  >hist  swap over c!  1+ wmove ;

: .history  guesses 0 ?do i hist@ swap  cr i 1+ . w.  cr 2 spaces s. loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses hist!  guesses 1+ to guesses ;

: latest ( -- guess score )  guesses 1- hist@ ;

\ Submit a guess to the game, update game state and return the score
: make-guess ( guess -- score )
    secret over score  swap over 2dup +history +answer ;



( ===== TESTS ===== )

TESTING HISTORY
0 to guesses
T{ w RAISE s GG-YY +history guesses -> 1 }T
T{ w COUNT s YY-GG +history guesses -> 2 }T
T{ 0 >hist 1+ w RAISE w= -> true }T
T{ 1 >hist c@ -> s YY-GG }T
T{ 0 hist@ drop w RAISE w= -> true }T
T{ 1 hist@ nip -> s YY-GG }T

TESTING MAKE-GUESS
init-game w ABACK secret wmove
T{ w DEFGH make-guess -> s ----- }T
T{ w AxAxK make-guess -> s G-G-G }T
T{ w ABACK make-guess -> s GGGGG }T
T{ guesses -> 3 }T
T{ w BABAA make-guess -> s YY-Y- }T
T{ w AKBCB make-guess -> s GYYG- }T
T{ w CCBAA make-guess -> s Y-YYY }T
T{ guesses -> 6 }T

TESTING +ANSWER
init-game
T{ w ABCDE s GY-G- +answer  w A--D- answer w= -> true }T
T{ w LMNOP s -G--- +answer  w AM-D- answer w= -> true }T
T{ w VWXYZ s ----- +answer  w AM-D- answer w= -> true }T

TESTING GREENS
T{ w ----- answer wmove     greens -> 0 }T
T{ w -A-B- answer wmove     greens -> 2 }T
T{ w WINDY answer wmove     greens -> 5 }T
