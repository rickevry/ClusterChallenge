namespace DAM2.Core.Shared.Settings
{
    public class GenericDataBaseSettings : IGenericDataBaseSettings
    {
        public string ConnectionName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

}
