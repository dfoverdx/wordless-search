using System;

namespace WordlessSearch
{
    [Flags]
    public enum Verbosity
    {
        Normal = 0x1 << 0,
        Verbose = 0x1 << 1,
        Extreme = 0x1 << 2,
    }
}