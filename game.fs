\ Wordle game

\ Game data
wordle secret       ( the secret word we are trying to guess )
0 value guesses     ( number of guesses so far, 0-6 )

6 constant #guesses ( set by game )

\ Maintain partial answer for solver convenience, letter or '-'
wordle answer
: +answer ( guess score -- ) \ record any green letters in answer
    len 0 do  3 /mod swap green = if  over i + c@  i answer + c!
    then loop 2drop ;
: greens ( -- n )  0  answer len + answer do  i c@ '-' = 1+ +  loop ;

: .secret  secret w. ;
: .game ." secret: " .secret ." answer: " answer w. ." guesses: " guesses . ;

: solved ( score -- f ) [s] ggggg = ;
: failed ( -- f )       guesses 5 > ; ( assuming not solved )

\ init everything except the secret
: init-game ( -- )  0 to guesses  answer len '-' fill ;

\ start a new game with w as the secret
: new-game-with ( w -- )  init-game secret wmove ;

\ Initialize a new game and pick a random secret word.
: new-game ( -- )  #hidden random ww new-game-with ; new-game

: score-guess ( guess -- score )  secret swap score ;



( ===== TESTS ===== )
testing +answer
init-game
t{ w abcde s gy-g- +answer  w a--d- answer w= -> true }t
t{ w lmnop s -g--- +answer  w am-d- answer w= -> true }t
t{ w vwxyz s ----- +answer  w am-d- answer w= -> true }t

testing greens
t{ w ----- answer wmove     greens -> 0 }t
t{ w -a-b- answer wmove     greens -> 2 }t
t{ w windy answer wmove     greens -> 5 }t
