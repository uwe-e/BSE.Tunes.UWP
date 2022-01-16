namespace BSE.Tunes.StoreApp.Models.Contract
{
    public class Query
    {
        public string SearchPhrase
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }
    }
}
