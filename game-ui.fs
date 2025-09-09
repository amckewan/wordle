( game UI )

: solved ( -- f )  greens @ len = ;
: failed ( -- f )  guesses @ #guesses >= ; ( assuming not solved )

\ keyboard with knowledge about each letter played
\ 0=unknown, 1=grey, 2=yellow, 3=green
create kb 26 allot
: kb@ ( c -- color )  A - kb + c@ ;
: kb! ( color c -- )  A - kb + c! ;

\ update kb with latest guess & score
: >k ( color -- n )  dup green = if drop 3 else yellow = negate 1+ then ;
: +kb ( color c -- )  swap >k swap  A - kb + 2dup c@ > if c! else 2drop then ;
: update-kb  score  guess len bounds do  count i c@ +kb  loop drop ;

: NEW  new-game  clear-history  kb 26 erase ;

: H .history ;

: .k ( c n -- )  over kb@ = if dup emit space then drop ;
: K cr ." Green:  " A 26 bounds do i 3 .k loop
    cr ." Yellow: " A 26 bounds do i 2 .k loop
    cr ." Grey:   " A 26 bounds do i 1 .k loop
    cr ." Unused: " A 26 bounds do i 0 .k loop ;

: G ( "w" -- ) \ "G RAISE" etc.
    w pad w! pad dup len upper ( convenient )
    score-guess add-history update-kb .history
    solved if ." You WIN! " else
    failed if ." Better luck next time (" secret len type ." )" else 
    k then then ;
