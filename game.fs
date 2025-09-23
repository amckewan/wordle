\ Wordle game

\ The score is a wordle word containing these letters:
'G' '@' - constant GREEN
'Y' '@' - constant YELLOW
        0 constant GREY     ( displayed as '-')

\ Game data (answer maintained for solver convenience)
0 value secret      ( the secret word we are trying to guess )
0 value answer      ( the answer we are building up, letter or 0 )
0 value guesses     ( number of guesses so far, 0-6 )

6 constant #guesses ( set by game )

: solved ( score -- f ) [w] GGGGG = ;
: failed ( -- f )       guesses 5 > ; \ assuming not solved

: +answer ( guess score -- ) \ record green letters in answer
    len 0 do  dup i get green = if
        over i get answer i put to answer
    then loop 2drop ;

: greens ( -- n )
    0  len 0 do  answer i get green = -  loop ;

\ Per-round data, set by make-guess (along with updating answer and greens)
0 value guess     ( current guess )
0 value score     ( score for the guess, string of colors )

: .game  ." secret: " secret w. ." answer: " answer w. ." guesses " guesses . ;
\         ." guess: "  guess w.  ." score: "  score w. ;

\ init everything except the secret
: init-game ( -- )  0 to answer  0 to guess  0 to score  0 to guesses ;

\ Initialize a new game and pick a random secret word.
: random-word ( -- w )  #hidden random  cells hidden-words + @ ;
: new-game ( -- )  init-game  random-word to secret ; new-game
