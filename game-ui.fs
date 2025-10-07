( game UI )

: new  new-game   ;

: h .history ;

'z' 1+ 'a' 2constant a-z ( -- limit index )
: .k ( c k -- )  over >kb c@ = if dup emit space then drop ;

: k cr ." Greens:  " greens w.
    cr ." Yellows: " a-z do i 2 .k loop
    cr ." Greys:   " a-z do i 1 .k loop
    cr ." Unused:  " a-z do i 0 .k loop ;

: g ( "w" -- ) \ "g raise" etc.
    w  dup valid-guess not abort" Not in word list"
    dup guess .history ( s )
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " .secret ." ) " else 
    k then then ;



( ===== TESTS ===== )
testing guess
init-game w aback secret!
t{ w defgh guess -> s ----- }t
t{ w axaxk guess -> s g-g-g }t
t{ w aback guess -> s ggggg }t
t{ guesses -> 3 }t
t{ w babaa guess -> s yy-y- }t
t{ w akbcb guess -> s gyyg- }t
t{ w ccbaa guess -> s y-yyy }t
t{ guesses -> 6 }t
