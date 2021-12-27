namespace WaPesLeague.Business.Dto.Server
{
    public class AddServerButtonDto
    {
        public string Message { get; set; }
        public string URL { get; set; }
        public decimal? UseRateOverwrite { get; set; }

        public AddServerButtonDto()
        {
            UseRateOverwrite = null;
        }
    }
}
