import fs from 'fs';
import path from 'path';

const words = fs.readFileSync(path.join(__dirname, './dictionary.txt'))
  .toString()
  .split(/\r?\n/)
  .filter(word => word.length > 2)
  .map(word => word.toUpperCase());

export default words;