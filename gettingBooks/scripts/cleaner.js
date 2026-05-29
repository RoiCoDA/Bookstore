import fs from 'fs';

function readJsonFile(filePath) {
  try {
    const data = fs.readFileSync(filePath, 'utf-8');
    console.log(`Successfully read file: ${filePath}`);
    return JSON.parse(data);
  } catch (error) {
    console.error(`Error reading file: ${filePath}`, error);
    return [];
  }
}

function writeJsonFile(filePath, data) {
  try {
    fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf-8');
    console.log(`Successfully wrote file: ${filePath}`);
  } catch (error) {
    console.error(`Error writing file: ${filePath}`, error);
  }
}

function deduplicateBooks(books) {
  const seenTitles = new Set();
  const uniqueBooks = [];

  books.forEach(book => {
    if (!seenTitles.has(book.title)) {
      seenTitles.add(book.title);
      book.rating = null;  // Add rating field to each book
      uniqueBooks.push(book);
    } else {
      console.log(`Duplicate book found and removed: ${book.title}`);
    }
  });

  console.log(`Deduplicated books count: ${uniqueBooks.length}`);
  return uniqueBooks;
}

function deduplicateAuthors(authors, books) {
  const seenAuthors = new Set();
  const uniqueAuthors = [];

  authors.forEach(author => {
    if (!seenAuthors.has(author.author)) {
      seenAuthors.add(author.author);
      author.books = books
        .filter(book => book.authors.includes(author.author))
        .map(book => book.title);
      uniqueAuthors.push(author);
    } else {
      console.log(`Duplicate author found and removed: ${author.author}`);
    }
  });

  console.log(`Deduplicated authors count: ${uniqueAuthors.length}`);
  return uniqueAuthors;
}

function main() {
  // Read the JSON files
  const books = readJsonFile('books.json');
  const ebooks = readJsonFile('ebooks.json');
  const authors = readJsonFile('author_details.json');

  // Check if files were read successfully
  if (!books.length) console.error('books.json is empty or failed to load');
  if (!ebooks.length) console.error('ebooks.json is empty or failed to load');
  if (!authors.length) console.error('author_details.json is empty or failed to load');

  // Deduplicate the books and authors
  const uniqueBooks = deduplicateBooks(books);
  const uniqueEbooks = deduplicateBooks(ebooks);
  const uniqueAuthors = deduplicateAuthors(authors, uniqueBooks.concat(uniqueEbooks));

  // Write the deduplicated data back to JSON files
  writeJsonFile('unique_books.json', uniqueBooks);
  writeJsonFile('unique_ebooks.json', uniqueEbooks);
  writeJsonFile('unique_authors.json', uniqueAuthors);
}

main();