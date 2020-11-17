namespace WordlessSearch {
  public static class Constants {
    public const int GridSize = 16;
    // public const int GridSize = 10;
    private const double MinVowelRatio = 1.0 / 10.0;
    public const int MinVowels = (int)(MinVowelRatio * GridSize * GridSize);
    public const int MaxRunLength = 3;
    public const int MaxRunsPerRowOrColumn = 3;
    // public const int MinVowelsPerRowOrColumn = 3;
    public const int MinVowelsPerRowOrColumn = 2;
    public const int MinWordLength = 3;
  }
}