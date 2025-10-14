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
: score-working  ( w -- #words )
    scored #scores cells erase   0 ( #words )
    working @ begin >r
        over r@ swap score  cells scored +  1 swap +!  1+
    r> next? until nip ;

\ calculate the entropy for a guess against all the words in the working set
: entropy ( w -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word from the working set with the highest entropy
\  : biggest ( w1 w2 -- w1 w2 | w2 w2 ) ( F: e -- e' )
\      dup entropy  fover fover f< if  nip dup  fswap  then  fdrop

: max-entropy ( -- w )    
    0e ( entropy ) working @ ( default ) dup begin
        dup entropy  fover fover f< if  nip dup  fswap  then  fdrop
    next? until fdrop ;

\ find the word with the highest entropy from all words
: max-entropy-all ( -- w )
    0e ( entropy ) working @ ( default ) #words 0 do
        i entropy  fover fover f< if  drop i  fswap  then  fdrop
    loop fdrop ;

\ It takes time for the first guess which is always the same
\   hidden on  init time max-entropy w.  2.189 sec raise
\   hidden off init time max-entropy w. 70.853 sec tares
\ When we consider all words (hidden off), "tares" has higher entropy
\   w raise entropy f. 5.91973684251619
\   w tares entropy f. 6.19405254437545
\ The proven best first word is "salet", which we don't prize quite as much:
\   w salet entropy f. 6.01684287539826

: first-guess  hidden @ if [w] raise else [w] tares then ;

\  cr .( calculating first entropy guess... )
\  wordle first-guess
\  init-solver max-entropy first-guess wmove

\ guess the word with the highest entropy
variable fence
variable allon2
: entropy-guess ( -- w )
    remaining 2 < if ( can't use entropy ) simple-guess exit then
    guesses 0= if ( shortcut ) first-guess exit then
    guesses 1 = allon2 @ and if max-entropy-all exit then
    \ entropy is ineffective when the working set gets too small
    remaining fence @ u<  guesses 5 < and if max-entropy-all exit then
    max-entropy ;


0 [if]
( ===== RESULTS =====)
fence off

endgame off hidden off solver
    0 Solved in 1 
   64 Solved in 2 
  828 Solved in 3 
 1022 Solved in 4 
  313 Solved in 5 
   71 Solved in 6 
   17 Failed 
Average: 3.75    146.995 sec

endgame off hidden on solver  ( using raise )
    1 Solved in 1 
  131 Solved in 2 
  990 Solved in 3 
  926 Solved in 4 
  208 Solved in 5 
   48 Solved in 6 
   11 Failed 
Average: 3.57    7.684 sec

endgame on hidden off solver 
    0 Solved in 1 
   64 Solved in 2 
  828 Solved in 3 
 1018 Solved in 4 
  319 Solved in 5 
   75 Solved in 6 
   11 Failed 
Average: 3.77    148.061 sec

endgame on hidden on solver ( using raise )
    1 Solved in 1 
  131 Solved in 2 
  988 Solved in 3 
  928 Solved in 4 
  218 Solved in 5 
   46 Solved in 6 
    3 Failed 
Average: 3.58    7.878 sec

( tally guess is faster and almost as good )
use tally-guess solver 
    1 Solved in 1 
  145 Solved in 2 
  861 Solved in 3 
 1006 Solved in 4 
  264 Solved in 5 
   34 Solved in 6 
    4 Failed 
Average: 3.63    2.588 sec

[then]
