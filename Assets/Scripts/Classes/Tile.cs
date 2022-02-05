using UnityEngine;

namespace Classes
{
    public class Tile
    {
        public enum TileType
        {
            Wood,
            Clay,
            Hay,
            Coal,
            Water
        }
        public string name { get;  }
        public string resourcesName { get;  }
        public int resourcesCount { get;  }
        public string color { get;  }
        public TileType tileType { get; }

        public Tile(string name, string resourcesName, int resourcesCount, string color, TileType tileType)
        {
            this.name = name;
            this.resourcesName = resourcesName;
            this.resourcesCount = resourcesCount;
            this.color = color;
            this.tileType = tileType;
        }
    }
}