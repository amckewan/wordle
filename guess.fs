( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )  working @ ;

\ Pick a random word from the working set
: random-guess ( -- w )  working @  remaining random 0 ?do next loop ;

\ Pick the word with the largest letter tally.
: tally-guess ( -- w )
    tally-working  0 ( w ) 0 ( tally )
    working @ begin  dup >r tally ( w1 t1 t2 )
        2dup < if ( replace ) nip nip r@ swap else drop then  r>
    next? until drop ;


\ Use different algorithms
' simple-guess value guesser
: use  ' to guesser ;
