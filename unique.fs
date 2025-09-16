( count unique letters )

: unique ( w -- n ) \ # unique letters in a word ABCDD -> 4
    pad 32 1 fill  0 ( n )  len 0 do
        over i get pad + ( w n a )
        dup c@ rot + ( update unique )  0 rot c! ( clear it )
    loop nip ;

TESTING UNIQUE
T{ w AAAAA unique -> 1 }T
T{ w ABCDD unique -> 4 }T
T{ w ABCDE unique -> 5 }T
T{ w ABABA unique -> 2 }T
T{ w AAAAZ unique -> 2 }T
