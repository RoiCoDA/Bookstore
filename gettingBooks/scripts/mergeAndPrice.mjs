const fs = require('fs');

var prices = [ 9.99, 10.99, 15.50, 19.99, 24.99, 11.99, 20.50, 14.50, 13.50, 14.90, 25.99 ]

function getRandomPrice() {
    return prices[Math.floor(Math.random() * prices.length)];
  }
  
  // Function to read a JSON file
  function readJsonFile(filePath) {
    try {
      const data = fs.readFileSync(filePath, 'utf-8');
      console.log(`Successfully read file: ${filePath}`);
      return JSON.parse(data);
    } catch (error) {
      console.error(`Error reading file: ${filePath}`, error);
      return null;
    }
  }
  
  // Function to write a JSON file
  function writeJsonFile(filePath, data) {
    try {
      fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf-8');
      console.log(`Successfully wrote file: ${filePath}`);
    } catch (error) {
      console.error(`Error writing file: ${filePath}`, error);
    }
  }
  
  // Function to add a random price to each item in the JSON object
  function addPriceToItems(jsonObject) {
    if (Array.isArray(jsonObject)) {
      return jsonObject.map(item => {
        item.price = getRandomPrice();
        return item;
      });
    } else if (typeof jsonObject === 'object' && jsonObject !== null) {
      Object.keys(jsonObject).forEach(key => {
        jsonObject[key].price = getRandomPrice();
      });
      return jsonObject;
    }
    return jsonObject;
  }
  
  // Function to merge two JSON objects deeply
  function mergeJsonObjectsDeep(obj1, obj2) {
    return { ...obj1, ...obj2 };
  }
  
  // Main function to merge two JSON files and add prices
  function main() {
    const file1 = 'unique_books.json';
    const file2 = 'unique_ebooks.json';
    const outputFile = 'merged_books.json';
  
    const json1 = readJsonFile(file1);
    const json2 = readJsonFile(file2);
  
    if (json1 && json2) {
      // Add prices to each item in both JSON objects
      const json1WithPrices = addPriceToItems(json1);
      const json2WithPrices = addPriceToItems(json2);
  
      // Merge the JSON objects
      const mergedJson = mergeJsonObjectsDeep(json1WithPrices, json2WithPrices);
  
      // Write the merged JSON object to a new file
      writeJsonFile(outputFile, mergedJson);
    } else {
      console.error('Error reading one or both of the input files.');
    }
  }
  
  main();