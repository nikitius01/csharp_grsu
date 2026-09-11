namespace laba_2.Domain.Entities;

public sealed class Board
{
    public int Size { get; }

    public Board(int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Размер поля должен быть положительным.");

        Size = size;
    }

    public int Normalize(int position)
    {
        var zeroBased = (position - 1) % Size;
        if (zeroBased < 0)
            zeroBased += Size;

        return zeroBased + 1;
    }
}
