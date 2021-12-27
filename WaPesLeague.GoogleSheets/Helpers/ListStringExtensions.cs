using System;
using System.Collections.Generic;
using System.Linq;

namespace WaPesLeague.GoogleSheets.Helpers
{
    public static class ListStringExtensions
    {
        public static List<string> MakeAllValuesUnique(this List<string> list, char cahrToAdd)
        {
            var listUniqueValues = new List<string>();
            foreach (var item in list)
            {
                if (!listUniqueValues.Any(x => string.Equals(x, item, StringComparison.InvariantCultureIgnoreCase)))
                {
                    listUniqueValues.Add(item);
                }
                else
                {
                    string updatedName = item;
                    do
                    {
                        updatedName += cahrToAdd;

                    } while (listUniqueValues.Any(x => string.Equals(x, updatedName, StringComparison.InvariantCultureIgnoreCase)) || list.Any(it => string.Equals(it, updatedName, StringComparison.InvariantCultureIgnoreCase)));
                    listUniqueValues.Add(updatedName);
                }
            }

            return listUniqueValues;
        }
    }
}
