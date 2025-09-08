( save history of guesses and scores )

6 constant #guesses ( and thus will ever be )

\ The history has 6 entries of guess+score
create history  len 2* #guesses * allot

variable guesses

: clear-history  0 guesses ! ;

: >guess ( n -- w )  len 2* * history + ;
: >score ( n -- w )  >guess len + ;

: .history ( -- )
    guesses @ 0 ?do
        cr i 1+ .   i >guess w.
        cr 2 spaces i >score w.
    loop ;

: +history ( guess score -- )
    guesses @ #guesses u< not abort" History full"
    guesses @ >score w!
    guesses @ >guess w!
    1 guesses +! ;

\ Add the most recent guess & score to the history
: add-history ( -- )  guess score +history ;


TESTING HISTORY
T{ clear-history guesses @ -> 0 }T
T{ w RAISE w GG-YY +history guesses @ -> 1 }T
T{ w COUNT w YY-GG +history guesses @ -> 2 }T
T{ 0 >guess w RAISE wcompare -> 0 }T
T{ 1 >score w YY-GG wcompare -> 0 }T
