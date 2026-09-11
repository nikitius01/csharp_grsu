namespace laba_2;

public class Game
{
    public int BoardSize { get; }
    public Player Mouse { get; }
    public Player Cat { get; }

    public bool IsFinished { get; private set; }
    public int? CaughtAt { get; private set; }

    public Game(int boardSize)
    {
        if (boardSize <= 0)
            throw new ArgumentException("Размер поля должен быть положительным.", nameof(boardSize));

        BoardSize = boardSize;
        Mouse = new Player("Mouse");
        Cat = new Player("Cat");
    }

    public void ExecuteCommand(string command, int value = 0)
    {
        if (IsFinished)
            return;

        switch (command.ToUpperInvariant())
        {
            case "M":
                MovePlayer(Mouse, value);
                break;

            case "C":
                MovePlayer(Cat, value);
                break;

            case "P":
                break;

            default:
                throw new InvalidOperationException($"Неизвестная команда: {command}");
        }
    }

    private void MovePlayer(Player player, int steps)
    {
        if (player.State == PlayerState.NotInGame)
            player.SetInitialPosition(steps, BoardSize);
        else
            player.Move(steps, BoardSize);

        CheckCapture();
    }

    private void CheckCapture()
    {
        if (Mouse.State == PlayerState.Playing &&
            Cat.State == PlayerState.Playing &&
            Mouse.Position == Cat.Position)
        {
            IsFinished = true;
            CaughtAt = Mouse.Position;

            Cat.SetState(PlayerState.Winner);
            Mouse.SetState(PlayerState.Loser);
        }
    }

    public int? GetDistance()
    {
        if (Mouse.State == PlayerState.NotInGame ||
            Cat.State == PlayerState.NotInGame)
            return null;

        return Math.Abs(Mouse.Position - Cat.Position);
    }
}
