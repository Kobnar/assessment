namespace Assessment.Models;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string AccountsCollectionName { get; set; } = null!;
}