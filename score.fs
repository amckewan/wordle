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
testing color
t{ '-' color -> grey }t
t{ 'y' color -> yellow }t
t{ 'g' color -> green }t
t{ 'y' color -> yellow }t
t{ 'g' color -> green }t

testing >s
t{ s" -----" drop >s -> 0 }t
t{ s" y----" drop >s -> yellow }t
t{ s" g----" drop >s -> green }t
t{ s" y--yy" drop >s -> yellow dup 27 * over 81 * + + }t
t{ s" -g--g" drop >s -> green dup 3 * swap 81 * + }t
t{ s" ggggg" drop >s -> #scores 1- }t

testing s [s]
t{ s ----- -> 0 }t
t{ s y---- -> yellow }t
t{ s g---- -> green }t
t{ s yg-y- -> yellow  green 3 * +  yellow 27 * + }t
t{ s yyyyy -> yellow dup 3 * dup 3 * dup 3 * dup 3 * + + + + }t
t{ s ggggg -> #scores 1- }t
t{ :noname [s] yg-y- ; execute -> s yg-y- }t

testing init-scoring
t{ w thorn w bears init-scoring -> }t
t{ scoring           2@ -> 't' 'b' }t
t{ scoring 2 cells + 2@ -> 'h' 'e' }t
t{ scoring 8 cells + 2@ -> 'n' 's' }t

testing score-greens
t{ w trace w xxxxx init-scoring  score-greens -> s ----- }t
t{ w trace w trace init-scoring  score-greens -> s ggggg }t
t{ w trace w traxx init-scoring  score-greens -> s ggg-- }t
t{ w trace w xrxcx init-scoring  score-greens -> s -g-g- }t

testing score-yellows
t{ w aabcd w xxxxx init-scoring  0 score-yellows -> s ----- }t
t{ w aabcd w bxxxx init-scoring  0 score-yellows -> s y---- }t
t{ w aabcd w xxaxx init-scoring  0 score-yellows -> s --y-- }t
t{ w aabcd w xxaax init-scoring  0 score-yellows -> s --yy- }t
t{ w aabcd w xxaaa init-scoring  0 score-yellows -> s --yy- }t
t{ w aabcd w ddxxx init-scoring  0 score-yellows -> s y---- }t
t{ w alert w raise init-scoring  0 score-yellows -> s yy--y }t
t{ w aabcd w axaxa init-scoring  score-greens score-yellows -> s g-y-- }t

testing score
t{ w aabcd w xxxxx score -> s ----- }t
t{ w aabcd w axxxx score -> s g---- }t
t{ w aabcd w dxxxx score -> s y---- }t
t{ w aabcd w ddddx score -> s y---- }t
t{ w aabcd w xxaxx score -> s --y-- }t
t{ w aabcd w xxaax score -> s --yy- }t
t{ w aabcd w xxaaa score -> s --yy- }t
t{ w aabcd w axbdx score -> s g-gy- }t
t{ w aabcd w axaxa score -> s g-y-- }t
t{ w vixen w eerie score -> s y--y- }t
