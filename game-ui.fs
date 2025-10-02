( game UI )

\ Submit a guess to the game, update game state and return the score
: submit ( guess score -- )  2dup +history +answer ;
: guess  ( guess -- score )  dup score-guess tuck submit ;

\ keyboard with knowledge about each letter played
\ 0=unknown, 1=grey, 2=yellow, 3=green (k = color+1)
create kb 32 allot

\ update kb with latest guess & score
: >kb ( c -- a )  31 and kb + ;
: +kb ( k c -- )  >kb 2dup c@ > if c! else 2drop then ;
: update-kb ( guess score -- )
    swap for-chars do  i c@ >r  3 /mod swap 1+  r> +kb  loop drop ;

: NEW  new-game  kb 32 erase ;

: H .history ;

'z' 1+ 'a' 2constant a-z ( -- limit index )

: .k ( c k -- )  over >kb c@ = if dup emit space then drop ;
: K cr ." Answer: " answer w.
    cr ." Green:  " a-z do i 3 .k loop
    cr ." Yellow: " a-z do i 2 .k loop
    cr ." Grey:   " a-z do i 1 .k loop
    cr ." Unused: " a-z do i 0 .k loop ;

: G ( "w" -- ) \ "G RAISE" etc.
    w  dup valid-guess not abort" Not in word list"
    dup guess ( g s ) tuck update-kb  .history ( s )
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " .secret ." ) " else 
    k then then ;



( ===== TESTS ===== )
TESTING GUESS
w ABACK new-game-with
T{ w DEFGH guess -> s ----- }T
T{ w AxAxK guess -> s G-G-G }T
T{ w ABACK guess -> s GGGGG }T
T{ guesses -> 3 }T
T{ w BABAA guess -> s YY-Y- }T
T{ w AKBCB guess -> s GYYG- }T
T{ w CCBAA guess -> s Y-YYY }T
T{ guesses -> 6 }T
