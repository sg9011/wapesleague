namespace WaPesLeague.Business.Helpers
{
    public class ProcessFileImportResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public ProcessFileImportResult()
        {

        }

        public ProcessFileImportResult IsSuccessful()
        {
            Success = true;
            ErrorMessage = null;

            return this;
        }

        public ProcessFileImportResult Failed(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;

            return this;
        }
    }
}
