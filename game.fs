\ Wordle game

create secret  len allot ( the secret answer we are tryig to guess)
create guess   len allot ( the current guess )
create score   len allot ( the score for the guess )

\ The score is a 5-char string with these three characters:
char G constant GREEN
char Y constant YELLOW
char - constant GREY

\ check if a guess is valid, in one of the two word lists
: valid-guess ( w -- f )
    dup find-wordle-word 0= if find-guess-word ( else drop true ) then ;

: match ( pos -- f )  dup guess + c@  swap secret + c@ = ;

: score-green ( -- )
    len 0 do  i match if  green score i + c!  then  loop ;

: grey? ( pos -- f ) score + c@ grey = ;

\ Ignoring geen letters, score yellow if a letter exists in a different spot
\ a: RAISE g: ABACE => Y---G ( only score first A )

\ : (score-yellow) ( pos -- )

: score-yellow ( -- )
 \   len 0 do  i (score-yellow)  loop ;


    len 0 do
        i grey? ( ignore existing greens and yellows )
        if  ( if the letter exists to the right, mark it yellow )
            guess i + c@ ( candidate letter )
            len i 1+ ?do
                dup secret i + c@ =  i grey? and
                if

                then
            loop drop
        then
    loop ;

: guessit ( w -- score )  ( score is static! )
    score len grey fill
    dup score-green  dup score-yellow ;

\ W RAISE GUESS W. *..?. ok
\ W RAISE GUESS W. G--Y- ok
\ G RAISE
\ R A I S E
\ G . . Y .
\

( === unit tests === )
include unit-test.fs

: expect-valid ( w -- ) test
    dup valid-guess 0= if fail ." expected valid " W. else drop then ;
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

test-valid-guess
forget-unit-tests
