( save history of guesses and scores )

6 constant #guesses     ( set by game )

0 value guesses         ( number of guesses so far, 0-6 )

\ The history has 6 entries of score+guess
create histbuf  #guesses 2 cells * allot

: history ( n -- a )  2 cells * histbuf + ;

: .history  guesses 0 ?do i history 2@ swap  cr i 1+ . w.  cr 2 spaces s. loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses history 2!  guesses 1+ to guesses ;

: latest ( -- guess score )  guesses 1- history 2@ ;



( ===== TESTS ===== )
testing history
0 to guesses
t{ w raise s gg-yy +history guesses -> 1 }t
t{ w count s yy-gg +history guesses -> 2 }t
t{ 0 history 2@ -> w raise s gg-yy }t
t{ 1 history 2@ -> w count s yy-gg }t
