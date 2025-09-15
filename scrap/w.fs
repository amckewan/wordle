\ words as integers

decimal
include utils/tester.fs

: wordle 36 base ! ;

create 36s  1 , 36 dup , 36 * dup , 36 * dup , 36 * ,

: pow36  ( n -- 36^n )  cells 36s + @ ; \ n = 0 to 4

: p36 1 swap 0 ?do 36 * loop ;

\ extract a letter
: letter ( w pos -- c ) cells 36s + @ / 36 mod 'A' + ;


0 [if]
\ Is letter 3 a D?
guessed 3 l@ 'D' = 

\ do words match at letter pos?

\ score letter 3 green
'G' 3 scored L!

: decompose ( w -- c1 c2 c3 c4 c5 )


: compose ( c1 c2 c3 c4 c5 -- w )

: xxx ( w pos -- c )
: yyy ( c w pos -- )

scored @  3 'G' lset  scored !

: lset ( w pos c -- w' )

\ 5 bits per letter, last letter in lsb (keep sorting order)
\ -------1 11112222 23333344 44455555

5 constant len

T{ w ABACK -> 32842 }T
T{ w RAISE -> 17834564 }T

\ : shift ( pos w -- w shift )  4 rot - len * ;

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

: w. ( w -- )  len 0 do  i over l@ emit  loop drop  space ;


: ww,  w , ;

create wordle-words 100 cells allot ( ... )

: ww ( w# -- w )  cells wordle-words + @ ;

[then]
