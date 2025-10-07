( save history of guesses and scores )

6 constant #guesses     ( set by game )

0 value guesses         ( number of guesses so far, 0-6 )

\ The history has 6 entries of score+guess (6 bytes)
create history  #guesses 6 * allot

: >hist ( n -- a )  6 * history + ;
: hist@ ( n -- guess score )  >hist count ;
: hist! ( guess score n -- )  >hist swap over c!  1+ wmove ;

: .history  guesses 0 ?do i hist@ swap  cr i 1+ . w.  cr 2 spaces s. loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses hist!  guesses 1+ to guesses ;

: latest ( -- guess score )  guesses 1- hist@ ;



( ===== TESTS ===== )
testing history
0 to guesses
t{ w raise s gg-yy +history guesses -> 1 }t
t{ w count s yy-gg +history guesses -> 2 }t
t{ 0 >hist 1+ w raise w= -> true }t
t{ 1 >hist c@ -> s yy-gg }t
t{ 0 hist@ drop w raise w= -> true }t
t{ 1 hist@ nip -> s yy-gg }t
