using System.Collections.Generic;

namespace WaPesLeague.Data.Helpers
{
    public class EndDbSessionResult
    {
        public bool Success { get; set; }
        public List<int> SingedOutUsers { get; set; }

        public EndDbSessionResult(bool success, List<int> signedOutUsers = null)
        {
            Success = success;
            SingedOutUsers = signedOutUsers;
        }
    }
}
