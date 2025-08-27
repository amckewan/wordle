( solver )

: round  make-guess  score-word  save-guess ;

: intro cr ." === Guess " guesses @ 1+ . ." Working " #working . ." === " ;

: solve
    all-words
    begin
        intro round .history
        solved if ." Solved " exit then
        failed if ." Failed " exit then
        prune
    again ;

\ shorthand
: r round .history ;
: p prune #working . ;

