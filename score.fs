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

\ Score a word, leaving the word in 'guess' and the score in 'score'
: score-word ( w -- )
    to guess  clear-score  score-green  score-yellow ;



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
