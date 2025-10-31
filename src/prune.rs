// prune

use crate::{score::{Score, GREEN}, words::{ww, Word}};

// Return true if we should prune target based on the game result
pub fn prune(target: Word, result: (Word, Score)) -> bool {
    let mut g: Vec<char> = ww(result.0).chars().collect();
    let mut t: Vec<char> = ww(target).chars().collect();
    let score: Score = result.1;

    // : prune-green? ( score -- f )
    // for-scoring do
    //     3 /mod swap green = if
    //         ( prune if guess and target don't match )
    //         i 2@ - if  drop true  unloop exit  then
    //         -1 -2 i 2! ( ok, mark letter used )
    //     then
    // 2 cells +loop   drop false ;

    // prune greens
    let mut s = score;
    for i in 0..5 {
        if s % 3 == GREEN {
            if g[i] != t[i] {
                return true;
            }
            g[i] = 1 as char;
            t[i] = 2 as char;
        }
        s /= 3;
    }
    false
}
