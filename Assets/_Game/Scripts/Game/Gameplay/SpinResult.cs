using VertigoGames.Data;

namespace VertigoGames.Gameplay
{
    public readonly struct SpinResult
    {
        public WheelSliceData Slice { get; }
        public ZoneConfig Config { get; }

        public SpinResult(WheelSliceData slice, ZoneConfig config)
        {
            Slice = slice;
            Config = config;
        }
    }
}