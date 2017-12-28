using System;
using System.Collections.Generic;

namespace ACTransit.Framework.Infrastructure.FileSystem
{
    public class SearchReplaceList : List<SearchReplace>
    {
        private static SearchReplaceList searchReplaceList;

        public virtual void AddPair(string Key, string Value)
        {
            Add(new SearchReplace { SearchFor = Key, ReplaceWith = Value });
        }

        /// <summary>
        ///     Load from file. Content alternatives between Search and Replace lines in pairs.
        /// </summary>
        /// <param name="File">File to load search/replace strings from</param>
        public static SearchReplaceList Get(string File)
        {
            if (searchReplaceList != null) return searchReplaceList;
            searchReplaceList = new SearchReplaceList();
            string search = "";
            try
            {
                foreach (string line in System.IO.File.ReadAllLines(File))
                {
                    if (search == "")
                        search = line.Trim();
                    else
                    {
                        string replace = line.Trim();
                        if (search != "")
                            searchReplaceList.AddPair(search, replace);
                        search = "";
                    }
                }
            }
            catch //(Exception e)
            {
            }
            return searchReplaceList;
        }
    }
}
