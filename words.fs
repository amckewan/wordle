( Wordle words )

\ Wordle words are 5 characters long. We encode them in a 32-bit cell
\ with 5 bits per character, the first character in the low 5 bits.
\ 0=@, 1=A, 2=B, 3=C, etc. This is 'w' in the stack diagrams.
5 CONSTANT LEN    \ letters per word (fixed by game)
5 CONSTANT BITS   \ bits per letter (5+)

: A-Z ( -- limit index )  27 1 ; \ for do..loop over the alphabet

1 BITS LSHIFT 1- CONSTANT CMASK

: LEFT  ( n pos -- n' )  BITS * LSHIFT ;
: RIGHT ( n pos -- n' )  BITS * RSHIFT ;
: MASK  ( pos -- mask )  CMASK SWAP LEFT ;

\ Convert between letter (0-31) and ASCII char, use - instead of @ for zero
: C>L ( c -- l )  DUP '-' <> AND  CMASK AND ;
: L>C ( l -- c )  DUP IF '@' ELSE '-' THEN + ;

\ Encode ASCII string
: >W ( a -- w )  0 SWAP  LEN 0 DO  COUNT C>L I LEFT  ROT OR SWAP  LOOP DROP ;

( literals )
:  W  ( "w" -- w )  BL PARSE  LEN <> ABORT" need 5 letters"  >W ;
: [W] ( "w" -- w )  W  POSTPONE LITERAL ; IMMEDIATE

: W. ( w -- )  LEN 0 DO  DUP CMASK AND L>C EMIT  1 RIGHT  LOOP  DROP SPACE ;

\ Insert and extract a letter at pos. Letters (l) are 0-31 not ASCII
: GET ( w pos -- l )     RIGHT CMASK AND ;
: PUT ( l w pos -- w' )  DUP >R  MASK INVERT AND  SWAP R> LEFT OR ;

\ True if letters at pos match
: MATCH ( w1 w2 pos -- f )  >R XOR R> MASK AND 0= ;


( === TESTS === )
TESTING MASK PUT GET
T{ 0 MASK -> $1F }T
T{ 2 MASK -> $1F 10 LSHIFT }T
T{ 4 MASK -> $1F 20 LSHIFT }T
T{ 'X' '@' - W RAISE 0 PUT -> w XAISE }T
T{ 'X' '@' - W RAISE 2 PUT -> w RAXSE }T
T{ 'X' '@' - W RAISE 4 PUT -> w RAISX }T
T{ W RAISE 0 GET '@' + -> 'R' }T
T{ W RAISE 1 GET '@' + -> 'A' }T
T{ W RAISE 3 GET '@' + -> 'S' }T

TESTING MATCH
T{ w AAAAA w AAAAA 1 MATCH -> TRUE }T
T{ w AAAAA w AAAAA 3 MATCH -> TRUE }T
T{ w ABCDE w ABCED 1 MATCH -> TRUE }T
T{ w AAAAA w BBBBB 0 MATCH -> FALSE }T
T{ w AAAAA w BBBBB 2 MATCH -> FALSE }T
T{ w AAAAA w BBBBB 4 MATCH -> FALSE }T
T{ w ABCDE w ABCED 4 MATCH -> FALSE }T

