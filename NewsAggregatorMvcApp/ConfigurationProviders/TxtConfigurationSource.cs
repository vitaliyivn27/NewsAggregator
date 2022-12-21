namespace NewsAggregatorMvcApp.ConfigurationProviders;

public class TxtConfigurationSource : IConfigurationSource
{
    private readonly string _path;
    public TxtConfigurationSource(string path)
    {
        _path = path;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new TxtConfigurationProvider(_path);
    }
}