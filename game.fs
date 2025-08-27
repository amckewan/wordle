\ Wordle game

\ The score is a 5-char string containing these characters:
char G constant GREEN
char Y constant YELLOW
char - constant GREY

create secret  len allot ( the secret answer )
create guess   len allot ( current guess )
create score   len allot ( score for the guess, string of colors )
create used    len allot ( a letter used for yellow or zero )

: .game  ." secret: " secret w. ." guess: " guess w. ." score: " score w.
     ." used: " used len bounds do i c@ ?dup 0= if [char] - then emit loop space ;

: clear-score  score len grey fill  used len erase ;

: random-word ( -- w )  #words random ww ;

: new-game  random-word secret w!  guess len blank  clear-score ;  new-game

: color?  ( pos color -- f )  swap score + c@ = ;
: green?  ( pos -- f )  green color? ;
: yellow? ( pos -- f )  yellow color? ;
: grey?   ( pos -- f )  grey color? ;

: #greens ( -- n )  0  len 0 do  i green? 1 and +  loop ;

\ Score any green letters first, then we will ignore these
: match  ( char pos -- f )  secret + c@ = ;
: score! ( char pos -- )    score + c! ;
: score-green ( -- )
    len 0 do  i guess + c@  i match if  green i score!  then  loop ;

\ To score yellows, we check the non-green letters that have
\ not already been used as yellows (to avoid double counting).
: used? ( pos -- f )   used + c@ ;
: used! ( ch pos -- )  used + c! ;

: check-yellow ( char pos -- )
    len 0 do
        over i match  i green? not and  i used? not and
        if  yellow over score!  over i used!  leave then
    loop 2drop ;

: score-yellow ( -- )
    len 0 do  i green? not if  i guess + c@  i check-yellow  then  loop ;


\ Score a word returning the score (saves guess and score)
: score-word ( guess -- )
    guess w!  clear-score  score-green  score-yellow ;


( === Game UI === )
: NEW  new-game ;
: G  w score-word  cr 2 spaces w. ;


( === unit tests === )
include unit-test.fs

: s! secret w! ;

: t1 [w] ----- [w] ----- ;
: t2 [w] ----- [w] ----G ;

: setup ( secret guess score -- score )
    test  swap guess w!  swap secret w!  clear-score ;

: expect-score ( score -- )
    dup score wcompare if fail ." Expected score " w. ." got " .game else drop then ;

: expect-green ( secret guess score -- )
    setup  score-green  expect-score ;

: test-score-green
    s" score-green" begin-unit-tests
    [W] ABACK [W] ABASE [W] GGG-- expect-green
    [W] ABASE [W] AWASH [W] G-GG- expect-green
    [W] ABACK [W] XXXXX [W] ----- expect-green
    report-unit-tests ;

test-score-green


: expect-yellow ( secret guess score -- )
    setup  score-yellow  expect-score ;

: test-score-yellow
    s" score-yellow" begin-unit-tests
    [W] AABCD [W] xxxxx [W] ----- expect-yellow
    [W] AABCD [W] Bxxxx [W] Y---- expect-yellow
    [W] AABCD [W] xxAxx [W] --Y-- expect-yellow
    [W] AABCD [W] xxAAx [W] --YY- expect-yellow
    [W] AABCD [W] xxAAA [W] --YY- expect-yellow
    [W] AABCD [W] DDxxx [W] Y---- expect-yellow
    [W] ALERT [W] RAISE [W] YY--Y expect-yellow \ Y---Y
    report-unit-tests ;

test-score-yellow


: expect-score-word ( guess score -- )
    test  swap score-word  expect-score ;

: test-score-word
    s" score-word" begin-unit-tests
    [W] AABCD s!  [W] xxxxx [W] ----- expect-score-word
                  [W] Axxxx [W] G---- expect-score-word
                  [W] Dxxxx [W] Y---- expect-score-word
                  [W] DDDDx [W] Y---- expect-score-word
                  [W] xxAxx [W] --Y-- expect-score-word
                  [W] xxAAx [W] --YY- expect-score-word
                  [W] xxAAA [W] --YY- expect-score-word
                  [W] AxBDx [W] G-GY- expect-score-word 
                  [W] AxAxA [W] G-Y-- expect-score-word

    [W] ABLED s!  [W] ALLEY [W] G-GG- expect-score-word
                  [W] ALLEL [W] G-GG- expect-score-word

    [W] UNION S!  [W] NOUNS [W] YYYY- expect-score-word

    [W] ALERT S!  [W] RAISE [W] YY--Y expect-score-word \ Y---Y

    report-unit-tests ;

test-score-word

forget-unit-tests
