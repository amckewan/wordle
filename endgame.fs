( Stop the guessing game )

\ Sometimes we get 3 or 4 greens that can be filled in various ways,
\ but we don't have enough guesses left to try them all.

\ Preconditions: turn 4 or 5, 4 greens, 

\ Get the green letters from history
create answer len allot

: #greens ( -- n )
    \ need 4 greens in the latest guess
    \ not really, we may know greens but guessing something else
    \ we want to keep track of the known green letters, see guess.fs
    answer len grey fill
    0 ( n) guesses @ 0 ?do
        i >score  len 0 do
            i over + c@ green =  i answer + c@ grey = and if
                i j >guess + c@  i answer + c!
                swap 1+ swap
            then
        loop drop
    loop ;


