\ Wordle game

include random.fs ( gforth )
: random-word ( -- w )  #words random ww ;

\ The score is a 5-char string containing these characters:
char G constant GREEN
char Y constant YELLOW
char - constant GREY

create secret  len allot ( the secret answer we are tryig to guess)
create guess   len allot ( the current guess for debugging )
create score   len allot ( the score for the guess, string of colors )

create yellows  len allot ( true if this letter has been used as a yellow )

variable guesses ( up to 6 allowed )

: .guesses  ." (" guesses @ 0 .r ." ) ";
: .game  ." secret: " secret w. ." guess: " guess w. .guesses ." score: " score w. ;

: clear-score
    score len grey fill
    yellows len erase ; clear-score

: new-game
    random-word secret w!
    guess len [char] ? fill
    clear-score  0 guesses ! ;

: secret@ ( pos -- ch ) secret + c@ ;
: score@  ( pos -- ch ) score + c@ ;
: score!  ( ch pos -- ) score + c! ;

: match ( char pos -- f )  secret@ = ;

: grey? ( pos -- f )  score@ grey = ;

: mark-yellow ( pos -- )  yellows +  1 swap c! ;
: yellow? ( pos -- f ) yellows + c@ ;

\ Score any green letters first, then we will ignore these
: score-green ( guess -- )
    len 0 do
        count i match if  green i score!  then
    loop drop ;

\ To score yellows, we check the grey letters
\ that have not already been used as yellows to avoid double counting.
: check-yellow ( char pos -- )
    len 0 do
        over i match  i grey? and  i yellow? not and
        if  yellow over score!  i mark-yellow  leave then
    loop 2drop ;

\ Score the yellow letters, ignoring existing green and yellow ones
: score-yellow ( guess -- )
    len 0 do
        i grey? if  count i check-yellow  else 1+ then
    loop drop ;

\ Score a word returning the score (saves guess and score)
: score-word ( guess -- score )
    dup guess w! ( we don't use it but good for diagnostics )
    clear-score dup
    score-green
    score-yellow
    score ;

\ Validate guesses (warnings for now)
: check-guess ( w -- )
    valid-guess not if ." Not in the word list. " then \ abort" Not in the word list"
    guesses @ 5 > if ." Too many guesses. " then \ abort" Too many guesses"
    ;

\ Make a guess and return the score
: make-guess ( w -- score )
    dup check-guess  1 guesses +!  score-word ;


( === Game UI === )
: NEW  new-game ;
: G  w make-guess  cr guesses ? w. ;


( === unit tests === )
include unit-test.fs

: s! secret w! ;

: t1 [w] ----- [w] ----- ;
: t2 [w] ----- [w] ----G ;

: setup ( secret guess score -- score guess )
    test  rot secret w!  swap  dup guess w!  clear-score ;

: expect-score ( score -- )
    dup score wcompare if fail .game ." Expected score " w. else drop then ;

: expect-green ( secret guess score -- )
    setup  score-green  expect-score ;

: test-score-green
    cr ." Testing SCORE-GREEN..." begin-unit-tests
    [W] ABACK [W] ABASE [W] GGG-- expect-green
    [W] ABASE [W] AWASH [W] G-GG- expect-green
    [W] ABACK [W] XXXXX [W] ----- expect-green
    report-unit-tests ;

test-score-green

: expect-yellow ( secret guess score -- )
    setup  score-yellow  expect-score ;

: test-score-yellow
    cr ." Testing SCORE-YELLOW..." begin-unit-tests
    [W] AABCD [W] xxxxx [W] ----- expect-yellow
    [W] AABCD [W] Bxxxx [W] Y---- expect-yellow
    [W] AABCD [W] xxAxx [W] --Y-- expect-yellow
    [W] AABCD [W] xxAAx [W] --YY- expect-yellow
    [W] AABCD [W] xxAAA [W] --YY- expect-yellow
    [W] AABCD [W] DDxxx [W] Y---- expect-yellow
    report-unit-tests ;

test-score-yellow

: expect-score-word ( guess score -- )
    test  swap score-word drop  expect-score ;

: test-score-word
    cr ." Testing SCORE-WORD..." begin-unit-tests
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

    [W] UNION S!  [W] NOUNS [W] YY-Y- expect-score-word

    report-unit-tests ;

test-score-word

\ forget-unit-tests
