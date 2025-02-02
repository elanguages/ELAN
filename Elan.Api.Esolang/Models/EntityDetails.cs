namespace Elan.Api.Esolang.Models
{
    public class EntityDetails
    {
        public Dictionary<string, string>? Description { get; set; }
        public Dictionary<string, StatementDetails>? Statements { get; set; }
    }

    public class StatementDetails
    {
        public string? PropertyDescription { get; set; }
        public string? PropertyLink { get; set; }
        public List<StatementValue> Values { get; set; } = new();
    }

    public class StatementValue
    {
        public string? Value { get; set; }
        public string? ValueLabel { get; set; }
        public string? ValueDescription { get; set; }
    }
}
