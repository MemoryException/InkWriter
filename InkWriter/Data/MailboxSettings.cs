using System.Collections.Generic;
using System.Xml.Serialization;

namespace InkWriter.Data
{
    [XmlRoot()]
    public class MailboxSettings
    {
        public string PopServer { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public List<string> Receivers { get; private set; }
    }
}