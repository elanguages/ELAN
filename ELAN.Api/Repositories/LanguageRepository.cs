using ELAN.Api.Repositories.Interfaces;

namespace ELAN.Api.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        public List<string> GetLanguages()
        {
            return
            [
                "Brainfuck",
                "LOLCODE",
                "Whitespace",
                "Piet",
                "Malbolge"
            ];
        }
    }
}
