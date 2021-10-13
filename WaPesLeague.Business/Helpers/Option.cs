namespace WaPesLeague.Business.Helpers
{
    public class Option
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Option(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
