\ Wordle game

wordle secret   ( the secret word we are trying to guess )

: secret! ( w -- )  secret wmove ;

: score-guess ( guess -- score )  secret swap score ;

: solved ( score -- f ) [s] ggggg = ;
: failed ( -- f )       guesses 5 > ; ( assuming not solved )

\ Maintain greens for solver convenience, letter or '-'
wordle greens
: #greens ( -- n )  0  greens for-chars do  i c@ '-' = 1+ +  loop ;
: +greens ( guess score -- )
    len 0 do  3 /mod swap green = if  over i + c@  i greens + c!  then
    loop 2drop ;

\ Like the wordle-game's keyboard, record what we know about each letter
\ 0=unknown, 1=grey, 2=yellow, 3=green (k = color+1)
create kb 32 allot
: >kb ( c -- a )  31 and kb + ;
: +kb ( k c -- )  >kb 2dup c@ > if c! else 2drop then ;
: +keyboard ( guess score -- ) \ update kb with latest guess & score
    swap for-chars do  3 /mod swap ( color ) 1+ i c@ +kb  loop drop ;

: .secret  secret w. ;
: .game ." secret: " .secret ." guesses: " guesses . ." greens: " greens w. ;

\ Submit guess to the game, update game state and return the score
: submit ( guess score -- )  2dup +history  2dup +greens  +keyboard ;
: guess  ( guess -- score )  dup score-guess  tuck submit ;

\ Initialize everything except the secret (reset game)
: init-game ( -- )  0 to guesses  greens len '-' fill  kb 32 erase ;

\ Initialize a new game and pick a random secret word
: new-game ( -- )  init-game  #hidden random ww secret! ;



( ===== TESTS ===== )
testing +greens
init-game
t{ w abcde s gy-g- +greens  w a--d- greens w= -> true }t
t{ w lmnop s -g--- +greens  w am-d- greens w= -> true }t
t{ w vwxyz s ----- +greens  w am-d- greens w= -> true }t

testing #greens
t{ w ----- greens wmove     #greens -> 0 }t
t{ w -a-b- greens wmove     #greens -> 2 }t
t{ w windy greens wmove     #greens -> 5 }t
