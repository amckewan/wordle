( score a word )

create used  len allot  \ set when letter is used for green or yellow

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
