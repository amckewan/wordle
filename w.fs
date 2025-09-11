\ words as integers

include utils/tester.fs

\ 5 bits per letter, last letter in lsb (keep sorting order)
\ -------1 11112222 23333344 44455555

5 constant len
char A constant A

: w ( -- w )  0  bl parse bounds do  5 lshift  i c@ A - or  loop ;

T{ w ABACK -> 32842 }T
T{ w RAISE -> 17834564 }T

\ fetch and store letters
: l@ ( pos w -- c )  4 rot - len * rshift  31 and  A + ;

T{ 0 w RAISE l@ -> char R }T
T{ 2 w RAISE l@ -> char I }T
T{ 4 w RAISE l@ -> char E }T

: l! ( c pos w -- w' )
    4 rot - len * >r ( shift )
    31 r@ lshift invert and ( mask off prev char )
    swap A - r> lshift or ;

T{ char X 0 w RAISE l! -> w XAISE }T
T{ char Y 2 w RAISE l! -> w RAYSE }T
T{ char Z 4 w RAISE l! -> w RAISZ }T

: w. ( w -- )  len 0 do i over l@ emit  loop drop space ;



