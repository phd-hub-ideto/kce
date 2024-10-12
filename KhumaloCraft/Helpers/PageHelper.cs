namespace KhumaloCraft.Helpers;

public static class PageHelper
{
    public static int CalculatePageNumber(int take, int skip)
    {
        return (take + skip) / take;
    }

    public static int CalculatePageCount(int totalItemCount, int take)
    {
        return (int)Math.Ceiling(totalItemCount / (double)take);
    }

    public static int CalculateTotalItemCount(int totalPages, int take)
    {
        return totalPages * take;
    }

    public static int CalculateSkip(int take, int pageNumber)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));

        return (pageNumber - 1) * take;
    }

    public static int CalculateIndexCircularPaging(int pageNumber, int totalItemCount, int take = 1)
    {
        var index = (CalculateSkip(take, pageNumber) + 1) % totalItemCount - 1;

        if (index == -1)
        {
            index = totalItemCount - 1;
        }

        return index;
    }
}
