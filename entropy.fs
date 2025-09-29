( entropy guesser )
\ using the ideas from https://www.youtube.com/watch?v=v68zYyaEmEA

\ information = log2(1/probablilty)     or I = -log2(P)
\ e.g. P = 1/16, I = 4

\ Entropy (H) = sum (P*I) over all possible scores

\ to get the probability distribution for a potential guess, we need
\ to score it against every word and count it up.

: flog2 ( F: f -- log2_f )  flog [ 2e flog ] fliteral f/ ;

: prob  ( total n -- ) ( F: -- probability )  s>f ?dup if s>f f/ then ;
: ibits ( F: prob -- ibits )  flog2 fnegate ;


\ entropy for each word, only valid for members of the working set
\  create entropies  #words floats allot
\  create probabilities  #words floats allot
\  create information  #words floats allot

\ for each score, how many words from the working set got that score
create scored   #scores cells allot

0 value total   \ # of words scored

\  : #scored ( score -- n )  cells scored + @ ;

: .scored  #scores 0 do  i 6 mod 0= if cr then
        i s.  i cells scored + @ 3 .r 3 spaces
    loop cr ." Total: " total . ;

\ update scored for each word in the working set
: score-working  ( w -- )  scored #scores cells erase   0 ( #words )
    for-working do i c@ if
        over i >ww swap score  cells scored +  1 swap +!  1+
    then loop to total drop ;

: entropy ( F: -- entropy ) \ using data from scored & totoal
    0e ( entropy ) #scores 0 do
        total i cells scored + @ ( # with this score )
        dup if  prob fdup ibits f* f+  else 2drop then
    loop ;
    
\ find word in working set with the highest entropy
: entropy-guesser ( -- w )  0 ww  0e
    for-working do i c@ if
        i >ww score-working  entropy
        fover fover f< if  drop i >ww  fswap  then  fdrop
    then loop fdrop ;
