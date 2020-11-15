declare interface Math {
  randomInt(max: number): number;
  randomInt(min: number, max: number): number;
}

Math.randomInt = (min: number, max?: number) => {
  if (max == null) {
    max = min;
    min = 0;
  }

  return Math.floor(Math.random() * (max - min)) + min
};


declare interface Array<T> {
  shuffle(): this;
}

Array.prototype.shuffle = function() {
  for (let i = 0; i < this.length; i++) {
    const idx = Math.randomInt(i, this.length);
    [this[i], this[idx]] = [this[idx], this[i]];
  }

  return this;
}