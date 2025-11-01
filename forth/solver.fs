( solver )

: init ( -- )  init-game  all-words ;

variable fails
: .failed fails @ if cr secret w. ." failed " fails @ 0> if .history cr then then ;

\ Try to solve the puzzle in 6 rounds, return true if we solved it.
: solve? ( -- f )  init
    begin  guess submit  solved not while
        failed if .failed false exit then
        prune
    repeat true ;

: solve  solve? .history if ." Solved " else ." Failed " then ;


\ exhaustive test, solve all puzzles and report results
#hidden value #tests
create results  #guesses 1+ cells allot  ( index 0 for failures )
: >result  cells results + ;
: +results ( solved? -- )  if guesses >result else results then 1 swap +! ;

: average ( -- n*100 )
    0  #guesses 1+ 1 do  i >result @ i * +  loop  100 #tests */ ;
: .## ( n -- )  0 <# # # '.' hold #s #> type space ;
: .results
    #guesses 1+ 1 do  cr i >result @ 5 .r ."  Solved in " i . loop
    cr results @ 5 .r ."  Failed "  cr ." Average: " average .## ;

: solve-all ( -- )
    results #guesses 1+ cells erase
    #tests 0 do  init  i to secret  solve? +results  loop ;
: solver ( -- )
    timestamp solve-all timestamp swap -  .results 3 spaces .elapsed ;
