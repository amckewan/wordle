( letter tallies )

\ The number of occurances of each letter at each position
\ 5 rows of 32, count for 'A' at offset 1.
32 cells constant tally-row
tally-row len * constant tallies-size
create tallies           tallies-size allot

tallies tallies-size bounds 2constant for-tallies ( -- limit index )

: clear-tallies  tallies tallies-size erase ;
: >tally ( c' pos -- a )  32 * + cells  tallies + ;

: tally-word ( w -- )
    for-tallies do  count 31 and cells i +  1 swap +!  tally-row +loop drop ;
: tally-all ( -- )
    clear-tallies  #words 0 do  i ww tally-word  loop ;
: tally-working ( -- ) \ tally the words in the working set
    clear-tallies  for-working do  i c@ if i >ww tally-word then  loop ;

: .tallies  len 0 do  cr i 0 .r ." : "
      27 1 do  i j >tally @  ?dup if i '@' + emit ." =" . then  loop
    loop ;

: tally ( w -- n )    0 ( n ) swap
    for-tallies do  count 31 and cells i +  @ +  tally-row +loop drop ;

\ Pick the word with the largest letter tally (from all words)
: tally-guesser ( -- w )  tally-working  0 0 ( w# tally )
    #wordles 0 do
      i ww tally 2dup < if ( replace ) nip nip i swap else drop then
    loop drop ww ;
