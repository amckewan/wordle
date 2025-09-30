\ Wordle game

\ Game data
wordle secret       ( the secret word we are trying to guess )
0 value guesses     ( number of guesses so far, 0-6 )

6 constant #guesses ( set by game )

\ Maintain partial answer for solver convenience, letter or '-'
wordle answer
: +answer ( guess score -- ) \ record any green letters in answer
    len 0 do  3 /mod swap green = if
        over i + c@  i answer + c!
    then loop 2drop ;
: greens ( -- n )  0  answer len bounds do  i c@ '-' = 1+ +  loop ;

: .game ." secret: " secret w. ." answer: " answer w. ." guesses: " guesses . ;

: solved ( score -- f ) #scores 1- = ;
: failed ( -- f )       guesses 5 > ; \ assuming not solved

\ init everything except the secret
: init-game ( -- )  0 to guesses  answer len '-' fill ;

\ start a new game with w as the secret
: new-game-with ( w -- )  init-game secret wmove ;

\ Initialize a new game and pick a random secret word.
: new-game ( -- )  #words random ww new-game-with ; new-game



( ===== TESTS ===== )

TESTING +ANSWER
init-game
T{ w ABCDE s GY-G- +answer  w A--D- answer w= -> true }T
T{ w LMNOP s -G--- +answer  w AM-D- answer w= -> true }T
T{ w VWXYZ s ----- +answer  w AM-D- answer w= -> true }T

TESTING GREENS
T{ w ----- answer wmove     greens -> 0 }T
T{ w -A-B- answer wmove     greens -> 2 }T
T{ w WINDY answer wmove     greens -> 5 }T
