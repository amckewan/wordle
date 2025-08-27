( solver )


: round  make-guess  score-word  save-guess ;

\ : G ( -- ) \ "G RAISE" etc.
\     w score-word  save-guess  cr .history
\     #greens len = if ." You WIN! " else
\     guesses @ max-guesses = if ." You LOSE! " then then ;

: r round .history ;
: p prune #working . ;

: solve
    all-words clear-history
    begin
        round .history
        solved if ." Solved " exit then
        failed if ." Failed " exit then
        prune
    again ;
