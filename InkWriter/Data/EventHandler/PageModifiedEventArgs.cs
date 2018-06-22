using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkWriter.Data.EventHandler
{
    public class PageModifiedEventArgs : EventArgs
    {
        public PageModifiedEventArgs(Page modifiedPage)
        {
            this.ModifiedPage = modifiedPage;
        }

        public Page ModifiedPage { get; private set; }
    }
}
