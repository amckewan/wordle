( save history of guesses and scores )

6 constant #guesses     ( set by game )

0 value guesses         ( number of guesses so far, 0-6 )

\ The history has 6 entries of score+guess
create histbuf  #guesses 2 cells * allot

: history ( n -- a )  2 cells * histbuf + ;

\ ansi colors
: esc  27 emit ." [" ;
: color ( color -- )  dup yellow = if 2 + then 100 +  esc 0 .r ." ;30m" ;
: normal  esc ." 0m" ;
: .colored ( guess score -- )
    swap ww len bounds do
        3 /mod swap color
        i c@ bl xor ( upc ) emit
    loop drop normal space ;

: .history  guesses 0 ?do  cr i 1+ .  i history 2@ .colored  loop ;

: +history ( guess score -- )
    guesses #guesses u< not abort" History full!"
    guesses history 2!  guesses 1+ to guesses ;

: latest ( -- guess score )  guesses 1- history 2@ ;
