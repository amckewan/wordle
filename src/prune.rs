// prune

use crate::{score::{Score, GREEN, GREY, YELLOW}, words::{ww, Word}};

// Return true if we should prune target based on the game answer
pub fn prune(target: Word, answer: (Word, Score)) -> bool {
    let mut target: Vec<char> = ww(target).chars().collect();
    let mut guess: Vec<char> = ww(answer.0).chars().collect();
    let score: Score = answer.1;

    if prune_greens(&mut target, &mut guess, score) {
        return true;
    }
    if prune_yellows(&mut target, &mut guess, score) {
        return true;
    }
    if prune_greys(&mut target, &mut guess, score) {
        return true;
    }
    false
}

fn prune_greens(target: &mut Vec<char>, guess: &mut Vec<char>, mut score: Score) -> bool {
    for i in 0..5 {
        if score % 3 == GREEN {
            if guess[i] != target[i] {
                return true;
            }
            guess[i] = 1 as char;
            target[i] = 2 as char;
        }
        score /= 3;
    }
    false
}

// : prune-yellow? ( score -- f )
//     for-scoring do
//         3 /mod swap yellow = if
//             i 2@ = if ( prune ) drop true unloop exit then
//             i @ find-match not if ( prune ) drop true unloop exit then
//             -1 swap ! ( mark )
//         then
//     2 cells +loop   drop false ;

fn prune_yellows(target: &mut Vec<char>, guess: &mut Vec<char>, mut score: Score) -> bool {
    for i in 0..5 {
        if score % 3 == YELLOW {
            if guess[i] == target[i] {
                return true;
            }
            match find_match(&target, guess[i]) {
                Some(pos) => target[pos] = 2 as char,
                None => return true,
            }
        }
        score /= 3;
    }
    false
}

fn prune_greys(target: &Vec<char>, guess: &Vec<char>, mut score: Score) -> bool {
    for i in 0..5 {
        if score % 3 == GREY {
            match find_match(&target, guess[i]) {
                Some(_) => return true,
                None => (),
            }
        }
        score /= 3;
    }
    false
}

fn find_match(target: &Vec<char>, c: char) -> Option<usize> {
    for i in 0..5 {
        if target[i] == c {
            return Some(i);
        }
    }
    None
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_prune_greens() {
        // assert!(prune());
    }
}
