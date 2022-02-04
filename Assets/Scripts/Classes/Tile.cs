using UnityEngine;

namespace Classes
{
    public class Tile
    {
        public Tile(string name, string resourcesName, int resourcesCount, string color)
        {
            this.name = name;
            this.resourcesName = resourcesName;
            this.resourcesCount = resourcesCount;
            this.color = color;
        }

        public string name { get;  }

        public string resourcesName { get;  }

        public int resourcesCount { get;  }

        public string color { get;  }
    }
}