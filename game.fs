\ Wordle game

\ The score is a wordle word containing these letters:
'G' '@' - constant GREEN
'Y' '@' - constant YELLOW
        0 constant GREY     ( displayed as '-')

\ Game data (answer and greens maintained for solver convenience)
0 value secret    ( the secret answer )
0 value answer    ( the answer we are building up, letter or 0 )
0 value greens    ( # of greens in answer )

\ Per-round data, set by make-guess (along with updating answer and greens)
0 value guess     ( current guess )
0 value score     ( score for the guess, string of colors )
0 value used      ( true if position used to satisfy a yellow score -internal)

: .game  ." secret: " secret w. ." answer: " answer w.
         ." guess: "  guess w.  ." score: "  score w. ;

: clear-score ( -- )  0 to score  0 to used ;
: random-word ( -- w )  #wordle-words random ww ;

: new-game ( -- )  random-word to secret  0 to answer  0 to greens
    0 to guess  clear-score ;  new-game

: scored ( color pos -- f )  score swap get = ;
: score! ( color pos -- )    score swap put to score ;

\ Score any green letters first, then we will ignore these
: score-green ( -- )  len 0 do  guess secret i match if
      green i score!  answer i get 0= ( record new green )
      if  guess i get answer i put to answer  greens 1+ to greens  then
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

\ Score a word, leaving results in guess & score
: score-guess ( w -- )  to guess  clear-score  score-green  score-yellow ;

\ or is this the API for the game?
\ : new-game
\ : make-guess ( w -- score ) ...
\ updates guess, score, answer greens

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

TESTING SCORE-GUESS
T{ w AABCD w xxxxx w ----- setup  guess score-guess -> score }T
T{ w AABCD w Axxxx w G---- setup  guess score-guess -> score }T
T{ w AABCD w Dxxxx w Y---- setup  guess score-guess -> score }T
T{ w AABCD w DDDDx w Y---- setup  guess score-guess -> score }T
T{ w AABCD w xxAxx w --Y-- setup  guess score-guess -> score }T
T{ w AABCD w xxAAx w --YY- setup  guess score-guess -> score }T
T{ w AABCD w xxAAA w --YY- setup  guess score-guess -> score }T
T{ w AABCD w AxBDx w G-GY- setup  guess score-guess -> score }T
T{ w AABCD w AxAxA w G-Y-- setup  guess score-guess -> score }T

T{ w VIXEN w EERIE w Y--Y- setup  guess score-guess -> score }T
