using System;

namespace WordlessSearch {
  using Point = Tuple<int, int>;

  public static class Constants {
    public const bool DoPrint = true;
    public const Verbosity VerbosityLevel = Verbosity.Normal;
    public const bool RunParallel = true;
    public const bool VerifyOnChange = false;
    public const bool HighlightVowels = true;

    public const int GridSize = 20;

    public const double MinDistributionRatio = 0.55;

    public const int MaxRunLength = 2;
    public const int MaxVowelRunsPerRowOrColumn = 1;
    public const int MaxRunsPerRowOrColumn = 4;
    public const int MinVowelsPerRowOrColumn = 2;
    public const int MaxVowelsPerRowOrColumn = 8;
    public const int MinDistinctVowelsPerRowOrColumn = 3;
    public const int MaxSameCharacterPerRowOrColumn = 4;
    public const int MinWordLength = 3;

    public const int Threads = 10;

    public static readonly WordPos[] StaticWords = new WordPos[] {
        new WordPos { Word = "FRIENDS", Direction = Direction.East, Point = new Point(1, 1) },
        new WordPos { Word = "MONEY", Direction = Direction.South, Point = new Point(GridSize - 1, 4) }
    };
  }
}