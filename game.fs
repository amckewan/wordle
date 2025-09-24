\ Wordle game

\ Game data (answer maintained for solver convenience)
0 value secret      ( the secret word we are trying to guess )
0 value answer      ( the answer we are building up, letter or 0 )
0 value guesses     ( number of guesses so far, 0-6 )

6 constant #guesses ( set by game )

: .game  ." secret: " secret w. ." answer: " answer w. ." guesses " guesses . ;

: solved ( score -- f ) #scores 1- = ;
: failed ( -- f )       guesses 5 > ; \ assuming not solved

: +answer ( guess score -- ) \ record green letters in answer
    len 0 do  3 /mod swap green = if
        over i get   answer i put   to answer
    then loop 2drop ;

: greens ( -- n )  0 answer  len 0 do
    dup cmask and 0= 1+  rot + swap  bits rshift  loop drop ;

\ init everything except the secret
: init-game ( -- )  0 to answer  0 to guesses ;

\ Initialize a new game and pick a random secret word.
: new-game ( -- )  init-game   #hidden random hidden@ to secret ; new-game



( ===== TESTS ===== )

TESTING +ANSWER
T{ w ----- to answer  w ABCDE s GY-G- +answer  answer -> w A--D- }T
T{                    w LMNOP s -G--- +answer  answer -> w AM-D- }T
T{                    w VWXYZ s ----- +answer  answer -> w AM-D- }T

TESTING GREENS
T{ w ----- to answer    greens -> 0 }T
T{ w -A-B- to answer    greens -> 2 }T
T{ w WINDY to answer    greens -> 5 }T

