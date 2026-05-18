using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse
{
    internal class Program
    {
        static void ShowAlert(string itemName, int currentQuantity)
        {
            Console.WriteLine($"[уведомление]: товар '{itemName}' заканчивается! остаток: {currentQuantity} шт.");
        }

        static void Main(string[] args)
        {
            WarehouseManager<IInventoryItem> Manager = new WarehouseManager<IInventoryItem>();

            Manager.OnLowStock += ShowAlert;

            CategoryInfo BooksCategory = new CategoryInfo("книги", "b01");
            CategoryInfo TechCategory = new CategoryInfo("комплектующие", "t02");

            Book B1 = new Book("чистый код", 1200m, 15, BooksCategory);
            Book B2 = new Book("совершенный код", 1500m, 3, BooksCategory);
            HardwarePart H1 = new HardwarePart("видеокарта", 45000m, 8, TechCategory);
            HardwarePart H2 = new HardwarePart("процессор", 22000m, 2, TechCategory);

            Manager.Add(B1);
            Manager.Add(B2);
            Manager.Add(H1);
            Manager.Add(H2);

            Console.WriteLine("текущие товары на складе:");
            foreach (var Item in Manager.GetAllItems())
            {
                Console.WriteLine($"товар: {Item.Name} | цена: {Item.Price} | количество: {Item.Quantity} | категория: {Item.Category}");
            }

            Console.WriteLine("\nдемонстрация работы событий");
            Console.WriteLine("изменяем количество товара 'видеокарта' на 4 (порог сработает):");
            Manager.UpdateQuantity("видеокарта", 4);

            Console.WriteLine("\nотписываемся от уведомлений и меняем количество товара 'процессор' на 1:");
            Manager.OnLowStock -= ShowAlert;
            Manager.UpdateQuantity("процессор", 1);

            Manager.OnLowStock += ShowAlert;

            Console.WriteLine("\nаналитика склада (linq)");

            Console.WriteLine("1. товары с остатком меньше 5 шт:");
            var LowStock = Manager.GetLowStockItems(5);
            foreach (var Item in LowStock)
            {
                Console.WriteLine($" -> {Item.Name} (остаток: {Item.Quantity})");
            }

            Console.WriteLine("\n2. общая стоимость всех товаров на складе:");
            decimal Total = Manager.GetTotalInventoryValue();
            Console.WriteLine($" -> общая сумма: {Total} руб.");

            Console.WriteLine("\n3. группировка товаров по категориям:");
            var CategoryGroup = Manager.GetItemsByCategory();
            foreach (var Group in CategoryGroup)
            {
                Console.WriteLine($"категория: {Group.Key}");
                foreach (var Item in Group)
                {
                    Console.WriteLine($"  - {Item.Name} ({Item.Quantity} шт.)");
                }
            }

            Console.WriteLine("\n4. топ-1 категория по суммарной стоимости:");
            var TopCategories = Manager.GetTopCategoriesByValue(1);
            foreach (var CatName in TopCategories)
            {
                Console.WriteLine($" -> самая дорогая категория: {CatName}");
            }
        }
    }
}