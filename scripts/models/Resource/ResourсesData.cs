using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CityGame.scripts.Models.Resource;

public class ResourсesData
{
    // Властивості ресурсів
    public ResourсesExtensions Properties { get; set; } = new();
    // Словник "тип ресурсу - клас"
    public Dictionary<ResourceType, ResourceTileInfo> ResourceTilesData { get; set; } = new();
    // Граф зв’язків для розташування станцій
    public ResourceGraph ResourceGraph { get; set; } = new();

    // Конструктор
    public ResourсesData()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            if (type == ResourceType.None) continue;
            ResourceTilesData[type] = new ResourceTileInfo();
        }
    }
    public void GiveFirstResource()
    {
        ResourceTilesData[ResourceType.People].Amount = 10;
        ResourceTilesData[ResourceType.Money].Amount = 50;
        ResourceTilesData[ResourceType.Food].Amount = 200;
    }
    public void BuildGraph() 
    {
        ResourceGraph.BuildGraph();
    }

    // Додати позицію
    public bool AddPosition(ResourceType type, Vector2I position)
    {
        bool canPlace = ResourceGraph.CanPlaceStation(position, ResourceTilesData);
        if (!canPlace) return false;

        ResourceTilesData[type].AddPosition(position, ResourceGraph.FoundNearestStockConsume(new ResourсeStation(position, Vector2I.Zero), type, ResourceTilesData));
        return true;
    }

    // Видалити позицію
    public bool ClearPosition(ResourceType type, Vector2I position)
    {
        bool canDelete = ResourceGraph.CanDeleteStation(position, ResourceTilesData);
        if (!canDelete) return false;

        Vector2I targetPosition = ResourceTilesData[type].Stantions.Find(s => s.Position == position)?.TargetPosition ?? Vector2I.Zero;

        if (targetPosition != Vector2I.Zero) ResourceTilesData[Properties.GetProperties(type).StockConsumeType].ClearTargetPosition(targetPosition);

        ResourceTilesData[type].ClearPosition(position);

        return true;
    }

    // Отримати тип ресурсу на позиції
    public ResourceType TypeAtPosition(Vector2I position)
    {
        foreach (var kvp in ResourceTilesData)
        {
            if (kvp.Value.ContainsPosition(position))
            {
                return kvp.Key;
            }
        }
        return ResourceType.None; 
    }

    // Перевірка станцій, які мають можливість вилючитись/включитись
    private void CheckStockConsume()
    {
        Dictionary<ResourceType, int> resourcesConsumeAmounts = new();
        Dictionary<ResourceType, bool> resourcesIsActive = new();

        foreach (ResourceType type in ResourceTilesData.Keys)
        {
            resourcesConsumeAmounts[type] = 0;
            resourcesIsActive[type] = true;
        }

        foreach (ResourceType type in ResourceTilesData.Keys)
        {
            ResourceType stockConsumeType = Properties.GetProperties(type).StockConsumeType;
            int stockConsume = Properties.GetProperties(type).StockConsume;
            int consumeAmount = resourcesConsumeAmounts[stockConsumeType];
            bool isActive = resourcesIsActive[stockConsumeType];

            // Якщо станція активна - перевіряємо, чи достатньо ресурсу для споживання, якщо ні - деактивуємо станцію
            foreach (ResourсeStation station in ResourceTilesData[type].Stantions.Where(s => s.IsActive))
            {
                if (isActive)
                {
                    if (ResourceTilesData[stockConsumeType].Amount < consumeAmount + stockConsume)
                    {
                        station.IsActive = false;
                        isActive = false;
                    }
                    else consumeAmount += stockConsume;
                }
                else station.IsActive = false;

                if (!ResourceTilesData[stockConsumeType].ContainsPosition(station.TargetPosition))
                {
                    station.TargetPosition = Vector2I.Zero;
                    station.IsActive = false;
                }
             }

            // Якщо станція неактивна, але займає сток - перевіряємо чи можна її активувати
            foreach (ResourсeStation station in ResourceTilesData[type].Stantions.Where(s => !s.IsActive && s.TargetPosition != Vector2I.Zero))
            {
                if (ResourceTilesData[stockConsumeType].Amount < consumeAmount + stockConsume) 
                {
                    isActive = false;
                    break;
                }
                else 
                {
                    consumeAmount += stockConsume;
                    station.IsActive = true; 
                }
            }

            // Якщо сток залишився - шукаємо станції без стока і створюємо нові зв’язки
            foreach (ResourсeStation station in ResourceTilesData[type].Stantions.Where(s => !s.IsActive && s.TargetPosition == Vector2I.Zero))
            {
                if (ResourceTilesData[stockConsumeType].Amount < consumeAmount + stockConsume) 
                {
                    isActive = false;
                    break;
                }
                else 
                {
                    consumeAmount += stockConsume;
                    station.TargetPosition = ResourceGraph.FoundNearestStockConsume(station, type, ResourceTilesData);
                }
            }
            resourcesConsumeAmounts[stockConsumeType] = consumeAmount;
            resourcesIsActive[stockConsumeType] = isActive;
        }
    }
    
    // Видалити ресурс усіх активних станцій від ресурсу витрати
    private void RemoveAmount()
    {
        foreach (ResourceType type in ResourceTilesData.Keys)
        {
            ResourceType consumeType = Properties.GetProperties(type).ConsumeType;
            int totalAmount = ResourceTilesData[consumeType].Amount;

            foreach (var station in ResourceTilesData[type].Stantions.Where(s => s.IsActive))
            {
                if (ResourceTilesData[type].Amount == Properties.GetProperties(type).Stock * ResourceTilesData[type].Stantions.Count + Properties.GetMinStock(type)) return;
                if (totalAmount >= Properties.GetProperties(type).Consume) totalAmount -= Properties.GetProperties(type).Consume;
                else station.IsActive = false;
            }
            ResourceTilesData[consumeType].Amount = totalAmount;
        }
    }

    // Додати ресурс із усіх активних станцій
    private void AddAmount()
    {
        foreach (ResourceType type in ResourceTilesData.Keys)
        {
            int totalAmount = ResourceTilesData[type].Amount;
            int stationCount = ResourceTilesData[type].Stantions.Count(s => s.IsActive);
            int maxAmount = Properties.GetProperties(type).Stock * ResourceTilesData[type].Stantions.Count + Properties.GetMinStock(type);
            
            for (int i = stationCount; i > 0; i--)
            {
                if (totalAmount + Properties.GetProperties(type).Yield <= maxAmount) totalAmount += Properties.GetProperties(type).Yield;
            }
            ResourceTilesData[type].Amount = totalAmount;
        }
    }

    // Дія кожного такту
    public void TickAction()
    {
        CheckStockConsume();
        RemoveAmount();
        AddAmount();
    }

    // Десеріалізація зі словника Godot
    public static ResourсesData FromGodotDictionary(Godot.Collections.Dictionary dict)
    {
        ResourсesData data = new();
        data.ResourceTilesData.Clear();
        
        // Реініціалізуємо всі типи ресурсів
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            if (type == ResourceType.None) continue;
            data.ResourceTilesData[type] = new ResourceTileInfo();
        }

        foreach (var key in dict.Keys)
        {
            string resName = key.ToString();

            if (!Enum.TryParse<ResourceType>(resName, out var type)) continue;

            var infoDict = dict[key].AsGodotDictionary();
            data.ResourceTilesData[type] = ResourceTileInfo.FromGodotDictionary(infoDict);
            
        }
        data.ResourceGraph = ResourceGraph.FromGodotDictionary(dict["ResourceGraph"].AsGodotDictionary());
        return data;
    }

    // Серіалізація в словник Godot
    public Godot.Collections.Dictionary ToGodotDictionary()
    {
        Godot.Collections.Dictionary dict = new();

        foreach (var kvp in ResourceTilesData) {
            if (kvp.Key == ResourceType.None) continue;
            dict[kvp.Key.ToString()] = kvp.Value.ToGodotDictionary();
        }
        
        dict["ResourceGraph"] = ResourceGraph.ToGodotDictionary();

        return dict;
    }
}



