( game UI )


6 constant #guesses
variable guess# ( 0 to 5 )

: .guesses  ." (" guess# @ 0 .r ." ) ";
: .game  ." secret: " secret w. ." guess: " guess w. .guess# ." score: " score w. ;

\ \ array of guesses and solutions
\ create guesses  #guesses len 2* * allot

\ : guess ( -- w )  guesses  guess# @ len 2* * + ;
\ : score ( -- w )  guess len + ;

\ Validate guesses (warnings for now)
: check-guess ( w -- )
    valid-guess not if ." Not in the word list. " then \ abort" Not in the word list"
    guess# @ 5 > if ." Too many guesses. " then \ abort" Too many guesses"
    ;

\ Make a guess (with checks)
: make-guess ( w -- )
    dup check-guess  1 guess# +!  score-word ;

