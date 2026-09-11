using laba_2.Application.Commands;

namespace laba_2.Domain.Interfaces;

public interface ICommandParser
{
    GameCommand Parse(string line);
}
