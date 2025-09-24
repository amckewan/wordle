( score a word )

\ Each letter can score grey, yellow or greeen, giving 3^5 possible scores.
\ Scores range from 0 (all grey) to 242 (all green), denoted by 's'.

3 3 3 3 3 * * * * constant #scores

0 constant grey         
1 constant yellow
2 constant green

create score  len allot  \ working score for each letter
create used   len allot  \ set when a letter is used for green or yellow

: score@ ( -- s )  0  0 len 1- do  3 * i score + c@ +  -1 +loop ;

: score-greens ( secret guess -- ) \ sets all of score and used
    xor ( 0=green ) len 0 do
        dup i mask and 0= ( green? ) green and  dup i score + c!  i used + c!   
    loop drop ;

\ look in all positions for a candidate to score pos yellow
\ mark the position where we found the letter as used (avoid double-counting)
: score-yellow ( secret guess pos -- secret guess )
    len 0 do  i used + c@ 0= if ( unused )
        2dup get ( can do outside loop) 3 pick i get = if ( match @ secret )
            yellow   dup i used + c!   over score + c!   leave
        then
    then loop drop ;
: score-yellows ( secret guess -- )
    len 0 do  i score + c@ green < if i score-yellow then  loop 2drop ;

\ Score a guess against a secret, returning the score.
: score-guess ( secret guess -- score )
    2dup score-greens score-yellows score@ ;


\ literals
: >s ( a -- s )
    0 swap  dup len 1- + do
        3 *   3 i c@ 2 rshift 3 and - ( ascii tricks )  +
    -1 +loop ;
:  s  ( "w" -- s )  bl parse  len <> abort" need 5 letters"  >s ;
: [s] ( "w" -- s )  s  postpone literal ; immediate


\ Display score using -,Y,G
create schars  '-' c,  'Y' c,  'G' c,
: s. ( s -- )  len 0 do 3 /mod swap schars + c@ emit loop drop space ;

: .score ( for debug )
    ." score: " score@ s.
    ." used: " len 0 do i used + c@ if 'X' else '.' then emit loop space ;



( ===== TESTS ===== )

: -score  score len erase  used len erase ;

TESTING SCORE@
T{ -score                   score@ -> 0                     }T
T{ yellow 0 score + c!      score@ -> yellow                }T
T{ green  1 score + c!      score@ -> green 3 * yellow +    }T
T{ score len green fill     score@ -> #scores 1-            }T

TESTING >S
T{ S" -----" drop >s -> grey }T
T{ S" Y----" drop >s -> yellow }T
T{ S" G----" drop >s -> green }T
T{ S" YYYYY" drop >s -> 1 3 9 27 81 + + + + }T
T{ S" GGGGG" drop >s -> #scores 1- }T

TESTING S
T{ s ----- -> grey }T
T{ s Y---- -> yellow }T
T{ s G---- -> green }T
T{ s YYYYY -> 1 3 9 27 81 + + + + }T
T{ s GGGGG -> #scores 1- }T

TESTING SCORE-GREENS
T{ W ABCDE W ABCDE score-greens  score@ -> s GGGGG }T
T{ W ABACK W ABASE score-greens  score@ -> s GGG-- }T
T{ W ABACK W XXXXX score-greens  score@ -> s ----- }T
T{ W ABACK W XBXCX score-greens  score@ -> s -G-G- }T

TESTING SCORE-YELLOWS
T{ -score w ABCDE w xxBxx 2 score-yellow 2drop  score@ -> s --Y-- }T
T{ 0 score + c@ -> grey }T
T{ 2 score + c@ -> yellow }T
T{ 4 score + c@ -> grey }T
T{ 0 used + c@ -> 0 }T
T{ 1 used + c@ 0= -> 0 }T
T{ 2 used + c@ -> 0 }T
T{ w ABCDE w xxBBx 3 score-yellow  2drop score@ -> s --Y-- }T

T{ -score w AABCD w xxxxx score-yellows  score@ -> s ----- }T
T{ -score w AABCD w Bxxxx score-yellows  score@ -> s Y---- }T
T{ -score w AABCD w xxAxx score-yellows  score@ -> s --Y-- }T
T{ -score w AABCD w xxAAx score-yellows  score@ -> s --YY- }T
T{ -score w AABCD w xxAAA score-yellows  score@ -> s --YY- }T
T{ -score w AABCD w DDxxx score-yellows  score@ -> s Y---- }T
T{ -score w ALERT w RAISE score-yellows  score@ -> s YY--Y }T
T{ w AABCD w AxAxA 2dup score-greens score-yellows  score@ -> s G-Y-- }T

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
