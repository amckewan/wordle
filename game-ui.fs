( game UI )

: NEW  new-game  clear-history ;

: G ( -- ) \ "G RAISE" etc.
    w score-word  save-guess  cr .history
    #greens len = if ." You WIN! " else
    guesses @ max-guesses = if ." You LOSE! " then then ;



\ \ Validate guesses (warnings for now)
\ : check-guess ( w -- )
\     valid-guess not if ." Not in the word list. " then \ abort" Not in the word list"
\     guess# @ 5 > if ." Too many guesses. " then \ abort" Too many guesses"
\     ;

\ \ Make a guess (with checks)
\ : make-guess ( w -- )
\     dup check-guess  1 guess# +!  score-word ;

