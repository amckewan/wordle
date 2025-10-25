\ Wordle game

0 value secret      ( the secret word we are trying to guess )

: score-guess ( guess -- score )  secret swap score ;

: solved ( score -- f ) [s] ggggg = ;
: failed ( -- f )       guesses 5 > ; ( assuming not solved )

\ Maintain greens for solver convenience, letter or '-'
create greens len allot
: #greens ( -- n )  len  greens len bounds do  i c@ '-' = +  loop ;
: +greens ( guess score -- )  swap ww swap
    len 0 do  3 /mod swap green = if  over i + c@  i greens + c!  then
    loop 2drop ;

\ Like the wordle-game's keyboard, record what we know about each letter
\ 0=unknown, 1=grey, 2=yellow, 3=green (k = color+1)
create kb 32 allot
: >kb ( c -- a )  31 and kb + ;
: +kb ( k c -- )  >kb 2dup c@ > if c! else 2drop then ;
: +keyboard ( guess score -- ) \ update kb with latest guess & score
    swap ww len bounds do  3 /mod swap ( color ) 1+ i c@ +kb  loop drop ;

: .game ." secret: " secret w. ." guesses: " guesses .
        ." greens: " greens len type space ;

\ Submit guess to the game, update game state and return the score
: submit ( guess score -- )  2dup +history  2dup +greens  +keyboard ;
: guess  ( guess -- score )  dup score-guess  tuck submit ;

\ Initialize everything except the secret (reset game)
: init-game ( -- )  0 to guesses  greens len '-' fill  kb 32 erase ;

\ Initialize a new game and pick a random secret word
: new-game ( -- )  init-game  #hidden random to secret ;



( ===== TESTS ===== )
testing +greens
init-game
t{ w raise s gy-g- +greens  s" r--s-" greens len compare -> 0 }t
t{ w smack s -g--- +greens  s" rm-s-" greens len compare -> 0 }t
t{ w pound s ----- +greens  s" rm-s-" greens len compare -> 0 }t

testing #greens
t{ s" -----" greens swap move   #greens -> 0 }t
t{ s" -i-d-" greens swap move   #greens -> 2 }t
t{ s" windy" greens swap move   #greens -> 5 }t
