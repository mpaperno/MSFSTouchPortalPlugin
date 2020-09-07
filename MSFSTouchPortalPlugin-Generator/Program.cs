namespace MSFSTouchPortalPlugin_Generator {
  class Program {
    private const string _PLUGIN_NAME = "MSFSTouchPortalPlugin";
    private static string _TargetPath = "..\\..\\..\\..\\";
    static void Main(string[] args) {
      if (args.Length >= 1) {
        _TargetPath = args[0];
      }

      var entryGenerator = new GenerateEntry(_PLUGIN_NAME, _TargetPath);
      entryGenerator.Generate();

      var docGenerator = new GenerateDoc(_PLUGIN_NAME, _TargetPath);
      docGenerator.Generate();
    }
  }
}
