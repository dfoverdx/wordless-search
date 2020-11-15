import Grid from './grid';

Grid.verbose = true;
Grid.highlightWords = true;

console.clear();

// for (const word of Grid.findWords()) {
//   Grid.highlight(word);
//   console.log('\n\n\n');
// }

// console.log(Grid.toString());

Grid.allowOnlyNWords();
console.log('\n' + Grid.findWords().map(({ word }) => word).join('\n'));