( take a guess )

\ Pick the first word from the working set
: simple-guess ( -- w )  working @ cell+ ;

\ Pick a random word from the working set
: random-guess ( -- w )  working remaining random -1 do @ loop cell+ ;

\ Pick the word with the largest letter tally.
\ We tally the working set every round.
: tally-guess ( -- w )
    tally-working \ guesses 0= if tally-working then
    wordle-words cell+ ( w )  0 ( tally )
    \  hidden @ if for-hidden-words else for-all-words then do
    for-all-words do
      i tally 2dup < if ( replace ) nip nip i swap else drop then
    wsize +loop drop ;

\ Use different algorithms
' simple-guess value guesser
: use  ' to guesser ;
