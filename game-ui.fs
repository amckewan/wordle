( game UI )

: solved ( -- f )  guess secret = ;
: failed ( -- f )  guesses #guesses >= ; ( assuming not solved )

\ keyboard with knowledge about each letter played
\ 0=unknown, 1=grey, 2=yellow, 3=green (k)
create kb 32 allot

\ update kb with latest guess & score
: >k ( color -- k )  dup green = if drop 3 else yellow = negate 1+ then ;
: +kb ( k l -- )  kb +  2dup c@ > if c! else 2drop then ;
: update-kb  len 0 do  score i get >k  guess i get +kb  loop ;

: NEW  new-game  clear-history  kb 32 erase ;

: H .history ;

: .k ( l k -- )  over kb + c@ = if dup l>c emit space then drop ;
: a-z ( -- limit index )  27 1 ;
: K cr ." Green:  " a-z do i 3 .k loop
    cr ." Yellow: " a-z do i 2 .k loop
    cr ." Grey:   " a-z do i 1 .k loop
    cr ." Unused: " a-z do i 0 .k loop ;

: G ( "w" -- ) \ "G RAISE" etc.
    w score-guess add-history update-kb .history
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " secret w. ." ) " else 
    k then then ;
