( solver )

: round  endgame? if endgame-guess else make-guess then
         score-guess  add-history ;

: solve? ( -- f )
    all-words clear-history
    begin
        \ cr ." Round " guesses @ 1+ . ." #working=" #working .
        endgame? if cr secret w. ." endgame " then
        round
        solved if true  exit then
        failed if cr secret w. ." failed " .history cr false exit then
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
        new-game i ww secret w!
        1 solve? if guesses @ >result else results then +!
    loop .results ;

: solve-with ( xt -- )  guesser ! solver cr ;
: try-all
    cr ." Using random-guess "      ['] random-guess    solve-with
    cr ." Using tally-guess "       ['] tally-guess     solve-with
    cr ." Using trim-guess "        ['] trim-guess      solve-with
    cr ." Using fixed-guess "       ['] fixed-guess     solve-with
    cr ." Using weighted-guess "    ['] weighted-guess  solve-with
;
