( letter tallies )

\ The number of occurances of each letter at each position
\ 5 rows of 32, count for 'A' at offset 1.
32 cells len * constant tallies#
create tallies          tallies# allot

: >tally ( c row -- a )  swap 31 and cells + ;

: .tallies  len 0 do  cr i 0 .r ." : "  a-z do
    i j 32 * tallies + >tally @ ?dup if i emit ." =" . then loop loop ;

\ Init tallies from the working set
: tally-word ( w -- )  ww  tallies tallies# bounds do
    count i >tally  1 swap +!  32 cells +loop drop ;
: tally-working ( -- )
    tallies tallies# erase
    working @ begin  dup tally-word  next? until ;

\ get the tally for a word
: tally ( w -- n )   0 ( n ) swap ww
    tallies tallies# bounds do
        count i >tally @  rot + swap
    32 cells +loop drop ;

\ Guess the word with the largest letter tally
: tally-guess ( -- w )
    tally-working  0 ( w ) 0 ( tally )
    working @ begin  dup >r tally ( w1 t1 t2 )
        2dup < if ( replace ) nip nip r@ swap else drop then  r>
    next? until drop ;
