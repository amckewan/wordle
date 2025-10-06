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
    working @ begin >r
        over r@ cell+ swap score  cells scored +  1 swap +!  1+
    r> @ ?dup 0= until nip ;

\ calculate the entropy for a guess against all the words in the working set
: entropy ( guess -- ) ( F: -- entropy )
    score-working ( #words ) 0e ( entropy ) 
    #scores 0 do
        i cells scored + @ ( words with this score )
        ?dup if  over probability  fdup ibits f*  f+  then
    loop drop ;

\ find the word from the working set with the highest entropy
: max-entropy ( -- w )  0 ( default ) 0e ( entropy )
    working @ begin
        dup cell+ entropy  fover fover f< if  nip dup  fswap  then  fdrop
    @ ?dup 0= until cell+ fdrop ;

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
variable fence   \ use a different guesser if fewer than fence words left
: entropy-guess ( -- w )
    guesses 0= if ( shortcut ) first-guess exit then
    remaining 1 = if ( can't use entropy ) simple-guess exit then
    remaining fence @ < if tally-guess exit then
    \ use all words for the second guess
    \  guesses 1 = if max-entropy exit then
\     #working fence < if max-entropy exit then
\   #working fence
    max-entropy ;
