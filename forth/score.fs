( score a word )

\ Each letter can score grey, yellow or greeen, giving 3^5 possible scores.
\ Scores range from 0 (all grey) to 242 (all green), denoted by 's'.
\ We use '-' 'Y' and 'G' for input and display.

3 3 3 3 3 * * * * constant #scores

0 constant grey         
1 constant yellow
2 constant green

\ literals
: color ( char -- color )  2 rshift  3 and  3 - negate ( ascii tricks ) ;
: >s ( a -- s )  0 1  len 0 do >r  swap count color  r@ * rot +  r> 3 * loop drop nip ;
: s   ( "w" -- s )  bl parse ?len >s ;
: [s] ( "w" -- s )  s postpone literal ; immediate

: s. ( s -- ) len 0 do  3 /mod swap S" -YG" drop + c@ emit  loop drop space ;

\ scoring...
2 cells ( guess,target ) len * constant #scoring
create scoring  #scoring allot
scoring #scoring bounds 2constant for-scoring

: init-scoring ( target guess -- )  ww swap ww
    for-scoring do  swap count rot count rot i 2!  2 cells +loop  2drop ;

: score-greens ( -- score )   0 ( score ) 1 ( mult )
    for-scoring do
        i 2@ = if ( match )
            dup 2* rot + swap ( update score )  -1 -2 i 2! ( mark used )
        then  3 * ( mult )
    2 cells +loop  drop ;

: score-yellow ( score mult scoringentry -- score' mult )   dup @
    scoring cell+ #scoring bounds do
        dup i @ = if ( found match ) 2drop
            dup rot + swap ( update score )  -1 i ! ( mark used )
            unloop exit
        then
    2 cells +loop  2drop ;

: score-yellows ( score -- score' )   1 ( mult )
    for-scoring do
        i @ 0> if ( unused, look for matching letter in target )
            i score-yellow
        then  3 * ( mult )
    2 cells +loop  drop ;

: score ( target guess -- score )
    init-scoring  score-greens  score-yellows ;
