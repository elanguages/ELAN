namespace ELAN.Api.Models
{
    public class EntityDetails
    {
        public EntityDescription? Description { get; set; }
        public List<EntityStatement>? Statements { get; set; }
    }

    public class EntityDescription
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
    }

    public class EntityStatement
    {
        public string? PropertyLabel { get; set; }
        public string? PropertyDescription { get; set; }
        public string? PropertyLink { get; set; }
        public List<StatementValue> Values { get; set; } = [];
    }

    public class StatementValue
    {
        public string? Value { get; set; }
        public string? ValueLabel { get; set; }
        public string? ValueDescription { get; set; }
    }

}
