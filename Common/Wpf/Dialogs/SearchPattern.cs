namespace Wpf.Dialogs
{
    public class SearchPattern
    {
        public SearchPattern(string title, string pattern)
        {
            this.Title = title;
            this.Pattern = pattern;
        }

        public string Title { get; private set; }
        public string Pattern { get; private set; }
    }
}