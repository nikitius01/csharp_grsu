using laba_2.Domain.Entities;

namespace laba_2.Domain.Interfaces;

public interface IGameLogger
{
    void WriteHeader();
    void WriteState(Game game);
    void WriteFooter(Game game);
    string Build();
}
