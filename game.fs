\ Wordle game

(
    new-game

    s" RAISE" guess --> 
)

char * constant GREEN
char ? constant YELLOW
char . constant GREY

\ *?..?


variable solution  \ answer for the current game (0-#words)

create scratch len allot

: valid-guess ( a n -- w t | f )
    LEN <> IF  DROP FALSE EXIT  THEN
    
;

: guess ( a n -- a n )
    2dup valid-guess if



    else
        cr ." Invalid guess " type space abort
    then ;
