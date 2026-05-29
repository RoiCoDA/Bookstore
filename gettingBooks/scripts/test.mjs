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
    "9780679783268", "9780451526342", "9780307277671", "9780743273565", "9780060850524",
    "9780143127741", "9780143125426", "9780143126560", "9780143127550", "9780143128724",
    "9780143129080", "9780143129394", "9780143129554", "9780143129707", "9780143129745",
    "9780062498533", "9781501128035", "9780062498557", "9780062498571", "9780062498595",
    "9780062320239", "9780316015844", "9780142402511", "9780142414934", "9780545162074",
    "9780060885598", "9780316055468", "9780316013698", "9780316038370", "9780316073494"
];

const ebooksISBNs = [
    "9781501175565", "9780345538376", "9780062316097", "9780062073488", "9780062464316",
    "9780062666673", "9781501171581", "9780062368970", "9781501139154", "9780062267822",
    "9780062024039", "9780062378925", "9780062872340", "9780062419099", "9780062941507",
    "9780062316059", "9780062348707", "9780062952202", "9780062868995", "9780062422073",
    "9780062654444", "9780062835683", "9780062869045", "9780062910496", "9780062952202",
    "9780062937387", "9780062941520", "9780062674213", "9780062654987", "9780062872347",
    "9780062434237", "9780062952212", "9780062952199", "9780062941699", "9780062869052",
    "9780062941491", "9780062742028", "9780062842797", "9780062742042", "9780062742073",
    "9780062742219", "9780062867905", "9780062952202", "9780062457790", "9780062699663",
    "9780062834996", "9780062950917", "9780062976580", "9780063003537", "9780063049191",
    "9780062899287", "9780062676468", "9780062852404", "9780062966675", "9780063053176",
    "9780063021144", "9780062842705", "9780063053183", "9780062966682", "9780062852411",
    "9780063021151", "9780062472106", "9780062696167", "9780062390622", "9780062390639",
    "9780062412614", "9780545582889", "9780062387240", "9780545010221", "9780316319194",
    "9780062024068", "9780142424179", "9780062425247", "9780545010221", "9780062886455",
    "9780062425247"
];

let books = [];
let ebooks = [];
let authors = new Set();
let notFoundISBNs = [];
let seenISBNs = new Set();
let authorDetails = [];

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

function parseIndustryIdentifiers(industryIdentifiers) {
  let isbn_10 = null;
  let isbn_13 = null;

  if (industryIdentifiers) {
    industryIdentifiers.forEach(identifier => {
      if (identifier.type === "ISBN_10") {
        isbn_10 = identifier.identifier;
      } else if (identifier.type === "ISBN_13") {
        isbn_13 = identifier.identifier;
      }
    });
  }

  return { isbn_10, isbn_13 };
}

function extractPdfLink(pdf) {
  return pdf && pdf.isAvailable ? pdf.acsTokenLink : "";
}

function formatBookData(item, isEbook) {
  const { isbn_10, isbn_13 } = parseIndustryIdentifiers(item.volumeInfo.industryIdentifiers);

  return {
    title: item.volumeInfo.title || "",
    authors: item.volumeInfo.authors || [""],
    subtitle: item.volumeInfo.subtitle || "",
    description: item.volumeInfo.description || "",
    categories: item.volumeInfo.categories || [""],
    publisher: item.volumeInfo.publisher || "",
    publishedDate: item.volumeInfo.publishedDate ? new Date(item.volumeInfo.publishedDate).toISOString() : "",
    printType: item.volumeInfo.printType || "",
    pageCount: item.volumeInfo.pageCount || 0,
    previewLink: item.volumeInfo.previewLink || "",
    maturityRating: item.volumeInfo.maturityRating || "",
    language: item.volumeInfo.language || "",
    infoLink: item.volumeInfo.infoLink || "",
    ISBN_10: isbn_10,
    ISBN_13: isbn_13,
    smallThumbnail: item.volumeInfo.imageLinks ? item.volumeInfo.imageLinks.smallThumbnail : "",
    thumbnail: item.volumeInfo.imageLinks ? item.volumeInfo.imageLinks.thumbnail : "",
    canonicalVolumeLink: item.volumeInfo.canonicalVolumeLink || "",
    selfLink: item.selfLink || "",
    isEbook: isEbook,
    pdf: extractPdfLink(item.accessInfo.pdf) || ""
  };
}

async function getBooksByISBNs(isbns, isEbook = false) {
  for (const isbn of isbns) {
    if (seenISBNs.has(isbn)) {
      console.log(`Skipping duplicate ISBN: ${isbn}`);
      continue;
    }
    const data = await searchBookByISBN(isbn);
    if (data && data.items) {
      for (const item of data.items) {
        const book = formatBookData(item, isEbook);
        if (isEbook) {
          if (!ebooks.some(e => e.isbn === isbn)) {
            ebooks.push(book);
          }
        } else {
          if (!books.some(b => b.isbn === isbn)) {
            books.push(book);
          }
        }
        seenISBNs.add(isbn);
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

async function searchWikipedia(author) {
  const url = `https://en.wikipedia.org/api/rest_v1/page/summary/${encodeURIComponent(author)}`;

  try {
    const response = await fetch(url);
    const data = await response.json();
    return {
      author,
      summary: data.extract || "",
      image: data.originalimage ? data.originalimage.source : "",
      link: data.content_urls ? data.content_urls.desktop.page : ""
    };
  } catch (error) {
    console.error(`Error fetching Wikipedia data for author: ${author}`, error);
    return {
      author,
      summary: "",
      image: "",
      link: ""
    };
  }
}

async function getAuthorDetails(authors) {
  for (const author of authors) {
    const details = await searchWikipedia(author);
    authorDetails.push(details);
  }
}

async function main() {
  await getBooksByISBNs(regularBooksISBNs, false);
  await getBooksByISBNs(ebooksISBNs, true);

  console.log('Books:', books.length);
  console.log('eBooks:', ebooks.length);
  console.log('Authors:', authors.size);
  console.log('Not Found ISBNs:', notFoundISBNs);

  // Get author details
  await getAuthorDetails(Array.from(authors));

  // Export to JSON files
  fs.writeFileSync('books.json', JSON.stringify(books, null, 2), 'utf-8');
  fs.writeFileSync('ebooks.json', JSON.stringify(ebooks, null, 2), 'utf-8');
  fs.writeFileSync('authors.json', JSON.stringify(Array.from(authors), null, 2), 'utf-8');
  fs.writeFileSync('author_details.json', JSON.stringify(authorDetails, null, 2), 'utf-8');
  fs.writeFileSync('not_found_isbns.json', JSON.stringify(notFoundISBNs, null, 2), 'utf-8');
}

main();