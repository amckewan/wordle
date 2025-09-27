( letter tallies )

\ The number of occurances of each letter at each position
create tallies  32 len * cells allot

: clear-tallies  tallies 32 len * cells erase ;  clear-tallies
: >tally ( l pos -- a )  32 * + cells tallies + ;

: tally ( w -- n )
    0  len 0 do  over i get i >tally @ +  loop nip ;

: .tallies  len 0 do  cr i 0 .r ." : "
      a-z do  i j >tally @  ?dup if i l>c emit ." =" . then  loop
    loop ;

: tally-word ( w -- )  len 0 do  1 over i get i >tally +!  loop drop ;
: tally-working  \ tally all words from the working set
    clear-tallies  working-size 0 do  i has if i ww tally-word then  loop ;

\ Pick the word with the largest letter tally (from all words)
: tally-guesser ( -- w )  tally-working  0 0 ( w# tally )
    working-size 0 do
      i ww tally 2dup < if ( replace ) nip nip i swap else drop then
    loop drop ww ;
