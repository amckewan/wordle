\ Wordle game

\ The score is a 5-char string containing these characters:
char G constant GREEN
char Y constant YELLOW
char - constant GREY

char A constant A   \ useful for letter sets

create secret  len allot ( the secret answer )
create guess   len allot ( current guess )
create score   len allot ( score for the guess, string of colors )
create used    len allot ( true if a letter has been used to satisfy a yellow score )

\ Build up answer based on green scores.
\ This is used by the solver but doesn't expose anything secret.
\ It is convenient to update it here when we score a word.
\ We start all grey, then change to the letter when we get a green.
create answer len allot




: .used  used len bounds do S" -X" drop i c@ l@ emit loop space ;
: .game  ." secret: " secret w. ." answer: " answer w. ." guess: " guess w. ." score: " score w. ( ." used: " .used ) ;

: clear-score  score len grey fill  used len erase ;

: random-word ( -- w )  #wordle-words random ww ;
: new-game  random-word secret w!  answer len grey fill  guess len grey fill  clear-score ;  new-game

: green?  ( pos -- f )  score l@ green = ;
: yellow? ( pos -- f )  score l@ yellow = ;
: grey?   ( pos -- f )  score l@ grey = ;

: #scores ( color -- n )  0  score len bounds do over i c@ = - loop nip ;

\ Score any green letters first, then we will ignore these
: score-green ( -- )  len 0 do
      i guess l@ i secret l@ = if ( match )
        green i score l!  i guess l@ i answer l!
    then loop ;

\ To score yellows, we check the non-green letters that have
\ not already been used as yellows (to avoid double counting).
: used? ( pos -- f )  used + c@ ;
: used! ( pos -- )    used + 1 swap c! ;

: check-yellow ( pos -- )  len 0 do
      dup guess l@ i secret l@ =  i green? not and  i used? not and
      if  yellow over score l!  i used!  leave then
    loop drop ;

: score-yellow ( -- )
    len 0 do  i green? not if  i check-yellow  then  loop ;

\ Score a word, leaving results in guess & score
: score-guess ( w -- )
    guess w!  clear-score  score-green  score-yellow ;


( === TESTS === )

: setup ( secret guess score -- score )  swap guess w! swap secret w! clear-score ;

TESTING SCORE-GREEN
T{ W ABACK W ABASE w GGG-- setup  score-green  score w= -> true }T
T{ W ABACK W XXXXX w ----- setup  score-green  score w= -> true }T
T{ W ABACK W XBXCX w -G-G- setup  score-green  score w= -> true }T

TESTING SCORE-YELLOW
T{ w AABCD w xxxxx w ----- setup  score-yellow  score w= -> true }T
T{ w AABCD w Bxxxx w Y---- setup  score-yellow  score w= -> true }T
T{ w AABCD w xxAxx w --Y-- setup  score-yellow  score w= -> true }T
T{ w AABCD w xxAAx w --YY- setup  score-yellow  score w= -> true }T
T{ w AABCD w xxAAA w --YY- setup  score-yellow  score w= -> true }T
T{ w AABCD w DDxxx w Y---- setup  score-yellow  score w= -> true }T
T{ w ALERT w RAISE w YY--Y setup  score-yellow  score w= -> true }T

TESTING SCORE-GUESS
T{ w AABCD w xxxxx w ----- setup  guess score-guess  score w= -> true }T
T{ w AABCD w Axxxx w G---- setup  guess score-guess  score w= -> true }T
T{ w AABCD w Dxxxx w Y---- setup  guess score-guess  score w= -> true }T
T{ w AABCD w DDDDx w Y---- setup  guess score-guess  score w= -> true }T
T{ w AABCD w xxAxx w --Y-- setup  guess score-guess  score w= -> true }T
T{ w AABCD w xxAAx w --YY- setup  guess score-guess  score w= -> true }T
T{ w AABCD w xxAAA w --YY- setup  guess score-guess  score w= -> true }T
T{ w AABCD w AxBDx w G-GY- setup  guess score-guess  score w= -> true }T
T{ w AABCD w AxAxA w G-Y-- setup  guess score-guess  score w= -> true }T

T{ w VIXEN w EERIE w Y--Y- setup  guess score-guess  score w= -> true }T
