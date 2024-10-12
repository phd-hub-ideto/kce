using KhumaloCraft.Helpers;
using System.Collections.Specialized;

namespace KhumaloCraft.Application.Http
{
    public static class QueryStringBuilder
    {

        public static NameValueCollection Create(string queryString = null)
        {
            // only way to construct an instance of the internal System.Web.HttpValueCollection class
            return System.Web.HttpUtility.ParseQueryString(queryString ?? string.Empty);
        }

        public static NameValueCollection AddToQuery<T>(this NameValueCollection queryString, string key, T? value, Func<bool> check = null) where T : struct
        {
            if (value != null && (check == null || check()))
            {
                queryString.Add(key, value.ToString());
            }

            return queryString;
        }

        public static NameValueCollection AddToQuery(this NameValueCollection queryString, string key, string value, Func<bool> check = null)
        {
            if (!string.IsNullOrWhiteSpace(value) && (check == null || check()))
            {
                queryString.Add(key, value);
            }

            return queryString;
        }

        public static NameValueCollection AddToQuery<T>(this NameValueCollection queryString, string key, T[] values, Func<bool> check = null) where T : struct
        {
            if (values?.Count() > 0 && (check == null || check()))
            {
                foreach (var value in values)
                {
                    queryString.Add(key, value.ToString());
                }
            }

            return queryString;
        }

        public static NameValueCollection AddToQuery(this NameValueCollection queryString, string key, string[] values, Func<bool> check = null)
        {
            if (values?.Count() > 0 && (check == null || check()))
            {
                foreach (var value in values)
                {
                    queryString.Add(key, value);
                }
            }

            return queryString;
        }

        public static string Build(this NameValueCollection queryString)
        {
            return queryString.ToString();
        }

        public static NameValuePair[] BuildArray(this NameValueCollection queryString)
        {
            return queryString.ToNameValuePairs().ToArray();
        }
    }
}
