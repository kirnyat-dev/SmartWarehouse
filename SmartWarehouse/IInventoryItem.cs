using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse
{
    public interface IInventoryItem
    {
        string Name { get; }
        decimal Price { get; }
        int Quantity { get; set; }
        CategoryInfo Category { get; }
    }
}