import re
from pathlib import Path
from collections import Counter

# Macedonian alphabet you use
MACEDONIAN_LETTERS = "АБВГДЃЕЖЗЅИЈКЛЉМНЊОПРСТЌУФХЦЧЏШ"
MACEDONIAN_PATTERN = re.compile(rf"^[{MACEDONIAN_LETTERS}]+$")

# Paths are relative to the script location
INPUT_FILE = "./dict_all.txt"
BLACKLIST_FILE = "./blacklist.txt"

# This should resolve to: Assets/Resources/Dictionary
OUTPUT_DIR = Path("../../Resources/Dictionary")


def load_blacklist():
    path = Path(BLACKLIST_FILE)

    if not path.exists():
        return set()

    return {
        line.strip().upper()
        for line in path.read_text(encoding="utf-8").splitlines()
        if line.strip()
    }


def clean_words():
    blacklist = load_blacklist()
    words = []

    with open(INPUT_FILE, "r", encoding="utf-8") as file:
        for line in file:
            word = line.strip().upper()

            if not word:
                continue

            if not MACEDONIAN_PATTERN.match(word):
                continue

            if len(word) < 3 or len(word) > 6:
                continue

            if word in blacklist:
                continue

            words.append(word)

    counter = Counter(words)
    # Keep words that appear at least once (Counter is mostly useful if you want to filter by frequency)
    final_words = [word for word, count in counter.items() if count >= 1]

    return sorted(final_words)


def save_by_first_letter(words):
    # Group words by first letter
    words_by_letter = {letter: [] for letter in MACEDONIAN_LETTERS}

    for word in words:
        first_letter = word[0]
        if first_letter in words_by_letter:
            words_by_letter[first_letter].append(word)

    # Write one file per letter
    for letter, word_list in words_by_letter.items():
        if not word_list:
            continue  # skip empty letters if you want

        file_path = OUTPUT_DIR / f"{letter}.txt"
        file_path.parent.mkdir(parents=True, exist_ok=True)

        # Join words with newline; always use UTF-8
        text = "\n".join(word_list)
        file_path.write_text(text, encoding="utf-8")


def main():
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    words = clean_words()
    save_by_first_letter(words)

    print(f"Done. Total valid words: {len(words)}")
    print(f"Dictionary created in: {OUTPUT_DIR.resolve()}")


if __name__ == "__main__":
    main()