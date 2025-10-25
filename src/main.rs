mod words;
mod score;
mod wordle;
mod workset;
mod solver;

static WORDLIST: &str = "\
aback\
abase\
abate\
abbey\
abbot\
";

fn main() {
    println!("wordlist = '{}'", WORDLIST);
    for w in 0..2315 {
        print!("{} ", words::ww(w))
    }
}
