using laba_2.Domain.Enums;

namespace laba_2.Domain.Entities;

public sealed class Player
{
    public string Name { get; }
    public int Position { get; private set; }
    public PlayerState State { get; private set; }
    public long DistanceTraveled { get; private set; }

    public Player(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя игрока не может быть пустым.", nameof(name));

        Name = name;
        State = PlayerState.NotInGame;
    }

    public void Enter(int position, Board board)
    {
        ArgumentNullException.ThrowIfNull(board);
        Position = board.Normalize(position);
        State = PlayerState.Playing;
    }

    public void Move(int steps, Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        if (State != PlayerState.Playing)
            return;

        Position = board.Normalize(Position + steps);
        DistanceTraveled += Math.Abs((long)steps);
    }

    public void Win() => State = PlayerState.Winner;
    public void Lose() => State = PlayerState.Loser;
}
