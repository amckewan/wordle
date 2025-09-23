( score a word )

\ Each letter can score grey, yellow or greeen, giving 3^5 possible scores.
\ Scores range from 0 (all grey) to 242 (all green), denoted by 's'.

0 constant grey         
1 constant yellow
2 constant green

create score    len allot   \ working score for each letter
create used     len allot   \ set when a letter is used for green or yellow

: -score  score len erase  used len erase ;

: s@  ( pos -- color )  score + c@ ;
: s!  ( color pos -- )  score + c! ;

: u@  ( pos -- f )      used + c@ ;
: u!  ( f pos -- )      used + c! ;

: score@ ( -- s )  0  0 len 1- do  3 * i s@ +  -1 +loop ;

: score-greens ( secret guess -- ) \ sets score and used
    xor ( 0=green ) len 0 do
        dup i mask and 0=   dup i u!   green and i s!
    loop drop ;

\ look in all positions for a candidate to score pos yellow
: score-yellow ( secret guess pos -- secret guess )
    len 0 do  i u@ 0= if ( unused )
        2dup get 3 pick i get = if ( found a match )
            yellow over s!   true i u!   leave
        then  
    then loop drop ;
: score-yellows ( secret guess -- )
    len 0 do  i s@ green < if i score-yellow then  loop 2drop ;

\ Score a guess against a secret, returning the score (0-242).
: score-guess ( secret guess -- score )
    2dup score-greens score-yellows score@ ;


\ Convert score to a wordle word for display and testing.
create schars  '@' '@' - c,  'Y' '@' - c,  'G' '@' - c,
: s>w ( s -- w )  0 swap  len 0 do ( w s )
      3 /mod ( w r q ) swap schars + c@ ( w q l ) i left rot or swap
    loop drop ;
: s. ( s -- )  s>w w. ;
: .score ( for debug )
    ." score: " score@ s>w w.
    ." used: " used len bounds do i c@ if 'X' else '.' then emit loop space ;



( ===== TESTS ===== )

TESTING SCORE@
T{ score len erase          score@ -> 0 }T
T{ yellow 0 s!              score@ -> yellow }T
T{ green  1 s!              score@ -> green 3 * yellow + }T
T{ score len green fill     score@ -> 3 3 * 3 * 3 * 3 * 1- }T

TESTING S>W
T{ 0 S>W -> w ----- }T
T{ 242 S>W -> w GGGGG }T

TESTING SCORE-GREENS
T{ W ABCDE W ABCDE score-greens  score@ s>w -> w GGGGG }T
T{ W ABACK W ABASE score-greens  score@ s>w -> w GGG-- }T
T{ W ABACK W XXXXX score-greens  score@ s>w -> w ----- }T
T{ W ABACK W XBXCX score-greens  score@ s>w -> w -G-G- }T

TESTING SCORE-YELLOWS
T{ -score w ABCDE w xxBxx 2 score-yellow 2drop  score@ s>w -> w --Y-- }T
T{ 0 s@ -> grey }T
T{ 2 s@ -> yellow }T
T{ 4 s@ -> grey }T
T{ 0 u@ -> 0 }T
T{ 1 u@ 0= -> 0 }T
T{ 2 u@ -> 0 }T
T{ w ABCDE w xxBBx 3 score-yellow  2drop score@ s>w -> w --Y-- }T

T{ -score w AABCD w xxxxx score-yellows  score@ s>w -> w ----- }T
T{ -score w AABCD w Bxxxx score-yellows  score@ s>w -> w Y---- }T
T{ -score w AABCD w xxAxx score-yellows  score@ s>w -> w --Y-- }T
T{ -score w AABCD w xxAAx score-yellows  score@ s>w -> w --YY- }T
T{ -score w AABCD w xxAAA score-yellows  score@ s>w -> w --YY- }T
T{ -score w AABCD w DDxxx score-yellows  score@ s>w -> w Y---- }T
T{ -score w ALERT w RAISE score-yellows  score@ s>w -> w YY--Y }T
T{ w AABCD w AxAxA 2dup score-greens score-yellows  score@ s>w -> w G-Y-- }T

