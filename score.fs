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
: s   ( "w" -- s )  bl parse  len - abort" need 5 letters"  >s ;
: [s] ( "w" -- s )  s  postpone literal ; immediate

: s. ( s -- ) len 0 do  3 /mod swap S" -YG" drop + c@ emit  loop drop space ;


\ scoring...
2 cells ( guess,target ) len * constant #scoring
create scoring  #scoring allot
scoring #scoring bounds 2constant for-scoring

: init-scoring ( target guess -- )
    for-scoring do  count rot count rot i 2! swap  2 cells +loop  2drop ;

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



( ===== TESTS ===== )
TESTING COLOR
T{ '-' color -> grey }T
T{ 'Y' color -> yellow }T
T{ 'G' color -> green }T
T{ 'y' color -> yellow }T
T{ 'g' color -> green }T

TESTING >S
T{ S" -----" drop >s -> 0 }T
T{ S" Y----" drop >s -> yellow }T
T{ S" G----" drop >s -> green }T
T{ S" Y--YY" drop >s -> yellow dup 27 * over 81 * + + }T
T{ S" -G--G" drop >s -> green dup 3 * swap 81 * + }T
T{ S" GGGGG" drop >s -> #scores 1- }T

TESTING S [S]
T{ s ----- -> 0 }T
T{ s Y---- -> yellow }T
T{ s G---- -> green }T
T{ s YG-Y- -> yellow  green 3 * +  yellow 27 * + }T
T{ s YYYYY -> yellow dup 3 * dup 3 * dup 3 * dup 3 * + + + + }T
T{ s GGGGG -> #scores 1- }T
T{ :noname [s] YG-Y- ; execute -> s YG-Y- }T

TESTING INIT-SCORING
T{ w ABCDE w MNOPQ init-scoring -> }T
T{ scoring           2@ -> 'A' 'M' }T
T{ scoring 2 cells + 2@ -> 'B' 'N' }T
T{ scoring 8 cells + 2@ -> 'E' 'Q' }T

TESTING SCORE-GREENS
T{ W TRACE W xxxxx init-scoring  score-greens -> s ----- }T
T{ W TRACE W TRACE init-scoring  score-greens -> s GGGGG }T
T{ W TRACE W TRAxx init-scoring  score-greens -> s GGG-- }T
T{ W TRACE W xRxCx init-scoring  score-greens -> s -G-G- }T

TESTING SCORE-YELLOWS
T{ w AABCD w xxxxx init-scoring  0 score-yellows -> s ----- }T
T{ w AABCD w Bxxxx init-scoring  0 score-yellows -> s Y---- }T
T{ w AABCD w xxAxx init-scoring  0 score-yellows -> s --Y-- }T
T{ w AABCD w xxAAx init-scoring  0 score-yellows -> s --YY- }T
T{ w AABCD w xxAAA init-scoring  0 score-yellows -> s --YY- }T
T{ w AABCD w DDxxx init-scoring  0 score-yellows -> s Y---- }T
T{ w ALERT w RAISE init-scoring  0 score-yellows -> s YY--Y }T
T{ w AABCD w AxAxA init-scoring  score-greens score-yellows -> s G-Y-- }T

TESTING SCORE
T{ w AABCD w xxxxx score -> s ----- }T
T{ w AABCD w Axxxx score -> s G---- }T
T{ w AABCD w Dxxxx score -> s Y---- }T
T{ w AABCD w DDDDx score -> s Y---- }T
T{ w AABCD w xxAxx score -> s --Y-- }T
T{ w AABCD w xxAAx score -> s --YY- }T
T{ w AABCD w xxAAA score -> s --YY- }T
T{ w AABCD w AxBDx score -> s G-GY- }T
T{ w AABCD w AxAxA score -> s G-Y-- }T
T{ w VIXEN w EERIE score -> s Y--Y- }T
