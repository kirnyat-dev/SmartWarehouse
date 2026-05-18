using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse
{
    public delegate void LowStockAlertHandler(string itemName, int currentQuantity);

    public class WarehouseManager<T> where T : class, IInventoryItem
    {
        private List<T> _items;

        public event LowStockAlertHandler OnLowStock;

        public WarehouseManager()
        {
            _items = new List<T>();
        }

        public void Add(T item)
        {
            if (item != null)
            {
                _items.Add(item);
            }
        }

        public void Remove(string name)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name == name)
                {
                    _items.RemoveAt(i);
                    return;
                }
            }
        }

        public void UpdateQuantity(string name, int newQuantity)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name == name)
                {
                    _items[i].Quantity = newQuantity;

                    if (newQuantity <= 5)
                    {
                        if (OnLowStock != null)
                        {
                            OnLowStock.Invoke(_items[i].Name, _items[i].Quantity);
                        }
                    }
                    return;
                }
            }
        }

        public IEnumerable<T> GetAllItems() => _items;

        public IEnumerable<T> GetLowStockItems(int threshold) => _items
            .Where(x => x.Quantity < threshold)
            .OrderBy(x => x.Name);

        public IEnumerable<IGrouping<string, T>> GetItemsByCategory() => _items
            .GroupBy(x => x.Category.Name);

        public decimal GetTotalInventoryValue() => _items
            .Sum(x => x.Price * x.Quantity);

        public IEnumerable<string> GetTopCategoriesByValue(int count) => _items
            .GroupBy(x => x.Category.Name)
            .OrderByDescending(g => g.Sum(x => x.Price * x.Quantity))
            .Take(count)
            .Select(g => g.Key);
    }
}