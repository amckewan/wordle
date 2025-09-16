( weighted guess )

\ ========================================================
\ Find a method to "weigh" each word to find the best guess.
\ 1. Word tally - sum of frequencies of each letter in the word
\ 2. Unique letters - words with many unique letters are good early
\ 3. Vowels - try them all early (or get at least two vowels)
\ 4. Avoid the guessing game (3-4 greens, many words that fit)

create counts 6 cells allot ( # vowels, 0 to 5 )
: count-vowels ( -- )  counts 6 cells erase
    #words 0 do i has if 1 i ww vowels cells counts + +! then loop ;
: .vowels  len 1+ 0 do i cells counts + ? loop ;

: scored-vowels ( -- n ) \ how many vowels got a green or yellow score
    0  len 0 do  guess i get vowel  grey i scored not and  +  loop ;
T{ w ABCDE to guess  w ----Y to score scored-vowels -> 1 }T

create used-letters 32 allot
: mark-used ( w -- )  used-letters 32 erase
    len 0 do  1 over i get used-letters + c!  loop drop ;
: #used ( w -- n )
    0 len 0 do  over i get used-letters + c@ + loop nip ;
T{ w ABCDE mark-used w ABCDE #used -> 5 }T
T{ w FGHIJ #used -> 0 }T
T{ w MMAMM #used -> 1 }T

\ First guess: find a word with 5 unique letters,
\ at least 3 vowels and the biggest tally
: first-guess ( -- w )  tally-guessing    0 ww 0 ( w tally )
    #words 0 do
        i ww unique 5 = if
        i ww vowels 2 > if
        i ww tally 2dup < if ( replace ) nip nip i ww swap  else drop
        then then then
    loop drop ;

\ 2nd guess we try 5 new letters, try to get at least two vowels
: second-guess ( -- w )  tally-guessing ( good? )  guess mark-used
    0 ww 0 ( w tally )  #words 0 do  i guess? if
        over #used 0=   i ww unique 5 = and if
            i ww vowels 1 > if
                i ww tally 2dup < if ( replace ) nip nip i ww swap else drop then
        then then
    then loop drop ;

: weighted-guess ( -- w )
    guesses 0= if first-guess else tally-guess then ;

use weighted-guess

0 [if]
\ Try different algs for each round
\ First round a high-scoring word with 5 different letters and at least two vowels

create tally #words cells allot

: tally'em
    tally #words cells erase  tally-guessing
    #words 0 do i has if
        i ww tally  i cells tally + !
    then loop ;

: find-largest-unique ( -- w# )
    0 0 ( w# tally ) #words 0 do i has if
        i ww  dup unique len = if       
            tally over > if  2drop  i dup ww tally  then
        else drop then
    then loop drop ;

: better ( w u t w2 -- f )
    >r  over r@ unique < if 

    then ;

: find-largest-unique2 ( -- w )
    0 ww 0 0 ( w unique tally ) #words 0 do i has if
        \ if this word is more unique, take it regardless of tally
        >r over unique over > r> swap ( w u t f )

        i ww better if  2drop drop  i ww dup unique over tally  then
    then loop drop ;


: weight ( w -- n )
    tally ; 
[then]
