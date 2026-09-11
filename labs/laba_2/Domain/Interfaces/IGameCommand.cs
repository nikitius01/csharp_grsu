using laba_2.Domain.Entities;

namespace laba_2.Domain.Interfaces;

public interface IGameCommand
{
    void Execute(Game game);
}
