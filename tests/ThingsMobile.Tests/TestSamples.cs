namespace ThingsMobile.Tests
{
    internal static class TestSamples
    {
        public static Task<string> GetSampleResourceAsStringAsync(string fileName)
        {
            var resourceName = string.Join(".", typeof(TestSamples).Namespace, "Samples", fileName);
            using var st = typeof(TestSamples).Assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(st);
            return reader.ReadToEndAsync();
        }

        public static Task<string> GetErrorAsync() => GetSampleResourceAsStringAsync("error.xml");
        public static Task<string> GetSimListResponseAsync() => GetSampleResourceAsStringAsync("simListResponse.xml");
    }
}
