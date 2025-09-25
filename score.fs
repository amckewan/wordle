( score a word )

\ Each letter can score grey, yellow or greeen, giving 3^5 possible scores.
\ Scores range from 0 (all grey) to 242 (all green), denoted by 's'.
\ Score:    G Y - - G
\ Encoded:  2 1 0 0 2  (base 3)
\ Decimal:  2*81 + 1*27 + 2*1 = 191

3 3 3 3 3 * * * * constant #scores

0 constant grey         
1 constant yellow
2 constant green

create score  len allot  \ working score for each letter (backwards)
create used   len allot  \ set when a letter is used for green or yellow

\ : score@ ( -- s )  0  score len bounds do  3 * i c@ +  loop ;
: score@ ( -- s )  0  score dup 4 + do  3 * i c@ +  -1 +loop ;

: score-greens ( secret guess -- ) \ sets all of score and used
    xor ( 0=green ) 4-0 do
        dup i mask and 0= green and
        dup i score + c!  i used + c!   
    -1 +loop drop ;

\ look in all positions for a candidate to score pos yellow
\ mark the position where we found the letter as used (avoid double-counting)
: score-yellow ( secret guess pos -- secret guess )
    4-0 do  i used + c@ 0= if ( unused )
        2dup get ( can do outside loop) 3 pick i get = if ( match @ secret )
            yellow   dup i used + c!   over score + c!   leave
        then
    then -1 +loop drop ;
: score-yellows ( secret guess -- )
    4-0 do  i score + c@ green < if i score-yellow then  -1 +loop 2drop ;

\ Score a guess against a secret, returning the score.
: score-guess ( secret guess -- score )
    2dup score-greens score-yellows score@ ;


\ literals
: >color ( c -- n )  2 rshift  3 and  3 swap - ( ascii tricks ) ;
: >s  ( a -- w )  0  len 0 do  swap count >color   rot 3 * +  loop nip ;
:  s  ( "w" -- s )  bl parse  len <> abort" need 5 letters"  >s ;
: [s] ( "w" -- s )  s  postpone literal ; immediate

\ Display score using -,Y,G
create schars  '-' c,  'Y' c,  'G' c,
: (s.)  <# len 0 do  3 /mod  swap schars + c@ hold  loop 0 #> ;
:  s.   (s.) type space ;

: .score ( for debug ) score len dump used len dump ;

TESTING (S.)
T{ 0 (s.) s" -----" compare -> 0 }T
T{ s GY-YG (s.) S" GY-YG" compare -> 0 }T

( ===== TESTS ===== )

: -score  score len erase  used len erase ;

TESTING SCORE@
T{ -score                   score@ -> 0                     }T
T{ yellow 0 score + c!      score@ -> yellow                }T
T{ green  1 score + c!      score@ -> green 3 * yellow +    }T
T{ score len green fill     score@ -> #scores 1-            }T

TESTING >S
T{ S" -----" drop >s -> grey }T
T{ S" ----Y" drop >s -> yellow }T
T{ S" ----G" drop >s -> green }T
T{ S" YY--Y" drop >s -> 1 27 81 + + }T
T{ S" YYYYY" drop >s -> 1 3 9 27 81 + + + + }T
T{ S" GGGGG" drop >s -> #scores 1- }T

TESTING S
T{ s ----- -> grey }T
T{ s ----Y -> yellow }T
T{ s ----G -> green }T
T{ s YYYYY -> 1 3 9 27 81 + + + + }T
T{ s GGGGG -> #scores 1- }T

TESTING SCORE-GREENS
T{ W ABCDE W ABCDE score-greens  score@ -> s GGGGG }T
T{ W ABACK W ABASE score-greens  score@ -> s GGG-- }T
T{ 4 used + c@ -> green }T
T{ 1 used + c@ -> grey }T
T{ W ABACK W XXXXX score-greens  score@ -> s ----- }T
T{ W ABACK W XBXCX score-greens  score@ -> s -G-G- }T

TESTING SCORE-YELLOWS
T{ -score w ABCDE w xxBxx 2 score-yellow 2drop  score@ -> s --Y-- }T
T{ 0 score + c@ -> grey }T
T{ 2 score + c@ -> yellow }T
T{ 4 score + c@ -> grey }T
T{ 0 used + c@ -> 0 }T
T{ 3 used + c@ 0= -> 0 }T
T{ 2 used + c@ -> 0 }T
T{ w ABCDE w xxBBx 1 score-yellow  2drop score@ -> s --Y-- }T

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
