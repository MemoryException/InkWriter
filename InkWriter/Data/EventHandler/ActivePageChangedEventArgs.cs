using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkWriter.Data.EventHandler
{
    public class ActivePageChangedEventArgs : EventArgs
    {
        public ActivePageChangedEventArgs(int activePageIndex, Page activePage)
        {
            this.ActivePageIndex = activePageIndex;
            this.ActivePage = activePage;
        }

        public int ActivePageIndex { get; private set; }

        public Page ActivePage { get; private set; }
    }
}
