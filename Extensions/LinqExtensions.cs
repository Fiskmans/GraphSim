using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> ThatAre<T,U>(this IEnumerable<U> self)
            where T : class
            where U : class

        {
            return self.Where(o => o is T).Select(o => o as T);
        }
    }
}
