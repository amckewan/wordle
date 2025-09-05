( solver )

: round  make-guess  score-word  save-history ;

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
: s all-words clear-history ;
: r round .history ;
: p prune #working . ;

\ exhaustive test
create results  #guesses 1+ cells allot
: >result  cells results + ;
: average ( -- n*100 )
    0  #guesses 1+ 1 do  i >result @ i * +  loop  100 #words */ ;
: .## ( n -- )  0 <# # # [char] . hold #s #> type space ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed "
    cr ." Average: " average .## ;

: solver ( try all words )
    results #guesses 1+ cells erase
    #words 0 do
        i ww secret w!
        1 solve? if guesses @ >result else results then +!
    loop .results ;

1 [if]
cr .( Using random-guess ) use random-guess solver
cr .( Using tally-guess ) use tally-guess solver
cr .( Using trim-guess ) use trim-guess solver
cr .( Using fixed-guess ) use fixed-guess solver
cr .( Using weighted-guess ) use weighted-guess solver
[then]
