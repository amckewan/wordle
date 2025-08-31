( string to uppercase )

\ in place, just works for ascii letters
: upper ( a n -- ) bounds ?do i c@ [ char a char A - invert ] literal and i c! loop ;
