using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Components;
using Sharper.ECS;
using Sharper.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sharper.Systems.Backend.Loaders
{
    public class WorldLoader
    {
        /* .map file formatting
         * mlc - map line counter, or just i from the for loop, tlc = tile line counter, used for data interpretation, tdc - tile data counter, used for deciding what data represents
         [MAP_START] mlc:0
         MAP_NAME: mlc:1
         [LAYER_START] mlc = X > 1;
         LAYER_TEX_NAME mlc: X+1;
         [TILE_START]mlc: >X+2 tlc:0
         [COLOR] tlc:1
         TILE_COLOR_R: tlc:2, tdc: 1
         TILE_COLOR_G: tlc:3, tdc: 2
         TILE_COLOR_B: tlc:4, tdc: 3
         [COMPONENTS] tlc:>5
         Transform (archetypes have default transform values)
         SpriteRenderer
         ATLAS_X tdc:1
         ATLAS_Y tdc:2
         Z_INDEX tdc:3
         [TILE_END]
         [LAYER_END]

         after reaching [TILE_END] line, the tile line counter should reset to 0 and look for another tile. if no other tiles are in this layer, than we look for another layer.
         */
        public Entity[] LoadMapFromTextureRefactored(string mapFile, out string mapName)
        {
            Stopwatch worldCreationTimer = new Stopwatch();
            worldCreationTimer.Start();
            mapName = "";
            List<string> layerTexNames = new List<string>(); //contains texture filenames for all the layers.
            List<TileInfo> layerTileInfo = new List<TileInfo>();//contains tile information for each layer.
            SpriteAtlasSettings currentSpriteAtlasSettings = RenderingSystem.Instance.SpriteAtlasSettings;
            List<string> mapFileData = new List<string>();
            List<Entity> worldMapEntities = new List<Entity>();
            int pps = currentSpriteAtlasSettings.pixelsPerSprite;

            //Initialize variables; read and decode raw file data
            mapFileData = File.ReadAllLines(GetMapFilePath(mapFile)).ToList();
            bool readyToReadMap = false;
            bool readyToReadLayer = false;
            bool readyToReadTile = false;
            MapDecodeState decodeState = MapDecodeState.None;
            int layerCount = -1; //number of layers found
            int tlc = 0; //tile data counter
            int tdc = 0;
            Entity tileArchetype = new Entity("Tile Archetype");
            Color entityColorCode = Color.White;
            string currentEntityArchetypeComponent = "";
            Debug.WriteLine($"[INFO]Reading map data from file {mapFile}.map");
            for (int mlc = 0; mlc < mapFileData.Count; mlc++)
            {
                if (mapFileData[mlc] == "[MAP_START]" && !readyToReadMap)
                {
                    readyToReadMap = true;
                    continue;
                }
                if (readyToReadMap)
                {
                    //read map name
                    if (mlc == 1) mapName = mapFileData[mlc];
                    //search for layer start
                    if (mapFileData[mlc] == "[LAYER_START]")
                    {
                        readyToReadLayer = true;
                        layerCount++;
                        continue;
                    }
                    //search for layer end
                    if (mapFileData[mlc] == "[LAYER_END]")
                    {
                        readyToReadLayer = false;
                        continue;
                    }
                    //if we found a layer
                    if (readyToReadLayer)
                    {
                        //search for tile data start
                        if (mapFileData[mlc] == "[TILE_START]")
                        {
                            readyToReadTile = true;
                            tlc++;
                            continue;
                        }
                        //found tile data end, add m_color code and archetype to layer id.
                        if (mapFileData[mlc] == "[TILE_END]")
                        {
                            //stop reading
                            readyToReadTile = false;
                            //reset counters
                            tlc = 0;
                            tdc = 0;
                            currentEntityArchetypeComponent = "";
                            decodeState = MapDecodeState.None;
                            //add archetype to layer
                            layerTileInfo.Add(new TileInfo(entityColorCode, tileArchetype, layerCount));
                            //reset entity archetype and m_color code
                            tileArchetype = new Entity("Tile Archetype");
                            entityColorCode = Color.White;
                            continue;
                        }
                        //if we found a tile
                        if (readyToReadTile)
                        {
                            //if still reading current data signature's data
                            if (decodeState != MapDecodeState.None)
                            {
                                if (decodeState == MapDecodeState.ReadingColor)
                                {
                                    switch (tdc)
                                    {
                                        case 1:
                                            entityColorCode.R = (byte)int.Parse(mapFileData[mlc].ToString());
                                            break;
                                        case 2:
                                            entityColorCode.G = (byte)int.Parse(mapFileData[mlc].ToString());
                                            break;
                                        case 3:
                                            entityColorCode.B = (byte)int.Parse(mapFileData[mlc].ToString());
                                            break;
                                        default:
                                            decodeState = MapDecodeState.None;
                                            tdc = 0;
                                            mlc--;
                                            continue;
                                    }
                                }
                                else if (decodeState == MapDecodeState.ReadingComponents)
                                {
                                    if (currentEntityArchetypeComponent == "")
                                        currentEntityArchetypeComponent = mapFileData[mlc].ToString();
                                    switch (currentEntityArchetypeComponent)
                                    {
                                        case "Transform":
                                            if (!tileArchetype.HasComponent<Transform>()) tileArchetype.AddComponent<Transform>();
                                            tdc = 0;
                                            currentEntityArchetypeComponent = "";
                                            break;
                                        case "MouseInteraction":
                                            if (!tileArchetype.HasComponent<MouseInteractable>()) tileArchetype.AddComponent<MouseInteractable>();
                                            tdc = 0;
                                            currentEntityArchetypeComponent = "";
                                            break;
                                        case "SpriteRenderer":
                                            if (!tileArchetype.HasComponent<SpriteRenderer>())
                                            {
                                                tileArchetype.AddComponent<SpriteRenderer>();
                                                mlc++;
                                            }
                                            switch (tdc)
                                            {
                                                case 1:
                                                    tileArchetype.GetComponent<SpriteRenderer>().sprite.atlasX = int.Parse(mapFileData[mlc].ToString());
                                                    break;
                                                case 2:
                                                    tileArchetype.GetComponent<SpriteRenderer>().sprite.atlasY = int.Parse(mapFileData[mlc].ToString());
                                                    break;
                                                case 3:
                                                    tileArchetype.GetComponent<SpriteRenderer>().sprite.zIndex = int.Parse(mapFileData[mlc].ToString());
                                                    break;
                                                default:
                                                    currentEntityArchetypeComponent = "";
                                                    tdc = 0;
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                tdc++;
                            }
                            else //if we read a signature of other data, or decodeState is not set.
                            {
                                decodeState = GetReadingState(mapFileData[mlc]);
                                tdc++;
                            }
                            tlc++;
                        }
                        else
                        {
                            //if this line isn't a tile start or end, than it's a layer texture name
                            layerTexNames.Add(mapFileData[mlc]);
                        }
                    }
                }
            }
            Debug.WriteLine($"[INFO]Successfully decoded map {mapName} file.");
            Debug.WriteLine("[INFO]Building World...");
            //Now that we know everything about the layers and tiles, we can start building our game world.
            //iterate layer textures and start building.
            for (int layerIndex = 0; layerIndex < layerTexNames.Count; layerIndex++)
            {
                Texture2D layerTexture = ResourceManager.Instance.GetTexture(layerTexNames[layerIndex]);
                //initialize 1D array of m_color data
                Color[] texturePixelData = new Color[layerTexture.Width * layerTexture.Height];
                //load pixel m_color data into the linear array
                layerTexture.GetData<Color>(texturePixelData);
                for (int x = 0; x < layerTexture.Width; x++)
                {
                    for (int y = 0; y < layerTexture.Height; y++)
                    {
                        int linearIndex = (y * layerTexture.Width) + x; //calculate the linear index for the pixel m_color data
                        //compare pixel m_color data with layer tile data
                        if (layerTileInfo.Exists(x => x.layerIndex == layerIndex && x.tileColor == texturePixelData[linearIndex]))
                        {
                            //grab tile info if a match  is found
                            TileInfo reference = layerTileInfo.Find(x => x.layerIndex == layerIndex && x.tileColor == texturePixelData[linearIndex]);
                            //create a new tile from archetype
                            Entity newWorldTile = CreateNewTileEntityFromArchetype(reference.tileArchetype);
                            //set the world position  of the new tile
                            newWorldTile.GetComponent<Transform>().Position = new System.Numerics.Vector3(x * pps, y * pps, 0f);
                            //add the tile to return list
                            worldMapEntities.Add(newWorldTile);
                        }
                    }
                }
            }
            worldCreationTimer.Stop();
            Debug.WriteLine($"[INFO]Done building world \"{mapName}\" with {worldMapEntities.Count} entities in {worldCreationTimer.Elapsed.TotalSeconds}s");
            return worldMapEntities.ToArray();
        }
        private Entity CreateNewTileEntityFromArchetype(Entity entity)
        {
            //TODO: Add copy constructors to components: Transform, SpriteRenderer, Collider
            //See which components are needed
            Type[] neededComponents = entity.GetTypesOfActiveComponents();
            //Create a new entity
            Entity newTileEntity = new Entity("World Tile Entity", neededComponents.ToArray());
            for (int i = 0; i < neededComponents.Length; i++)
            {
                Type componentType = neededComponents[i];
                Component component = newTileEntity.GetComponent<Component>(componentType);
                component.CopyComponentData(entity.GetComponent<Component>(componentType));
            }
            return newTileEntity;
        }
        public MapDecodeState GetReadingState(string line)
        {
            MapDecodeState state = MapDecodeState.None;
            switch (line)
            {
                case "[COLOR]":
                    state = MapDecodeState.ReadingColor;
                    break;
                case "[COMPONENTS]":
                    state = MapDecodeState.ReadingComponents;
                    break;
            }
            return state;
        }

        public string GetMapFilePath(string fileName)
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, $@"..\..\..\Maps\{fileName}.map");
            string sFilePath = Path.GetFullPath(sFile);
            return sFilePath;
        }
    }
    public struct TileInfo
    {
        public TileInfo(Color tColor, Entity tArchetype, int layerIndex)
        {
            this.layerIndex = layerIndex;
            tileColor = tColor;
            tileArchetype = tArchetype;
        }
        public int layerIndex;
        public Color tileColor;
        public Entity tileArchetype;
    }

    public enum MapDecodeState
    {
        None,
        ReadingColor,
        ReadingComponents,
        TileComplete
    }
}
