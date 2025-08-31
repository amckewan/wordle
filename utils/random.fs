( Simple pseudo-random number generator )

\ From https://en.wikipedia.org/wiki/Linear_congruential_generator#

\ Use linear congruential generator
\ X(n+1) = X(n) * A + C
\ Numerical Recipes ranqd1, Chapter 7.1, §An Even Quicker Generator, Eq. 7.1.6
\ parameters from Knuth and H. W. Lewis	A=1664525 C=1013904223

variable seed     time&date * * * * * seed ! ( cheesy seed )

: rnd ( -- n )  seed @ 1664525 um* drop  1013904223 +  dup seed ! ;

: random ( max -- n )  rnd um* nip ;
