( score a word )

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
