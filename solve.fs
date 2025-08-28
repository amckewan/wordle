( solver )

: round  make-guess  score-word  save-guess ;

\ : intro cr ." === Guess " guesses @ 1+ . ." Working " #working . ." === " ;

: solve
    all-words
    begin
        round
        solved if .history ." Solved " exit then
        failed if .history ." Failed " exit then
        prune
    again ;

\ shorthand
: r round .history ;
: p prune #working . ;

