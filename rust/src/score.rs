// scoring

use crate::words::{ww, Word};

pub type Score = u8;

pub const SCORES: usize = 243;

pub const GREY:   u8 = 0;
pub const YELLOW: u8 = 1;
pub const GREEN:  u8 = 2;

pub fn score(guess: Word, target: Word) -> Score {
    let mut guess: Vec<char> = ww(guess).chars().collect();
    let mut target: Vec<char> = ww(target).chars().collect();
    let mut score: Score = 0;
    let mut m;

    // score greens
    m = 1;
    for i in 0..5 {
        if guess[i] == target[i] {
            score += GREEN * m;
            guess[i] = 1 as char; // mark used
            target[i] = 2 as char;
        }
        m *= 3;
    }

    // score yellows
    m = 1;
    for i in 0..5 {
        // look for matching letter in target
        for j in 0..5 {
            if target[j] == guess[i] {
                score += YELLOW * m;
                target[j] = 2 as char; // mark used
                break;
            }
        }
        m *= 3;
    }

    score
}
