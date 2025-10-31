// scoring

use crate::words::{ww, Word};

pub type Score = u8;

pub const SCORES: usize = 243;

pub const GREY:   u8 = 0;
pub const YELLOW: u8 = 1;
pub const GREEN:  u8 = 2;

fn color(c: char) -> u8 {
    3 - ((c as u8 >> 2) & 3)
}

pub fn from_string(s: &str) -> Score {
    let mut score = 0;
    let mut mult = 1;

    for c in s.chars() {
        score += color(c) * mult;
        mult = mult * 3;
    }

    score
}

pub fn to_string(mut score: Score) -> String {
    let rep = ['-','Y','G'];

    let mut out = String::new();
    for _ in 0..5 {
        out.push(rep[score as usize % 3]);
        score = score / 3;
    }
    out
}

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


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_color() {
        assert_eq!(color('-'), GREY);
        assert_eq!(color('y'), YELLOW);
        assert_eq!(color('g'), GREEN);
        assert_eq!(color('Y'), YELLOW);
        assert_eq!(color('G'), GREEN);
    }

    #[test]
    fn test_from_string() {
        assert_eq!(from_string("-----"), 0);
        assert_eq!(from_string("Y----"), 1);
        assert_eq!(from_string("G----"), 2);
        assert_eq!(from_string("--Y-Y"), YELLOW*9+YELLOW*81);
        assert_eq!(from_string("--G-G"), GREEN*9+GREEN*81);
        assert_eq!(from_string("GY-YG"), GREEN+YELLOW*3+YELLOW*27+GREEN*81);
        assert_eq!(from_string("GGGGG"), 242);
    }

    #[test]
    fn test_to_string() {
        assert_eq!(to_string(0), "-----");
        assert_eq!(to_string(1), "Y----");
        assert_eq!(to_string(2), "G----");
        assert_eq!(to_string(1*81+1*9), "--Y-Y");
        assert_eq!(to_string(2*81+2*9), "--G-G");
        assert_eq!(to_string(242), "GGGGG");
    }

    #[test]
    fn test_to_from_string() {
        for score in 0..243 {
            assert_eq!(score, from_string(&to_string(score)));
        }
    }

    // #[test]
    // fn test_score() {
    //     assert_eq!(to_string(score("flown", "trace")), "-----");
    //     assert_eq!(to_string(score("tooth", "trace")), "G----");
    //     assert_eq!(to_string(score("could", "trace")), "Y----");
    //     assert_eq!(to_string(score("eerie", "vixen")), "Y--Y-");
    // }
}
