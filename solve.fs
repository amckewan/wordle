( solver )

: round  endgame not if make-guess then
         score-guess  add-history ;

: solve? ( -- f )
    all-words clear-history
    begin round solved not while
        failed if false exit then
        prune
    repeat true ;

: solve  solve? .history if ." Solved " else ." Failed " then ;

\ shorthand
: s all-words clear-history ;
: r round .history ;
: p prune #working . ;
: try new w to secret solve ;

\ exhaustive test
create results  #guesses 1+ cells allot
: >result  cells results + ;
: average ( -- n*100 )
    0  #guesses 1+ 1 do  i >result @ i * +  loop  100 #words */ ;
: .## ( n -- )  0 <# # # '.' hold #s #> type space ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed "
    cr ." Average: " average .## ;

: solver ( try all words )
    results #guesses 1+ cells erase
    #words 0 do
        new-game i ww to secret
        solve? if guesses >result else results then 1 swap +!
    loop .results ;

: solve-with ( xt -- )  to guesser solver cr ;
: try-all
    cr ." Using fixed-guess "       ['] fixed-guess     solve-with
    cr ." Using random-guess "      ['] random-guess    solve-with
    cr ." Using tally-guess "       ['] tally-guess     solve-with
    cr ." Using weighted-guess "    ['] weighted-guess  solve-with
;

