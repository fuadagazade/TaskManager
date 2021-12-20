namespace TaskManager.Core.Models.Table;

public class TableResponse<T>
{
    public List<T> Items { get; set; }

    public long Total { get; set; }

    public TableResponse()
    {
        this.Items = new List<T>();
        this.Total = 0;
    }
}