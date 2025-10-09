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
    w dup guess .history ( s )
    solved if ." You WIN! " else
    failed if ." Better luck next time ( " secret w. ." ) " else 
    k then then ;



( ===== TESTS ===== )
testing guess
init-game w aback to secret
t{ w found guess -> s ----- }t
t{ w track guess -> s --ggg }t
t{ w aback guess -> s ggggg }t
t{ guesses -> 3 }t
t{ w madam guess -> s -y-y- }t
t{ w brand guess -> s y-g-- }t
t{ w block guess -> s y--gg }t
t{ guesses -> 6 }t
