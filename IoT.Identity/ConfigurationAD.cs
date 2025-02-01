namespace IoT.Identity;

// ReSharper disable once InconsistentNaming
internal class ConfigurationAD
{
    public const string Position = "AD";
    public string BaseDn { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string OU { get; set; } = string.Empty;

}