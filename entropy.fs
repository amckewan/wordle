( entropy guesser )

\ ----------------------------------------------------------------
\ Using the ideas from "Solving Wordle using information theory"
\ https://www.youtube.com/watch?v=v68zYyaEmEA
\
\ P = probability that we get a particular score
\ I = information bits, I = log2(1/P) or -log2(P)
\ e.g. P = 1/16, I = 4
\
\ Entropy = sum (P*I) over all possible scores
\ ----------------------------------------------------------------

: flog2 ( F: f -- log2_f )  flog [ 2e flog ] fliteral f/ ;

: probability ( n total -- ) ( F: -- prob )  swap s>f s>f f/ ;
: ibits ( F: prob -- ibits )  flog2 fnegate ;

\ for each score, how many words from the working set got that score
create scored   #scores cells allot

: .scored  0 0  #scores 0 do i cells scored + @ ?dup if  
      over 6 mod 0= if cr then swap 1+ swap
      i s.  dup 3 .r 3 spaces  +
    then loop cr ." Total: " . drop ;

\ update scored for each word in the working set
: score-working  ( guess -- #words )
    scored #scores cells erase   0 ( #words )
    for-working do i c@ if
        over i >ww swap score  cells scored +  1 swap +!  1+
    then loop nip ;

\ calculate the entropy for a guess against all the words in the working set
: entropy ( guess -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word from the working set with the highest entropy
: max-entropy ( -- w )  0 ww ( default ) 0e ( entropy )
    for-working do i c@ if
        i >ww entropy  fover fover f< if  drop i >ww  fswap  then  fdrop
    then loop fdrop ;

\ find the word with the highest entropy from all words
: max-entropy-all ( -- w )  0 ( default ) 0e ( entropy )
    #words 0 do
        i ww entropy  fover fover f< if  drop i  fswap  then  fdrop
    loop fdrop ww ;

\ It takes time for the first guess which is always the same
\   hidden on  init time max-entropy w.  2.189 sec raise
\   hidden off init time max-entropy w. 70.853 sec tares
\ Whe we consider all words (hidden off), "tares" has higher entropy
\   w raise entropy f. 5.91973684251619
\   w tares entropy f. 6.19405254437545
\ The proven best first word is "salet", which we don't prize quite as much:
\   w salet entropy f. 6.01684287539826

wordle first-guess   w tares   first-guess wmove

\  cr .( calculating first entropy guess... )
\  hidden off init-solver max-entropy first-guess wmove

\ guess the word with the highest entropy
\  4 value fence   \ use a different guesser if fewer than fence words left
: entropy-guess ( -- w )
    guesses 0= if ( shortcut ) first-guess exit then
    #working remaining 1 = if ( can't use entropy ) simple-guess exit then
    \ use all words for the second guess
    \  guesses 1 = if max-entropy exit then
\     #working fence < if max-entropy exit then
\   #working fence
    max-entropy ;






0 [if]
: score-all  ( w -- )  scored #scores cells erase
    #words 0 do
        i ww over score  cells scored +  1 swap +!
    loop drop ;

: entropy-all ( w -- ) ( F: -- entropy )  score-all  0e ( entropy ) 
    scored #scores bounds do
        i @ ?dup if  #words probability  fdup ibits f*  f+  then
    cell +loop ;

\ find the word from the working set that has the highest entropy
: max-entropy-all ( -- w )  0 ww ( default ) 0e ( entropy )
    working #words bounds do i c@ if
        i >ww entropy-all fover fover f< if  drop i >ww  fswap  then  fdrop
    then loop fdrop ;


\  : working-entropy  score-working (entropy) ;
\  : all-entropy  score-all #words entropy ;

\ find the word with the highest entropy
: max-entropy ( #words -- w )  0 ww ( default ) 0e ( entropy )
    swap 0 do  i ww entropy
        fover fover f< if  drop i ww  fswap  then  fdrop
    loop fdrop ;
[then]

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

\ Use max-entropy below the fence (slightly better than tally)
    0 Solved in 1 
   75 Solved in 2 
 1118 Solved in 3 
 1039 Solved in 4 
   80 Solved in 5 
    3 Solved in 6 
    0 Failed 
Average: 3.48    249.131 sec

\ Using simple-guess at 1
\ Use max-entropy if < 4
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

use entropy-guess ( best so far )
#words to #working ( working set size )
endgame off
time solver 
    0 Solved in 1 
   64 Solved in 2 
  828 Solved in 3 
 1018 Solved in 4 
  323 Solved in 5 
   73 Solved in 6 
    9 Failed 
Average: 3.77 183.830 sec

[then]
