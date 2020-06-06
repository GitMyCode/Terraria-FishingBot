using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

public interface ISearchStrategy
{
    Task<SearchResult> Search(Bitmap screen);

    Task<SearchResult> Search(Bitmap screen, TeraPixel pixel);
}