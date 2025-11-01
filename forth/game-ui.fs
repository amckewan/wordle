( game UI )

: new  new-game ;

: h .history ;

'z' 1+ 'a' 2constant a-z ( -- limit index )
: .k ( c k -- )  over >kb c@ = if dup emit space then drop ;

: k cr ." Greens:  " greens len type
    cr ." Yellows: " a-z do i 2 .k loop
    cr ." Greys:   " a-z do i 1 .k loop
    cr ." Unused:  " a-z do i 0 .k loop ;

: g ( "w" -- ) \ "g raise" etc.
    w dup submit .history ( s )
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " secret w. ." ) " else 
    cr k then then ;
