( solver )

: round  make-guess  score-word  save-history ;

\ : intro cr ." === Guess " guesses @ 1+ . ." Working " #working . ." === " ;

: solve? ( -- f )
    all-words clear-history
    begin
        \ cr ." Round " guesses @ 1+ . ." #working=" #working .
        round
        solved if true  exit then
        failed if false exit then
        prune
    again ;

: solve  solve? .history if ." Solved " else  ." Failed " then ;

\ shorthand
: r round .history ;
: p prune #working . ;

\ exhaustive test
create results  #guesses 1+ cells allot
: >result  cells results + ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed " ;
variable explain
: solver ( try all words )
    results #guesses 1+ cells erase
    #words 0 do
        i ww secret w!
        solve? if guesses @ >result else explain @ if .history then results then 1 swap +!
    loop .results ;
