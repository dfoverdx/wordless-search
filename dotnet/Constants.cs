namespace WordlessSearch {
  public static class Constants {
    static readonly int GridSize = 16;
    static readonly double MinVowelRatio = 0.1;
    static readonly int MinVowels = (int)(MinVowelRatio * GridSize * GridSize);
  }
}