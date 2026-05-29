# gettingBooks

One-time data pipeline used to initially seed the Avenue 79 bookstore database with real book, ebook, and author data pulled from the Google Books API and Wikipedia.

## Folder structure

| Folder | Contents |
|---|---|
| `scripts/` | Node.js scripts that fetch, clean, and price the data |
| `data/` | The JSON output files produced by those scripts |

## Pipeline (run order)

```
test.mjs  →  cleaner.js  →  mergeAndPrice.mjs
```

See `scripts/README.md` for details on each step.

> **Note:** The data has already been collected and lives in `data/`. These scripts do not need to be run again unless the book catalogue needs to be rebuilt from scratch.
