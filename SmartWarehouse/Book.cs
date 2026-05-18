using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse
{
    public class Book : IInventoryItem
    {
        private string _name;
        private decimal _price;
        private int _quantity;
        private CategoryInfo _category;

        public Book(string name, decimal price, int quantity, CategoryInfo category)
        {
            if (price < 0 || quantity < 0)
            {
                Console.WriteLine("ошибка: цена и количество не могут быть отрицательными");
                _name = "неизвестно";
                _price = 0;
                _quantity = 0;
                _category = new CategoryInfo("нет", "000");
                return;
            }

            _name = name;
            _price = price;
            _quantity = quantity;
            _category = category;
        }

        public string Name => _name;

        public decimal Price => _price;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value >= 0)
                {
                    _quantity = value;
                }
            }
        }

        public CategoryInfo Category => _category;
    }
}