namespace Core.Persistence.Dynamic;

public class Sort(string field, string dir)
{
    public string Field { get; set; } = field;
    public string Dir { get; set; } = dir;

    public Sort() : this(string.Empty, string.Empty)
    {
    }
}