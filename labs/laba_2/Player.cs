namespace laba_2;

public class Player
{
    public string Name { get; }
    public int Position { get; private set; }
    public PlayerState State { get; private set; }
    public long DistanceTraveled { get; private set; }

    public Player(string name)
    {
        Name = name;
        Position = 0;
        State = PlayerState.NotInGame;
        DistanceTraveled = 0;
    }

    public void SetInitialPosition(int position, int boardSize)
    {
        Position = NormalizePosition(position, boardSize);
        State = PlayerState.Playing;
    }

    public void Move(int steps, int boardSize)
    {
        if (State != PlayerState.Playing)
            return;

        Position = NormalizePosition(Position + steps, boardSize);
        DistanceTraveled += Math.Abs((long)steps);
    }

    public void SetState(PlayerState state) => State = state;

    private static int NormalizePosition(int position, int boardSize)
    {
        int zeroBased = (position - 1) % boardSize;
        if (zeroBased < 0)
            zeroBased += boardSize;

        return zeroBased + 1;
    }
}
