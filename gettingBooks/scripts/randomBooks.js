import fetch from 'node-fetch';
import fs from 'fs';

// Ensure the arrays contain 50 unique ISBNs each
const regularBooksISBNs = [
    "9780316769488", "9780062316097", "9780316769532", "9780141182803", "9780141185064",
    "9780316769174", "9780143039433", "9780062315007", "9780307389732", "9780553386790",
    "9780385472579", "9780140449266", "9780451524935", "9780143035008", "9780671027032",
    "9780345803481", "9780812981605", "9781400033416", "9780307474278", "9780385751537",
    "9781400078776", "9780307592736", "9780061120084", "9780679783268", "9780451526342",
    "9780451532251", "9780307277671", "9780743273565", "9780140177398", "9780143105954",
    "9780141439600", "9780141439518", "9780143128540", "9780743482840", "9780446310789",
    "9780385490818", "9780143106456", "9780385737955", "9780307278449", "9780140283334",
    "9780747532743", "9781451673319", "9780061122415", "9780060883287", "9780060935467",
    "9780679783268", "9780451526342", "9780307277671", "9780743273565", "9780060850524"
];

const ebooksISBNs = [
    "9780062024329", "9780307594006", "9780062060624", "9780062020536", "9780061842160",
    "9780316098328", "9780061965092", "9780590353427", "9780060935467", "9780385490818",
    "9780307588371", "9780316129360", "9780062024039", "9780307592736", "9780316769488",
    "9780307476463", "9780062316097", "9780316769174", "9780316769532", "9780141182803",
    "9780141185064", "9780143039433", "9780062315007", "9780307389732", "9780553386790",
    "9780385472579", "9780140449266", "9780451524935", "9780143035008", "9780671027032",
    "9780345803481", "9780812981605", "9781400033416", "9780307474278", "9780385751537",
    "9781400078776", "9780307592736", "9780061120084", "9780679783268", "9780451526342",
    "9780451532251", "9780307277671", "9780743273565", "9780140177398", "9780143105954",
    "9780141439600", "9780141439518", "9780143128540", "9780062060624", "9780060935467"
];

let books = [];
let ebooks = [];
let authors = new Set();
let notFoundISBNs = [];

async function searchBookByISBN(isbn) {
  const baseUrl = 'https://www.googleapis.com/books/v1/volumes';
  const url = `${baseUrl}?q=isbn:${isbn}`;

  try {
    console.log(`Fetching URL: ${url}`);
    const response = await fetch(url);
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching book:', error);
  }
}

async function getBooksByISBNs(isbns, isEbook = false) {
  for (const isbn of isbns) {
    const data = await searchBookByISBN(isbn);
    if (data && data.items) {
      for (const item of data.items) {
        const book = {
          title: item.volumeInfo.title,
          authors: item.volumeInfo.authors,
          isbn: isbn,
          isEbook: isEbook
        };
        if (isEbook) {
          ebooks.push(book);
        } else {
          books.push(book);
        }
        if (book.authors) {
          book.authors.forEach(author => authors.add(author));
        }
      }
    } else {
      console.log(`No data found for ISBN: ${isbn}`);
      notFoundISBNs.push(isbn);
    }
  }
}

async function main() {
  await getBooksByISBNs(regularBooksISBNs, false);
  await getBooksByISBNs(ebooksISBNs, true);
  
  console.log('Books:', books.length);
  console.log('eBooks:', ebooks.length);
  console.log('Authors:', authors.size);
  console.log('Not Found ISBNs:', notFoundISBNs);
  
  // Export to JSON files
  fs.writeFileSync('books.json', JSON.stringify(books, null, 2), 'utf-8');
  fs.writeFileSync('ebooks.json', JSON.stringify(ebooks, null, 2), 'utf-8');
  fs.writeFileSync('authors.json', JSON.stringify(Array.from(authors), null, 2), 'utf-8');
}

main();