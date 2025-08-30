( save history of guesses and scores )

6 constant #guesses ( and thus will ever be )

\ The history has 6 entries of guess+score
create history  len 2* #guesses * allot

variable guesses

: clear-history  0 guesses ! ;

: >guess ( n -- w )  len 2* * history + ;
: >score ( n -- w )  >guess len + ;

: +history ( guess score -- )
    guesses @ #guesses u< not abort" History full"
    guesses @ >score w!
    guesses @ >guess w!
    1 guesses +! ;

: save-guess  guess score +history ; \ save most recent guess & score

: .history ( -- )
    guesses @ 0 ?do
        cr i 1+ .   i >guess w.
        cr 2 spaces i >score w.
    loop ;

