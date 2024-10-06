using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sharper.Systems.Backend.Management
{
    public class ResourceManager
    {
        private Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
        private static ResourceManager instance;
        public static ResourceManager Instance
        {
            get { return instance; }
            private set { instance = value; }
        }
        public ResourceManager()
        {
            if (instance == null) instance = this;
        }

        public void LoadTextures(Dictionary<string, Texture2D> textures)
        {
            loadedTextures = textures;
        }

        public Texture2D GetTexture(string name)
        {
            return loadedTextures[name];
        }

        public void LoadTexture(string texName, Texture2D texture)
        {
            loadedTextures.Add(texName, texture);
        }
        public static string GetResourceFullPath(string relativePath)
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, $@"..\..\..\Resources\{relativePath}");
            string sFilePath = Path.GetFullPath(sFile);
            return sFilePath;
        }
    }
}
