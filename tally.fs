( letter tallies )

variable tallypos  \ true to use individual positions

\ The number of occurances of each letter at each position
\ 5 rows of 32, count for 'A' at offset 1.
32 cells constant #tally
#tally len * constant #tallies
create tallies        #tallies allot

: >tally ( c pos -- a )  32 *  swap 31 and +  cells tallies + ;

tallies #tallies bounds 2constant for-tallies ( -- limit index )

: .tallies  len 0 do  cr i 0 .r ." : "  a-z do  i j >tally @
      ?dup if  i emit ." =" . then loop loop ;

: tally-word ( w -- )
    for-tallies do  count 31 and cells i +  1 swap +!  #tally +loop drop ;
: tally-working ( -- )  tallies #tallies erase
    working @ begin  dup cell+ tally-word  @ ?dup 0= until ;

: tally ( w -- n )    0 ( n ) swap
    for-tallies do  count 31 and cells i + @  rot + swap  #tally +loop drop ;
