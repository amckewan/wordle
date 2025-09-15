( make a guess )

\ A copy of the working set that we whittle down to pick a guess
create guessing  #words allot   guessing #words 1 fill ( for test )

\ Initialize for each round of guessing
: start-guess  working guessing #words cmove ( copy working set ) ;

: guess? ( n -- f )  guessing + c@ ;
: -guess ( n -- )    guessing + 0 swap c! ;

: #guessing ( -- n )  0 #words 0 do i guess? + loop ;
: .guessing  0  #words 0 do i guess? if i ww w. 1+ then loop  . ." words " ;

\ FIRST-GUESS: pick the first word (for repeatable tests)
: first-guess ( -- w )
    0 #words 0 do i guess? if drop i leave then loop ww ;

\ RANDOM-GUESS: pick a random word from the guessing set
: random-guess ( -- w )
    #guessing random  #words 0 do
      i guess? if  1- dup 0< if drop i leave  then then
    loop  ww ;

\ The number of occurances of each letter
create tallies  32 cells allot
: clear-tally  tallies 32 cells erase ;
: tally ( l -- a )  cells tallies + ;

: .tallies  a-z do  i tally @  ?dup if i l>c emit ." =" . then  loop ;

: tally-word ( w -- )  len 0 do  1 over i get tally +!  loop drop ;
: tally-guessing ( -- )  clear-tally
    #words 0 do  i guess? if  i ww tally-word  then loop ;

\ TALLY-GUESS: pick the word with the largest letter tally
: word-tally ( w -- n )
    tallies pad 32 cells move ( work on a copy, we will change as we go )
    0 len 0 do ( w n )
        over i get cells pad +  dup @  0 rot ! ( don't count again )  +
    loop nip ;
: tally-guess ( -- w )
    tally-guessing  0 ww 0 ( w tally )
    #words 0 do  i guess? if
        i ww word-tally 2dup < if ( replace ) nip nip i ww swap else drop then
    then loop drop ;

\ TRIM-GUESS: trim guesses a letter at a time
\ Pick the words with the most popular first letter, then from those pick
\ the words with the most popular second letter, etc.
: tally-letter ( pos -- )  clear-tally
    #words 0 do  i guess? if  i ww over get tally  1 swap +!  then loop drop ;
: most-popular ( -- l )
    1 ( max ) a-z do  i tally @ over tally @ > if  drop i  then loop ;
: trim ( pos -- )  dup tally-letter most-popular
    #words 0 do  i guess? if
        ( pos c ) 2dup i ww rot get <> if i -guess then
    then loop 2drop ;
: trim-guess ( -- w )  len 0 do i trim loop  first-guess ;


\ FIXED-GUESS: start with RAISE and COUNT
: fixed-guess ( -- w )
    guesses 0=  if [w] RAISE else
    guesses 1 = if [w] COUNT else
    tally-guess then then ;



0 [if]

\ ========================================================
\ Find a method to "weigh" each word to find the best guess.
\ 1. Word tally - sum of frequencies of each letter in the word
\ 2. Unique letters - words with many unique letters are good early
\ 3. Vowels - try them all early (or get at least two vowels)
\ 4. Avoid the guessing game (3-4 greens, many words that fit)

create scratch #words allot

: #unique ( w -- n ) \ #  unique letters in a word ABCDD -> 4
    scratch 32 1 fill  0 swap len bounds do
        i c@ A - scratch + ( n a )  dup c@ rot + swap ( update unique )  0 swap c! ( mark it )
    loop ;

create vowels 32 allot  vowels 32 erase
: >vowel ( c -- a )  A - vowels + ;
1 char A >vowel c!  1 char E >vowel c!  1 char I >vowel c!
1 char O >vowel c!  1 char U >vowel c!

: #vowels ( w -- n )  0 swap len bounds do i c@ >vowel c@ + loop ;
T{ w AEIOU #vowels -> 5 }T
T{ w EEEEE #vowels -> 5 }T
T{ w BMNFG #vowels -> 0 }T
T{ w ABCDE #vowels -> 2 }T

create counts 6 cells allot
: count-vowels  counts 6 cells erase  #words 0 do i has if 1 i ww #vowels cells counts + +! then loop ;
: .vowels  len 1+ 0 do i cells counts + ? loop ;

: scored-vowels ( -- n ) \ how many vowels got a green or yellow score
    0  len 0 do  i guess l@ >vowel c@  grey scored not and  +  loop ;
T{ w ABCDE to guess  w ----Y to score scored-vowels -> 1 }T

create used-letters 32 allot
: mark-used ( w -- )  used-letters 32 erase
    len bounds do  i c@ A - used-letters +  1 swap c!  loop ;
: #used ( w -- n )
    0 swap len bounds do i c@ A - used-letters + c@ + loop ;
T{ w ABCDE mark-used w ABCDE #used -> 5 }T
T{ w FGHIJ #used -> 0 }T
T{ w MMAMM #used -> 1 }T

\ First guess, find a word with 5 unique letters, at least 3 vowels and the biggest tally
: first ( -- w )  tally-guessing    0 ww 0 ( w tally )
    #words 0 do
        i ww #unique 5 = if
            i ww #vowels 2 > if
                i ww tally 2dup < if ( replace ) nip nip i ww swap else drop then
        then then
    loop drop ;

\ 2nd guess we try 5 new letters, try to get at least two vowels
: second ( -- w )  tally-guessing ( good? )  guess mark-used 
    0 ww 0 ( w tally )  #words 0 do  i guess? if
        over #used 0=   i ww #unique 5 = and if
            i ww #vowels 1 > if
                i ww tally 2dup < if ( replace ) nip nip i ww swap else drop then
        then then
    then loop drop ;

: weighted-guess ( -- w )
    guesses 0=  if first else tally-guess then ;


0 [if]
\ Try different algs for each round
\ First round a high-scoring word with 5 different letters and at least two vowels

create word-tally #words cells allot

: tally'em
    word-tally #words cells erase  tally-guessing
    #words 0 do i has if
        i ww tally  i cells word-tally + !
    then loop ;

: find-largest-unique ( -- w# )
    0 0 ( w# tally ) #words 0 do i has if
        i ww  dup #unique len = if       
            tally over > if  2drop  i dup ww tally  then
        else drop then
    then loop drop ;

: better ( w u t w2 -- f )
    >r  over r@ #unique < if 

    then ;

: find-largest-unique2 ( -- w )
    0 ww 0 0 ( w #unique tally ) #words 0 do i has if
        \ if this word is more unique, take it regardless of tally
        >r over #unique over > r> swap ( w u t f )

        i ww better if  2drop drop  i ww dup #unique over tally  then
    then loop drop ;


: weight ( w -- n )
    tally ; 
[then]

\ ================ Try different algorithms ================

[then]
variable guesser
: make-guess ( -- w )  start-guess  guesser @ execute ;

: use ' guesser ! ;


use random-guess

