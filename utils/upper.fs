( string to uppercase )

\ Only works for letters
: upc ( c -- C )  [ char a char A - invert ] literal and ;

\ This one only changes a-z
\ : upc ( c -- C )  dup [char] a - 25 u>   [ char a char A - invert ] literal or   and ;

: upper ( a n -- ) bounds ?do i c@ upc i c! loop ;
