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

: entropy ( guess -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word with the highest entropy
: max-entropy ( #words -- w )  0 ww  0e
    swap 0 do  i ww entropy
        fover fover f< if  drop i ww  fswap  then  fdrop
    loop fdrop ;

\ it takes almost 3 seconds for the first guess, precalculate for a speedup
\ since it's always the same (RAISE)
1 [if]
cr .( calculating first entropy guess... )
wordle first-guess
init-solver #words max-entropy first-guess wmove
[else]
: first-guess  [w] raise ; \ just do it once
[then]


\ find word in working set with the highest entropy
: entropy-guesser ( -- w )
    guesses 0= if first-guess exit then
    #working 1 = if simple-guesser exit then
    timing @ if timestamp >r then
    #words max-entropy
    timing @ if timestamp cr #working . ." words in " r> - . ." us " then ;



\ experimental...
\ just pick guesses from the working set
: max-working-entropy ( -- w )  0 ww  0e
    for-working do i c@ if  i >ww entropy
        fover fover f< if  drop i >ww  fswap  then  fdrop
    then loop fdrop ;

\ find word in working set with the highest entropy
: entropy-guesser2 ( -- w )
    #working 1 = if simple-guesser exit then
    max-working-entropy ;

0 [if]
=========================================
failing test cases
=========================================

use entropy-guesser  ok
solver 

mecca failed
1 raise 
  -Y--Y 
2 cleat 
  Y-YY- 
3 aback 
  Y--G- 
4 aback 
  Y--G- 
5 aback 
  Y--G- 
6 aback 
  Y--G- 
ninja failed

1 raise 
  -YY-- 
2 until 
  -Y-Y- 
3 aback 
  Y---- 
4 aback 
  Y---- 
5 aback 
  Y---- 
6 aback 
  Y---- 
pupal failed

1 raise 
  -Y--- 
2 clout 
  -Y-Y- 
3 aback 
  Y---- 
4 aback 
  Y---- 
5 aback 
  Y---- 
6 aback 
  Y---- 


total failed
1 raise 
  -Y--- 
2 clout 
  -YY-Y 
3 again 
  Y---- 
4 aback 
  Y---- 
5 aback 
  Y---- 
6 aback 
  Y---- 
    1 Solved in 1 
   34 Solved in 2 
  829 Solved in 3 
 1291 Solved in 4 
  153 Solved in 5 
    3 Solved in 6 
    4 Failed 
Average: 3.67  ok
[then]
