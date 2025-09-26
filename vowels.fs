( count vowels )

create vowels_ 32 allot  vowels_ 32 erase
1 'A' 31 and vowels_ + c!
1 'E' 31 and vowels_ + c!
1 'I' 31 and vowels_ + c!
1 'O' 31 and vowels_ + c!
1 'U' 31 and vowels_ + c!

: vowel ( c -- 0/1 )  31 and vowels_ + c@ ; ( close enough )

: vowels ( w -- n )  0 swap  for-chars do  i c@ vowel +  loop ;


TESTING VOWELS
T{ 'A' vowel -> 1 }T
T{ 'M' vowel -> 0 }T
T{ 'U' vowel -> 1 }T
T{ 'Z' vowel -> 0 }T

T{ w AEIOU vowels -> 5 }T
T{ w EEEEE vowels -> 5 }T
T{ w BMNFG vowels -> 0 }T
T{ w ABCDE vowels -> 2 }T
