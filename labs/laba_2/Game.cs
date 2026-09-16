namespace laba_2;

internal sealed class Game
{
    private readonly int _boardSize;
    private int? _mousePosition;
    private int? _catPosition;
    private long _mouseDistance;
    private long _catDistance;

    public int? MousePosition => _mousePosition;
    public int? CatPosition => _catPosition;
    public long MouseDistance => _mouseDistance;
    public long CatDistance => _catDistance;
    public int? CaughtAt { get; private set; }
    public bool IsFinished => CaughtAt.HasValue;

    public Game(int boardSize)
    {
        _boardSize = boardSize;
    }

    public void MoveMouse(int steps) => Move(ref _mousePosition, ref _mouseDistance, steps);

    public void MoveCat(int steps) => Move(ref _catPosition, ref _catDistance, steps);

    public int? GetDistance() => MousePosition.HasValue && CatPosition.HasValue
        ? Math.Abs(MousePosition.Value - CatPosition.Value)
        : null;

    private void Move(ref int? position, ref long distance, int steps)
    {
        if (IsFinished)
            return;

        if (position.HasValue)
        {
            position = Normalize((long)position.Value + steps);
            distance += Math.Abs((long)steps);
        }
        else
        {
            position = Normalize(steps);
        }

        if (MousePosition.HasValue && MousePosition == CatPosition)
            CaughtAt = MousePosition;
    }

    private int Normalize(long position)
    {
        var zeroBased = (position - 1) % _boardSize;
        if (zeroBased < 0)
            zeroBased += _boardSize;

        return (int)zeroBased + 1;
    }
}
