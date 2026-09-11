using laba_2.Domain.Enums;

namespace laba_2.Domain.Entities;

public sealed class Game
{
    public Board Board { get; }
    public Player Mouse { get; }
    public Player Cat { get; }
    public bool IsFinished { get; private set; }
    public int? CaughtAt { get; private set; }

    public Game(Board board, Player mouse, Player cat)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
        Mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
        Cat = cat ?? throw new ArgumentNullException(nameof(cat));
    }

    public void MoveMouse(int steps) => MovePlayer(Mouse, steps);
    public void MoveCat(int steps) => MovePlayer(Cat, steps);

    public int? GetDistance()
    {
        if (Mouse.State == PlayerState.NotInGame || Cat.State == PlayerState.NotInGame)
            return null;

        return Math.Abs(Mouse.Position - Cat.Position);
    }

    private void MovePlayer(Player player, int steps)
    {
        if (IsFinished)
            return;

        if (player.State == PlayerState.NotInGame)
            player.Enter(steps, Board);
        else
            player.Move(steps, Board);

        CheckCapture();
    }

    private void CheckCapture()
    {
        if (Mouse.State != PlayerState.Playing || Cat.State != PlayerState.Playing)
            return;

        if (Mouse.Position != Cat.Position)
            return;

        IsFinished = true;
        CaughtAt = Mouse.Position;
        Cat.Win();
        Mouse.Lose();
    }
}
