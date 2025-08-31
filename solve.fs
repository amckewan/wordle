( solver )

: round  make-guess  score-word  save-history ;

\ : intro cr ." === Guess " guesses @ 1+ . ." Working " #working . ." === " ;

: solve? ( -- f )
    all-words clear-history
    begin
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
    #guesses 1+ 1 do  cr i >result @ 4 .r ."  Solved in " i . loop
    cr results @ 4 .r ."  Failed " ;
: solver 
    results #guesses 1+ cells erase
    #words 0 do
        i ww secret w!
        solve? if  1 guesses @ >result +!  else  1 results +!  then
    loop .results ;
