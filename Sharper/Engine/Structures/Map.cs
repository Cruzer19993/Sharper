using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.ECS;
using Sharper.Helpers;
using Sharper.Systems.Backend;
using Sharper.Systems.Backend.Management;

namespace Sharper.Source.Structures
{
    class Map
    {
        public Map(int size)
        {
            this.size = size;
            AddLayer();
        }
        public int size;
        public List<Layer> mapLayers = new List<Layer>();
        public void Export()
        {

        }

        public void AddLayer()
        {
            mapLayers.Add(new Layer(size, mapLayers.Count, "Layer"));
        }

        public void RemoveLayer()
        {

        }
    }

    public class Layer
    {
        public Layer(int size,int layerIndex,string layerName)
        {
            CreateTileGrid(size, size);
            this.layerIndex = layerIndex;
        }
        void CreateTileGrid(int x, int y)
        {
            int pps = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
            int linearIndex = x * y;
            for (int i = 0; i < linearIndex; i++)
            {
                int posx = i % x;
                int posy = (int)(i / x);
                Entity tile = EntityHelper.CreateGridTile(new Vector3(posx * pps, posy * pps, 0f));
                layerEntitiesIDs.Add(tile.entityID);
            }
        }

        public void SetVisible(bool visible)
        {
            foreach(int key in layerEntitiesIDs)
            {
                SceneManager.CurrentScene.entityManager.entities[key].GetComponent<EntityRenderer>().m_allowRendering = visible;
            }
        }

        public int layerIndex;
        public string layerName;
        public bool visible;
        public List<int> layerEntitiesIDs = new List<int>(); 
    }
}
