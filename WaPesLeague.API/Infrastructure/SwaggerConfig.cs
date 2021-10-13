namespace WaPesLeague.API.Infrastructure
{
    public class SwaggerConfig
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string XmlDocumentationPath { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactUrl { get; set; }

        public SwaggerConfig()
        {
            ContactName = "Contact Name";
            ContactEmail = "contact@email.com";
            ContactUrl = "https://www.google.com";
        }
    }
}
