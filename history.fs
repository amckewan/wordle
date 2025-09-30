( save history of guesses and scores )

\ The history has 6 entries of score+guess (6 bytes)
create histbuf  #guesses len 1+ * allot

: >hist ( n -- a )  6 * histbuf + ;
: hist@ ( n -- guess score )  >hist count ;
: hist! ( guess score n -- )  >hist  swap over c!  1+ wmove ;

: .history  guesses 0 ?do i hist@ swap  cr i 1+ . w.  cr 2 spaces s. loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses hist!  guesses 1+ to guesses ;

: latest ( -- guess score )  guesses dup 0= abort" no history" 1- hist@ ;



( ===== TESTS ===== )
TESTING HISTORY
0 to guesses
T{ w RAISE s GG-YY +history guesses -> 1 }T
T{ w COUNT s YY-GG +history guesses -> 2 }T
T{ 0 >hist 1+ w RAISE w= -> true }T
T{ 1 >hist c@ -> s YY-GG }T
T{ 0 hist@ drop w RAISE w= -> true }T
T{ 1 hist@ nip -> s YY-GG }T
