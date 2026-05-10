# scrape_all_words.py

import requests
from bs4 import BeautifulSoup
from pathlib import Path
import time

BASE_URL = "https://makedonski.gov.mk/bukva/{letter}?strana={page}"

LETTERS = [
    "а", "б", "в", "г", "д", "ѓ", "е", "ж", "з",
    "ѕ", "и", "ј", "к", "л", "љ", "м", "н", "њ",
    "о", "п", "р", "с", "т", "ќ", "у", "ф", "х",
    "ц", "ч", "џ", "ш"
]

OUTPUT_FILE = Path("./dict_all.txt")

NOT_FOUND_TEXT = "Страницата која ја баравте не е пронајдена!"

HEADERS = {
    "User-Agent": "Mozilla/5.0"
}


def get_page(letter, page_number):
    url = BASE_URL.format(letter=letter, page=page_number)

    response = requests.get(url, headers=HEADERS, timeout=15)
    response.raise_for_status()

    return response.text


def page_not_found(html):
    return NOT_FOUND_TEXT in html


def extract_words(html):
    soup = BeautifulSoup(html, "html.parser")

    words = []

    for link in soup.select("h2 a"):
        word = link.get_text(strip=True)

        if word:
            words.append(word)

    return words


def scrape_letter(letter):
    page_number = 1
    letter_words = []

    print(f"\nScraping letter: {letter}")

    while True:
        print(f"Page {page_number}")

        try:
            html = get_page(letter, page_number)

            if page_not_found(html):
                print(f"Page {page_number} not found. Moving to next letter.")
                break

            words = extract_words(html)

            if words:
                letter_words.extend(words)
                print(f"Found {len(words)} words")
            else:
                print("No words found on this page.")

            page_number += 1
            time.sleep(0.5)

        except Exception as e:
            print(f"Error on letter {letter}, page {page_number}: {e}")
            break

    print(f"Total for {letter}: {len(letter_words)}")

    return letter_words


def main():
    all_words = []

    if OUTPUT_FILE.exists():
        OUTPUT_FILE.unlink()

    for letter in LETTERS:
        words = scrape_letter(letter)
        all_words.extend(words)

    OUTPUT_FILE.write_text(
        "\n".join(all_words),
        encoding="utf-8"
    )

    print(f"\nDONE. Saved {len(all_words)} words to {OUTPUT_FILE}")


if __name__ == "__main__":
    main()