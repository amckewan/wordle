( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )  working @ >w ;

\ Pick a random word from the working set
: random-guess ( -- w )  working  remaining random -1 do @ loop  >w ;

\ Pick the word with the largest letter tally.
\ We tally the working set every round.
: tally-guess ( -- w )
    tally-working \ guesses 0= if tally-working then
    0 ( w ) 0 ( tally )
    #working 0 do
        i tally 2dup < if ( replace ) nip nip i swap else drop then
    loop drop ;

\ Use different algorithms
' simple-guess value guesser
: use  ' to guesser ;
