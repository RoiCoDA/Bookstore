# data

JSON files produced by the scripts in `../scripts/`. Each file is an intermediate or final output of the data pipeline.

## Files

| File | Produced by | Description |
|---|---|---|
| `books.json` | `test.mjs` | Raw regular books fetched from Google Books by ISBN |
| `ebooks.json` | `test.mjs` | Raw ebooks fetched from Google Books by ISBN |
| `authors.json` | `test.mjs` | Flat list of unique author names collected from the above |
| `author_details.json` | `test.mjs` | Per-author Wikipedia summary, image URL, and page link |
| `not_found_isbns.json` | `test.mjs` | ISBNs that returned no results from the Google Books API |
| `unique_books.json` | `cleaner.js` | Deduplicated version of `books.json`, with a `rating` placeholder field |
| `unique_ebooks.json` | `cleaner.js` | Deduplicated version of `ebooks.json`, with a `rating` placeholder field |
| `unique_authors.json` | `cleaner.js` | Deduplicated version of `author_details.json`, each entry linked to the books they authored |
| `merged_books.json` | `mergeAndPrice.mjs` | Final combined catalogue — unique books and ebooks merged into one array with prices assigned |
