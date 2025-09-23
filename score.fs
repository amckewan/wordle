( score a word )

create used  len allot

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
    len 0 do  dup i get green xor if ( not scored green ) i score-yellow  then
    loop nip nip ;

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










0 [if]
: used? ( pos -- f )  used swap get ;
: used! ( pos -- )    1 used rot put to used ;






: scored ( color pos -- f )  score swap get = ;
: score! ( color pos -- )    score swap put to score ;

\ Score any green letters first, then we will ignore these
: +answer ( pos -- )
    guess over get  answer rot put  to answer   greens 1+ to greens ;
: score-green ( -- )
    len 0 do  guess secret i match if  green i score!
      answer i get 0= if ( record new green ) i +answer  then
    then loop ;

: sg ( secret guess -- score ) \ unroll loop
    xor dup cmask and 0= green and ( xor score )
    over [ 1 mask ] literal and 0= [ green 1 left ] literal and or
    over [ 2 mask ] literal and 0= [ green 2 left ] literal and or
    over [ 3 mask ] literal and 0= [ green 3 left ] literal and or
    over [ 4 mask ] literal and 0= [ green 4 left ] literal and or
    nip ;

TESTING SG
T{ w AAAAA w AAAAA sg -> w GGGGG }T
T{ w JKLMN w JKLMN sg -> w GGGGG }T
T{ w AAAAA w BBBBB sg -> w ----- }T
T{ w AAAAA w ABBAA sg -> w G--GG }T

\ To score yellows, we check the non-green letters that have
\ not already been used as yellows (to avoid double counting).
: used? ( pos -- f )  used swap get ;
: used! ( pos -- )    1 used rot put to used ;

: check-yellow ( pos -- )  len 0 do
      guess over get  secret i get =  green i scored not and  i used? not and
      if  yellow over score!  i used!  leave then
    loop drop ;

: score-yellow ( -- )
    len 0 do  green i scored not if  i check-yellow  then  loop ;

: sy  0 { secret guess score used -- score' }
    len 0 do score i get green <> if
        len 0 do
            guess j get secret i get =  score i get green <> and  used i get 0= and
            if  yellow score j put  1 used i put  then
        loop
    then loop nip nip ;

\ Score a word, leaving the word in 'guess' and the score in 'score'
: score-word ( w -- )
    to guess  clear-score  score-green  score-yellow ;


: score-wordx ( secret guess -- score )
    sg sy
;


( === TESTS === )
: setup ( secret guess score -- score )
    swap to guess  swap to secret  clear-score ;

TESTING SCORE-GREEN
T{ W ABACK W ABASE w GGG-- setup  score-green -> score }T
T{ W ABACK W XXXXX w ----- setup  score-green -> score }T
T{ W ABACK W XBXCX w -G-G- setup  score-green -> score }T

TESTING SCORE-YELLOW
T{ w AABCD w xxxxx w ----- setup  score-yellow -> score }T
T{ w AABCD w Bxxxx w Y---- setup  score-yellow -> score }T
T{ w AABCD w xxAxx w --Y-- setup  score-yellow -> score }T
T{ w AABCD w xxAAx w --YY- setup  score-yellow -> score }T
T{ w AABCD w xxAAA w --YY- setup  score-yellow -> score }T
T{ w AABCD w DDxxx w Y---- setup  score-yellow -> score }T
T{ w ALERT w RAISE w YY--Y setup  score-yellow -> score }T

TESTING SCORE-WORD
T{ w AABCD w xxxxx w ----- setup  guess score-word -> score }T
T{ w AABCD w Axxxx w G---- setup  guess score-word -> score }T
T{ w AABCD w Dxxxx w Y---- setup  guess score-word -> score }T
T{ w AABCD w DDDDx w Y---- setup  guess score-word -> score }T
T{ w AABCD w xxAxx w --Y-- setup  guess score-word -> score }T
T{ w AABCD w xxAAx w --YY- setup  guess score-word -> score }T
T{ w AABCD w xxAAA w --YY- setup  guess score-word -> score }T
T{ w AABCD w AxBDx w G-GY- setup  guess score-word -> score }T
T{ w AABCD w AxAxA w G-Y-- setup  guess score-word -> score }T

T{ w VIXEN w EERIE w Y--Y- setup  guess score-word -> score }T
[then]
