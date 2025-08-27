( game UI )

: solved ( -- f )  #greens len = ;
: failed ( -- f )  guesses @ max-guesses < not ;

: NEW  new-game  clear-history ;

: G ( -- ) \ "G RAISE" etc.
    w score-word  save-guess  cr .history
    solved if ." You WIN! " else
    failed if ." You LOSE! " then then ;

: H .history ;

\ \ Validate guesses (warnings for now)
\ : check-guess ( w -- )
\     valid-guess not if ." Not in the word list. " then \ abort" Not in the word list"
\     guess# @ 5 > if ." Too many guesses. " then \ abort" Too many guesses"
\     ;

\ \ Make a guess (with checks)
\ : make-guess ( w -- )
\     dup check-guess  1 guess# +!  score-word ;

