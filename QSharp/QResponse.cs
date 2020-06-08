using System;
using System.Text.RegularExpressions;

namespace QSharp
{
    public class QResponse
    {
        private const string WORKSPACES_REGEX = @"/workspaces";
        public string status { get; set; }
        public string address { get; set; }
        public object[] data { get; set; }

        public string getReplyType()
        {
            Console.WriteLine($"QRESPONSE - Address to match: '{address}'");
            Match m = Regex.Match(address, WORKSPACES_REGEX);
            if (m.Success)
                return "WORKSPACES";
            return "";
        }
    }
}
