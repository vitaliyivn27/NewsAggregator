using Serilog;

namespace NewsAggregatorMvcApp.ConfigurationProviders;

public static class TextConfigurationExtenstion
{
    public static IConfigurationBuilder AddTextFile(this IConfigurationBuilder builder, string path)
    {
        try
        {
            var source = new TxtConfigurationSource(path);
            builder.Add(source);
            return builder;
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : AddTextFile");
            throw;
        }
    }
}