TESTING SCORE-GUESS
T{ w AABCD w xxxxx score-guess s>w -> w ----- }T
T{ w AABCD w Axxxx score-guess s>w -> w G---- }T
T{ w AABCD w Dxxxx score-guess s>w -> w Y---- }T
T{ w AABCD w DDDDx score-guess s>w -> w Y---- }T
T{ w AABCD w xxAxx score-guess s>w -> w --Y-- }T
T{ w AABCD w xxAAx score-guess s>w -> w --YY- }T
T{ w AABCD w xxAAA score-guess s>w -> w --YY- }T
T{ w AABCD w AxBDx score-guess s>w -> w G-GY- }T
T{ w AABCD w AxAxA score-guess s>w -> w G-Y-- }T

T{ w VIXEN w EERIE score-guess s>w -> w Y--Y- }T




0 [if]

: score-greens ( secret guess -- score )
    xor ( 0=green ) 0 ( score ) len 0 do 
      over i mask and 0=  dup used i + c!  green i left and or  loop nip ;



\ look in all positions for a candidate to score pos yellow
: score-yellow ( secret guess score pos -- secret guess score' )
    len 0 do  used i + c@ 0= if ( not used )
        2 pick over get 4 pick i get = if ( found a match )
            yellow swap left or ( score yellow )
            1 used i + c! ( mark the letter we used )
            unloop exit
        then            
    then loop drop ;

: score-yellows ( secret guess score -- score' )
    len 0 do  dup i get green xor if  i score-yellow  then loop   nip nip ;

: score-guess ( secret guess -- score )
    2dup score-greens score-yellows ;


( ===== TESTS ===== )

TESTING SCORE-GREENS
T{ W ABCDE W ABCDE score-greens -> w GGGGG }T
T{ W ABACK W ABASE score-greens -> w GGG-- }T
T{ W ABACK W XXXXX score-greens -> w ----- }T
T{ W ABACK W XBXCX score-greens -> w -G-G- }T

TESTING SCORE-YELLOWS
used len erase
T{ w abcde w xxbxx 0 2 score-yellow -> w abcde w xxbxx w --Y-- }T
T{ used 1 + c@ -> 1 }T
T{ used 2 + c@ -> 0 }T
T{ w abcde w xxbbx w --Y-- 3 score-yellow -> w abcde w xxbbx w --Y-- }T

T{ w AABCD w xxxxx 0 score-yellows -> w ----- }T
T{ w AABCD w Bxxxx 0 score-yellows -> w Y---- }T
T{ w AABCD w xxAxx 0 score-yellows -> w --Y-- }T used len erase
T{ w AABCD w xxAAx 0 score-yellows -> w --YY- }T used len erase
T{ w AABCD w xxAAA 0 score-yellows -> w --YY- }T
T{ w AABCD w DDxxx 0 score-yellows -> w Y---- }T used len erase
T{ w ALERT w RAISE 0 score-yellows -> w YY--Y }T used len erase 1 used c!
T{ w AABCD w AxAxA w G---- score-yellows -> w G-Y-- }T

TESTING SCORE-GUESS
T{ w AABCD w xxxxx score-guess -> w ----- }T
T{ w AABCD w Axxxx score-guess -> w G---- }T
T{ w AABCD w Dxxxx score-guess -> w Y---- }T
T{ w AABCD w DDDDx score-guess -> w Y---- }T
T{ w AABCD w xxAxx score-guess -> w --Y-- }T
T{ w AABCD w xxAAx score-guess -> w --YY- }T
T{ w AABCD w xxAAA score-guess -> w --YY- }T
T{ w AABCD w AxBDx score-guess -> w G-GY- }T
T{ w AABCD w AxAxA score-guess -> w G-Y-- }T

T{ w VIXEN w EERIE score-guess -> w Y--Y- }T

[then]
