( game UI )

\ keyboard with knowledge about each letter played
\ 0=unknown, 1=grey, 2=yellow, 3=green (k = color+1)
create kb 32 allot

\ update kb with latest guess & score
: +kb ( k l -- )  kb +  2dup c@ > if c! else 2drop then ;
: update-kb ( guess score -- )
    len 0 do  over i get >r  3 /mod swap 1+  r> +kb  loop 2drop ;

: NEW  new-game  kb 32 erase ;

: H .history ;

: .k ( l k -- )  over kb + c@ = if dup l>c emit space then drop ;
: K cr ." Answer: " answer w.
    cr ." Green:  " a-z do i 3 .k loop
    cr ." Yellow: " a-z do i 2 .k loop
    cr ." Grey:   " a-z do i 1 .k loop
    cr ." Unused: " a-z do i 0 .k loop ;

: G ( "w" -- ) \ "G RAISE" etc.
    w dup valid-guess not abort" Not in word list"
    dup make-guess  swap over update-kb  .history
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " secret w. ." ) " else 
    k then then ;
