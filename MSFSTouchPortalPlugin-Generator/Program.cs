namespace MSFSTouchPortalPlugin_Generator {
  class Program {
    private const string _PLUGIN_NAME = "MSFSTouchPortalPlugin";
    static void Main(string[] args) {
      var entryGenerator = new GenerateEntry(_PLUGIN_NAME);
      entryGenerator.Generate();

      var docGenerator = new GenerateDoc(_PLUGIN_NAME);
      docGenerator.Generate();
    }
  }
}
