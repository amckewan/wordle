( entropy guesser )

\ Using the ideas from "Solving Wordle using information theory"
\ https://www.youtube.com/watch?v=v68zYyaEmEA
\
\ P = probability that we get a particular score
\ I = information bits, I = log2(1/P) or -log2(P)
\ e.g. P = 1/16, I = 4
\
\ Entropy = sum (P*I) over all possible scores

: flog2 ( F: f -- log2_f )  flog [ 2e flog ] fliteral f/ ;

: probability ( n total -- ) ( F: -- prob )  swap s>f s>f f/ ;
: ibits ( F: prob -- ibits )  flog2 fnegate ;

\ for each score, how many words from the working set got that score
create scored   #scores cells allot

: .scored  0  #scores 0 do  i 6 mod 0= if cr then
      i s.  i cells scored + @ dup 3 .r 3 spaces +
    loop cr ." Total: " . ;

\ update scored for each word in the working set
: score-working  ( guess -- #words )
    scored #scores cells erase   0 ( #words )
    for-working do i c@ if
        over i >ww swap score  cells scored +  1 swap +!  1+
    then loop nip ;

: score-all  ( guess -- )
    scored #scores cells erase
    #guess-words 0 do
        i ww over score  cells scored +  1 swap +!
    loop drop ;

: entropy ( guess -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\  : working-entropy  score-working (entropy) ;
\  : all-entropy  score-all #words entropy ;

\ find the word with the highest entropy
: max-entropy ( #words -- w )  0 ww ( default ) 0e ( entropy )
    swap 0 do  i ww entropy
        fover fover f< if  drop i ww  fswap  then  fdrop
    loop fdrop ;

\ just pick guesses from the working set
: max-working-entropy ( -- w )  0 ww  0e
    for-working do i c@ if  i >ww entropy
        fover fover f< if  drop i >ww  fswap  then  fdrop
    then loop fdrop ;

\ it takes almost 3 seconds for the first guess, precalculate for a speedup
\ since it's always the same
1 [if]
\ use a known good (best?) starting word
wordle first-guess   w salet first-guess wmove
[else]
cr .( calculating first entropy guess... )
wordle first-guess
init-solver #words max-entropy first-guess wmove
[then]

\ find word with the highest entropy
4 value fence   \ use a different guesser if fewer than fence words left
: entropy-guesser ( -- w )
    guesses 0= if ( shortcut ) first-guess exit then
    #working 1 = if ( can't use entropy ) simple-guesser exit then
    #working fence < if max-working-entropy exit then
    #words max-entropy ;


0 [if]
\ starting with RAISE
    1 Solved in 1 
   52 Solved in 2 
 1100 Solved in 3 
 1044 Solved in 4 
  109 Solved in 5 
    9 Solved in 6 
    0 Failed 
Average: 3.53    224.815 sec

\ starting with SALET
    0 Solved in 1 
   75 Solved in 2 
 1117 Solved in 3 
 1038 Solved in 4 
   82 Solved in 5 
    3 Solved in 6 
    0 Failed 
Average: 3.49    248.144 sec

\ working-only not so good (but fast!)
true to working-only  ok
timing on  ok
solver 
    0 Solved in 1 
  148 Solved in 2 
 1021 Solved in 3 
  951 Solved in 4 
  172 Solved in 5 
   22 Solved in 6 
    1 Failed 
Average: 3.52    12.596 sec

\ Use max-working-entropy below the fence (slightly better than tally)
    0 Solved in 1 
   75 Solved in 2 
 1118 Solved in 3 
 1039 Solved in 4 
   80 Solved in 5 
    3 Solved in 6 
    0 Failed 
Average: 3.48    249.131 sec

\ Using simple-guess at 1
\ Use max-working-entropy if < 4
\ Otherwise max-entropy
    0 Solved in 1 
   75 Solved in 2 
 1118 Solved in 3 
 1039 Solved in 4 
   80 Solved in 5 
    3 Solved in 6 
    0 Failed 
Average: 3.48    245.206 sec 

\ salet still rules
w trace first-guess wmove solver 
    1 Solved in 1 
   77 Solved in 2 
 1106 Solved in 3 
 1039 Solved in 4 
   87 Solved in 5 
    5 Solved in 6 
    0 Failed 
Average: 3.49    249.583 sec

[then]
