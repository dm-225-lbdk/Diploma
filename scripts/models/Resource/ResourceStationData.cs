using System;
namespace CityGame.scripts.Models.Resource;

public class ResourсesExtensions
{
    // характеристики станцій
    public ResourceStationProperty GetProperties(ResourceType type)
    {
        return type switch
        {
            ResourceType.Fuel => new ResourceStationProperty 
            { 
                Yield = 5, 
                ConsumeType = ResourceType.Fuel, 
                Consume = 0, 
                Stock = 100, 
                StockConsumeType = ResourceType.People, 
                StockConsume = 2
            },
            ResourceType.Ore => new ResourceStationProperty
            {
                Yield = 3, 
                ConsumeType = ResourceType.Fuel, 
                Consume = 1, 
                Stock = 50, 
                StockConsumeType = ResourceType.People, 
                StockConsume = 2
            },
            ResourceType.Ingot => new ResourceStationProperty 
            { 
                Yield = 3, 
                ConsumeType = ResourceType.Ore, 
                Consume = 6, 
                Stock = 30, 
                StockConsumeType = ResourceType.People, 
                StockConsume = 3 
            },
            ResourceType.Food => new ResourceStationProperty 
            { 
                Yield = 15, 
                ConsumeType = ResourceType.Money, 
                Consume = 1, 
                Stock = 400, 
                StockConsumeType = ResourceType.People, 
                StockConsume = 2 
            },
            ResourceType.Money => new ResourceStationProperty 
            { 
                Yield = 10, 
                ConsumeType = ResourceType.Ingot, 
                Consume = 3, 
                Stock = 1000, 
                StockConsumeType = ResourceType.People, 
                StockConsume = 2
            },
            ResourceType.People => new ResourceStationProperty 
            { 
                Yield = 1, 
                ConsumeType = ResourceType.Food, 
                Consume = 5,
                Stock = 5,
                StockConsumeType = ResourceType.Money,
                StockConsume = 50
            },
            _ => new ResourceStationProperty 
            { 
                Yield = -1, 
                ConsumeType = ResourceType.None, 
                Consume = -1,
                Stock = -1,
                StockConsumeType = ResourceType.None,
                StockConsume = -1
            }
        };

    }

    public int GetMinStock(ResourceType type)
    {
        return type switch
        {
            ResourceType.Fuel => 50,
            ResourceType.Ore => 20,
            ResourceType.Ingot => 5,
            ResourceType.Food => 200,
            ResourceType.Money => 50,
            ResourceType.People => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    // вивід назви ресурсу
    // public string ToString(ResourceType type)
    // {
    //     return type switch
    //     {
    //         ResourceType.Fuel => "Паливо",
    //         ResourceType.Ore => "Руда",
    //         ResourceType.Ingot => "Злитки",
    //         ResourceType.Food => "Їжа",
    //         ResourceType.Money => "Гроші",
    //         ResourceType.People => "Люди",
    //         _ => type.ToString()
    //     };
    // }

}

public class ResourceStationProperty
{
    // Видобуток
    public int Yield { get; set; } 
    // Тип ресурсу споживання
    public ResourceType ConsumeType { get; set; } 
    // Споживання
    public int Consume { get; set; } 
    // Запас станції
    public int Stock { get; set; } 
    // Тип зарезервованого ресурсу
    public ResourceType StockConsumeType { get; set; } 
    // Зарезервований ресурс
    public int StockConsume { get; set; } 
}