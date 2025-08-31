( game UI )

: solved ( -- f )  #greens len = ;
: failed ( -- f )  guesses @ #guesses >= ; ( assuming not solved )

\ keyboard showing knowledge about each letter
create kb 26 allot
: kb@ ( c -- color )  A - kb + c@ ;
: kb! ( color c -- )  A - kb + c! ;

: +kb ( color c -- )
    over green = if kb! else
    over yellow = ( but don't overwrite a green...)
    over kb@ green <> and if kb! else 2drop then then ;

: mark-kb  score  guess len bounds do  count i c@ +kb  loop drop ;

: NEW  new-game  clear-history  kb 26 grey fill ;

: G ( "w" -- ) \ "G RAISE" etc.
    w pad w! pad len upper ( convenient )
    pad score-word save-history    
    cr 2 spaces score w. ." (" guesses @ 0 .r ." ) "
    solved if ." You WIN! " else
    failed if ." Better luck next time! " then then ;

: H .history ;
: K 26 0 do i A + emit  i kb + c@
    dup green = if drop [char] * else
    yellow = if [char] ? else [char] - then then emit space loop ;

