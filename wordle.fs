( Wordle solver )

( Build an array of valid Wordle words, 5 chars per entry )

5 CONSTANT LEN

: WW,  HERE LEN ALLOT  BL WORD COUNT  ( a' a # )  ROT SWAP CMOVE ;

CREATE WORDLE-WORDS   INCLUDE wordle-words.fs

HERE WORDLE-WORDS - LEN / CONSTANT #WORDS

: WW ( n - a)  LEN * WORDLE-WORDS + ;

: .WW ( n -)  WW LEN TYPE SPACE ;

( show all the words )
: .WORDS  #WORDS 0 DO  I .WW  LOOP ;

( lookup a word using binary search )
: WW? ( a n -- false | w# true )
  5 <> IF DROP FALSE EXIT THEN  >R  0 #WORDS ( low high )
  BEGIN 2DUP < WHILE
    2DUP + 2/ ( low high mid )
    DUP WW  R@ LEN  ROT LEN COMPARE
    DUP 0= IF ( found ) R> 2DROP NIP NIP TRUE EXIT  THEN
    0< IF ( bottom half ) NIP  ELSE  ( top half ) ROT DROP  1+ SWAP  THEN
  REPEAT
  2DROP R> DROP FALSE ;

: TEST-WW?
  ( valid words )
  S" ABACK" WW? 0= ABORT" can't find ABACK" DROP
  S" ABASE" WW? 0= ABORT" can't find ABASE" DROP
  S" CADET" WW? 0= ABORT" can't find CADET" DROP
  S" RANCH" WW? 0= ABORT" can't find RANCH" DROP
  S" ZESTY" WW? 0= ABORT" can't find ZESTY" DROP
  S" ZONAL" WW? 0= ABORT" can't find ZONAL" DROP
  ( invalid words )
  S" XXXXX" WW? ABORT" found XXXXX"
  S" ABAC" WW? ABORT" found ABAC"
  S" ABACKX" WW? ABORT" found ABACKX" ;
  