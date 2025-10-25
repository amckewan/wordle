// Working set

use crate::words::{Word, WORDS};
use crate::score::{Score};

struct Workset {
    head: Word,                     // first word in the list
    list: [Word; WORDS as usize],   // next word or 0 to end list
}

impl Workset {
    // New set with all words
    pub fn new() -> Workset {
        let mut workset = Workset{
            head: 0,
            list: [0; _],
        };
        workset.fill();
        workset
    }

    // Add all words to the working set
    pub fn fill(&mut self) {
        self.head = 0;
        for i in 0..WORDS-1 {
            self.list[i as usize] = i + 1;
        }
        self.list[WORDS as usize - 1] = 0; // terminate list

    }

    pub fn first(&self) -> Word {
        self.head
    }

    pub fn next(&self, w: Word) -> Option<Word> {
        let n = self.list[w as usize];
        if n > 0 { Some(n) } else { None }
    }

    pub fn remaining(&self) -> u16 {
        let mut count = 0;
        let mut w = self.head;
        loop {
            count += 1;
            match self.next(w) {
                Some(n) => w = n,
                None => break,

            }
        }
        count
    }

    // todo...
    fn pruner(target: Word, guess: Word, score: Score) -> bool {
        false
    }

    // Prune the working set, removing words that wouldn't produce this score.
    pub fn prune(&mut self, guess: Word, score: Score) {
        let mut w = self.head;
        while Self::pruner(w, guess, score) {
            w = self.list[w as usize];
        }
        self.head = w;

        loop {
            let next = self.list[w as usize];
            if next == 0 { break; }
            if Self::pruner(next, guess, score) {
                self.list[w as usize] = self.list[next as usize];
            } else {
                w = next;
            }
        }
    }

}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_first_next() {
        let mut workset = Workset::new();
        assert_eq!(workset.first(), 0);
        assert_eq!(workset.next(0), Some(1));
        assert_eq!(workset.next(1000), Some(1001));
        assert_eq!(workset.next(WORDS-1), None);
    }

    #[test]
    fn test_remaining() {
        let mut workset = Workset::new();
        assert_eq!(workset.remaining(), WORDS);
        workset.head = 3;
        assert_eq!(workset.remaining(), WORDS-3);
        workset.head = 0;
        workset.list[0] = 0;
        assert_eq!(workset.remaining(), 1);
    }
}
