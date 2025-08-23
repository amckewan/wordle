\ Wordle game

include random.fs ( gforth )

\ The score is a 5-char string containing these characters:
char G constant GREEN
char Y constant YELLOW
char - constant GREY

create secret  len allot ( the secret answer we are tryig to guess)
create guess   len allot ( the current guess )
create score   len allot ( the score for the guess, string of colors )

variable guesses ( up to 6 allowed )

: .guesses  ." (" guesses @ 0 .r ." ) ";
: .game  ." secret: " secret w. ." guess: " guess w. .guesses ." score: " score w. ;

: clear-score  score len grey fill ; clear-score

: new-game
    #words random ww secret w!
    guess len [char] ? fill
    clear-score  0 guesses ! ;

: secret@ ( pos -- ch ) secret + c@ ;
: guess@  ( pos -- ch ) guess + c@ ;
: score@  ( pos -- ch ) score + c@ ;
: score!  ( ch pos -- ) score + c! ;

: match ( char pos -- f )  secret + c@ = ;

\ Score any green letters first, then we will ignore these
: score-green ( -- )
    len 0 do  i guess@ i match if  green i score!  then  loop ;

: grey? ( pos -- f ) score@ grey = ;

\ Ignoring geen letters, score yellow if a letter exists in a different spot
\ a: RAISE g: ABACE => Y---G ( only score first A )

\ : (score-yellow) ( pos -- )

\ score yellow for the this position, knowing it's still grey
: score-yellow-letter ( pos -- )
    dup guess@
    len 0 do
        dup i secret@ =  i grey? and
        if  drop  yellow swap score!  ( todo: remember that we found one ) unloop exit  then
    loop 2drop ;

: score-yellow ( -- )
    len 0 do
        i grey? if  i score-yellow-letter  then
    loop ;

\ Make a guess and return the score (static).
: make-guess-unchecked ( w -- score )
    guess w!  clear-score  1 guesses +!
    score-green
    score-yellow
    ;

\ check if a guess is valid, in one of the two word lists
: valid-guess ( w -- f )
    dup find-wordle-word not if find-guess-word ( else drop true ) then ;

: check-guess ( w -- )
    guesses @ 5 > abort" Too many guesses"
    valid-guess not abort" Guess is not a known word" ;


: make-guess ( w -- score )  dup check-guess make-guess-unchecked ;

( === Game UI === )
: NEW  new-game ;
: N  new-game [W] FORTH secret w! ; N
: G  w make-guess  cr guesses ? score w. ;


( === unit tests === )
include unit-test.fs

: expect-valid ( w -- ) test
    dup valid-guess not if fail ." expected valid " W. else drop then ;
: expect-not-valid ( w -- ) test
    dup valid-guess if fail ." expected not valid " W. else drop then ;

: test-valid-guess
  cr ." Testing VALID-GUESS..." begin-unit-tests

    ( wordle words )
    [W] ABACK expect-valid
    [W] RAISE expect-valid
    [W] ZONAL expect-valid

    ( guess words )
    [W] ABLOW expect-valid
    [W] PONGO expect-valid
    [W] ZYMIC expect-valid

    ( invalid words )
    [W] XXXXX expect-not-valid
    [W] ABACC expect-not-valid

  report-unit-tests ;


: expect-green ( secret guess score -- ) test
    swap guess w!  swap secret w!  clear-score  score-green
    dup score w= not if fail ." Expected score " w. ." got score " score w.
    else drop then ;

: test-score-green
    cr ." Testing SCORE-GREEN..." begin-unit-tests
    [W] ABACK [W] ABASE [W] GGG-- expect-green
    [W] ABASE [W] AWASH [W] G-GG- expect-green
    [W] ABACK [W] DEFER [W] ----- expect-green
    report-unit-tests ;

: expect-yellow ( secret guess score -- )
    test  swap guess w! swap secret w! clear-score  score-yellow
    dup score w= if drop else fail ." Expected yellow score " w. .game then ;

: test-score-yellow
    cr ." Testing SCORE-YELLOW..." begin-unit-tests
    [W] AABCD [W] xxxxx [W] ----- expect-yellow
    [W] AABCD [W] Bxxxx [W] Y---- expect-yellow
    [W] AABCD [W] xxAxx [W] --Y-- expect-yellow
    [W] AABCD [W] DDxxx [W] Y---- expect-yellow ( this fails, see above )
    report-unit-tests ;

: s! secret w! ;

: expect-score ( guess score -- )
    test clear-score  swap make-guess-unchecked
    dup score w= if drop else fail .game ." Expected score " w. then ;

: test-make-guess
    cr ." Testing MAKE-GUESS..." begin-unit-tests
    [W] AABCD s!  [W] xxxxx [W] ----- expect-score
                  [W] Axxxx [W] G---- expect-score
                  [W] Dxxxx [W] Y---- expect-score
                  [W] xxAxx [W] --Y-- expect-score
                  [W] xxAAx [W] --Y-- expect-score
                  [W] AxBDx [W] G-GY- expect-score

    [W] ABLED s!  [W] ALLEY [W] G-GG- expect-score
    [W] ABLED s!  [W] ALLEL [W] G-GG- expect-score
    report-unit-tests ;


test-valid-guess
test-score-green
test-score-yellow
test-make-guess

forget-unit-tests
