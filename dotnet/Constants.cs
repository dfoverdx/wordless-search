namespace WordlessSearch {
  public static class Constants {
    public const int GridSize = 16;
    private const double MinVowelRatio = 1.0 / 5.0;
    public const int MinVowels = (int)(MinVowelRatio * GridSize * GridSize);
    public const int MaxRunLength = 2;
    public const int MaxRunsPerRowOrColumn = 2;
    public const int MinVowelsPerRowOrColumn = 3;
  }
}