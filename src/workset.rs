// Working set

use crate::words::{Word, WORDS};

struct Workset {
    head: Word,                     // first word in the list
    list: [Word; WORDS as usize],   // next word or 0 to end list
}

impl Workset {
    // New set with all words
    pub fn new() -> Workset {
        let mut workset = Workset {
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
        let mut count = 1;
        let mut w = self.head;
        while let Some(next) = self.next(w) {
            count += 1;
            w = next;
        }
        count
    }

    // Prune the working set, removing words for which 'prune' returns true.
    pub fn prune<F>(&mut self, prune: F) where F: Fn(Word) -> bool {
        // move head to first unpruned word
        let mut w = self.head;
        while prune(w) {
            w = self.list[w as usize];
            if w == 0 { panic!("prune empty") }
        }
        self.head = w;

        while let Some(next) = self.next(w) {
            if prune(next) {
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

    #[test]
    fn test_prune() {
        let mut workset = Workset::new();
        workset.prune(|w| w > 0);
        assert_eq!(workset.remaining(), 1);

        workset.fill();
        workset.prune(|w| w == WORDS-1);
        assert_eq!(workset.remaining(), WORDS-1);

        workset.fill();
        workset.prune(|w| w & 1 == 0);
        assert_eq!(workset.remaining(), WORDS/2);
    }

    #[test]
    #[should_panic]
    fn test_prune_empty() {
        let mut workset = Workset::new();
        workset.prune(|_w| true);
    }
}
