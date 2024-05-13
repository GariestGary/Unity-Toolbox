namespace VolumeBox.Toolbox
{
    public static class StaticData
    {
        public const string SettingsResourcesPath = "Toolbox/Settings.asset";
        
        public static SettingsData Settings => ResourcesUtils.ResolveScriptable<SettingsData>(SettingsResourcesPath);
        public static bool HasSettings => ResourcesUtils.HasScriptable(SettingsResourcesPath);

        public static void CreateSettings()
        {
            ResourcesUtils.ResolveScriptable<SettingsData>(SettingsResourcesPath);
        }
    }
}