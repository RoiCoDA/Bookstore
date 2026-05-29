# scripts

Node.js scripts that form the data collection pipeline. Run them in order from the `scripts/` directory.

## Scripts

### 1. `test.mjs` — Fetch raw data
Queries the **Google Books API** by ISBN for two hardcoded lists (regular books and ebooks). For each book it extracts a full set of fields: title, authors, ISBNs, description, categories, thumbnail, page count, language, and more. After collecting all books it also queries **Wikipedia** for a bio, image, and page link for every unique author.

**Outputs** (written to the `data/` folder):
- `books.json` — raw regular books
- `ebooks.json` — raw ebooks
- `authors.json` — flat list of unique author names
- `author_details.json` — author name, Wikipedia summary, image URL, and Wikipedia link
- `not_found_isbns.json` — ISBNs that returned no results from Google Books

```bash
node test.mjs
```

---

### 2. `cleaner.js` — Deduplicate
Reads `books.json`, `ebooks.json`, and `author_details.json` from `data/`. Removes duplicate entries (by title for books, by name for authors) and adds a placeholder `rating` field to each book.

**Outputs** (written to the `data/` folder):
- `unique_books.json`
- `unique_ebooks.json`
- `unique_authors.json`

```bash
node cleaner.js
```

---

### 3. `mergeAndPrice.mjs` — Merge and assign prices
Reads `unique_books.json` and `unique_ebooks.json` from `data/`. Assigns a random price from a fixed price list to each entry, then merges both arrays into a single file.

**Output** (written to the `data/` folder):
- `merged_books.json` — the final combined catalogue with prices, ready for database import

```bash
node mergeAndPrice.mjs
```

---

### `randomBooks.js` — Early prototype (unused)
An earlier, simpler version of `test.mjs`. Uses a shorter ISBN list, extracts fewer book fields, and does not fetch Wikipedia author details. Superseded by `test.mjs` and kept for reference only.

---

## package.json / package-lock.json

The only npm dependency is `node-fetch`, which `test.mjs` and `randomBooks.js` use to make HTTP requests (required in Node versions that predate the built-in `fetch`).

**These files are only needed if you plan to re-run the fetching scripts.** Since the data has already been collected in `data/`, they can be deleted if the pipeline will not be run again. If you do need to run the scripts, restore the dependency first:

```bash
npm install
```
