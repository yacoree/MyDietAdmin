using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDietAdmin
{
    public class DishComposition
    {
        public long ingredient_id { get; set; }
        public short? unit_id { get; set; }
        public short value { get; set; }
        public int weight { get; set; }
    }
}
