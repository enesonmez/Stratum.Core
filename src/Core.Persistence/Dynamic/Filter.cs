namespace Core.Persistence.Dynamic;

public class Filter(string field, string @operator)
{
    public string Field { get; set; } = field;
    public string Operator { get; set; } = @operator;
    public string? Value { get; set; }
    public string? Logic { get; set; }
    public bool CaseSensitive { get; set; } = false;
    public IEnumerable<Filter>? Filters { get; set; }

    public Filter() : this(string.Empty, string.Empty)
    {
    }
}