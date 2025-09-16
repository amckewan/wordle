( letter tallies )

\ The number of occurances of each letter at each position
create tallies  32 len * cells allot

: clear-tallies  tallies 32 len * cells erase ;  clear-tallies
: tally ( l pos -- a )  32 * + cells tallies + ;

: word-tally ( w -- n )
    0  len 0 do  over i get i tally @ +  loop nip ;

: .tallies  len 0 do  cr i 0 .r ." : "
      a-z do  i j tally @  ?dup if i l>c emit ." =" . then  loop
    loop ;

\ TALLY-GUESS: pick the word with the largest letter tally
: tally-word ( w -- )  len 0 do  1 over i get i tally +!  loop drop ;
: tally-guessing ( just the current guessing words )  clear-tallies
    #words 0 do  i guess? if  i ww tally-word  then loop ;
: tally-guess ( -- w )
    tally-guessing  0 ww 0 ( w tally )
    #words 0 do  i guess? if
        i ww word-tally 2dup < if ( replace ) nip nip i ww swap else drop then
    then loop drop ;
