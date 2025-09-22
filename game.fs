\ Wordle game

\ The score is a wordle word containing these letters:
'G' '@' - constant GREEN
'Y' '@' - constant YELLOW
        0 constant GREY     ( displayed as '-')

\ Game data (answer and greens maintained for solver convenience)
0 value secret    ( the secret answer )
0 value answer    ( the answer we are building up, letter or 0 )
0 value greens    ( # of greens in answer )

\ Per-round data, set by make-guess (along with updating answer and greens)
0 value guess     ( current guess )
0 value score     ( score for the guess, string of colors )
0 value used      ( true if position used to satisfy a yellow score -internal)

: .game  ." secret: " secret w. ." answer: " answer w.
         ." guess: "  guess w.  ." score: "  score w. ;

: clear-score ( -- )  0 to score  0 to used ;
: random-word ( -- w )  #words random ww ;

: init-game ( -- ) \ init everything except the secret
    0 to answer  0 to greens  0 to guess  clear-score ;
: new-game ( -- )
    init-game  random-word to secret ; new-game
