( score a word )

\ Each letter can score grey, yellow or greeen, giving 3^5 possible scores.
\ Scores range from 0 (all grey) to 242 (all green), denoted by 's'.

3 3 3 3 3 * * * * constant #scores

0 constant grey         
1 constant yellow
2 constant green

\ Literals, using "-YG"
: color ( char -- color )  2 rshift  3 and  3 - negate ( ascii tricks ) ;
: >s ( a -- s )  0 1  len 0 do >r  swap count color  r@ * rot +  r> 3 * loop drop nip ;
: s ( "w" -- s )  bl parse  len <> abort" need 5 letters"  >s ;
: [s] ( "w" -- s )  s  postpone literal ; immediate

: s. ( s -- ) len 0 do  3 /mod swap S" -YG" drop + c@ emit  loop drop space ;

\ Make copies of the target and guess since we are going to modify then
\ during scoring.
wordle target   \ the word we are scoring against (e.g. the secret answer)
wordle guess    \ the word to score

: init-score ( target guess -- )  guess wmove  target wmove ;
: match ( pos -- f )  dup guess + c@ swap target + c@ = ;
: mark-used ( tpos gpos -- )  1 swap guess + c!  2 swap target + c! ;

: score-greens ( -- score )   0 ( score ) 1 ( mult )
    len 0 do  i match if
        dup 2* rot + swap ( update score ) i i mark-used
    then 3 * loop drop ;

: score-yellows ( score -- score' )   1 ( mult )
    len 0 do  i guess + c@  dup bl > if
        len 0 do  dup i target + c@ = if  drop
            dup rot + swap ( update score ) i j mark-used
            dup leave
        then loop
    then drop 3 * loop drop ;

: score-guess ( target guess -- score )
    init-score  score-greens  score-yellows ;



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

TESTING SCORE-GREENS
T{ W TRACE W xxxxx init-score  score-greens -> s ----- }T
T{ W TRACE W TRACE init-score  score-greens -> s GGGGG }T
T{ W TRACE W TRAxx init-score  score-greens -> s GGG-- }T
T{ W TRACE W xRxCx init-score  score-greens -> s -G-G- }T

TESTING SCORE-YELLOWS
T{ w AABCD w xxxxx init-score  0 score-yellows -> s ----- }T
T{ w AABCD w Bxxxx init-score  0 score-yellows -> s Y---- }T
T{ w AABCD w xxAxx init-score  0 score-yellows -> s --Y-- }T
T{ w AABCD w xxAAx init-score  0 score-yellows -> s --YY- }T
T{ w AABCD w xxAAA init-score  0 score-yellows -> s --YY- }T
T{ w AABCD w DDxxx init-score  0 score-yellows -> s Y---- }T
T{ w ALERT w RAISE init-score  0 score-yellows -> s YY--Y }T
T{ w AABCD w AxAxA init-score  score-greens score-yellows -> s G-Y-- }T

TESTING SCORE-GUESS
T{ w AABCD w xxxxx score-guess -> s ----- }T
T{ w AABCD w Axxxx score-guess -> s G---- }T
T{ w AABCD w Dxxxx score-guess -> s Y---- }T
T{ w AABCD w DDDDx score-guess -> s Y---- }T
T{ w AABCD w xxAxx score-guess -> s --Y-- }T
T{ w AABCD w xxAAx score-guess -> s --YY- }T
T{ w AABCD w xxAAA score-guess -> s --YY- }T
T{ w AABCD w AxBDx score-guess -> s G-GY- }T
T{ w AABCD w AxAxA score-guess -> s G-Y-- }T
T{ w VIXEN w EERIE score-guess -> s Y--Y- }T
