namespace IoT.Temperature;

public class ModuleDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string TemperatureCollectionName { get; set; } = null!;
}