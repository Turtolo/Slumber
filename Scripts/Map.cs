using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoTile;

namespace Slumber
{
    public static class Map
    {
        public static List<Tilemap> LoadMap(string path)
        {
            List<Tilemap> maps = new();

            var data = TiledLoader.Extract(path);

            var tileset = data.Tilesets.FirstOrDefault();
            var source = tileset.ImageSource;

            var root = Engine.Resource.ContentRoot;
            var directory = Path.GetDirectoryName(path);
            var texturePath = Path.Combine(directory, source);

            if (texturePath.StartsWith(root + Path.DirectorySeparatorChar) ||
                texturePath.StartsWith(root + "/"))
            {
                texturePath = texturePath.Substring(root.Length + 1);
            }

            texturePath = Path.ChangeExtension(texturePath, null);
            
            foreach (var layer in data.Layers)
            {
                var map = Engine.Table.Create<Tilemap>().Set(n =>
                {
                    n.IndexOffset = tileset.FirstGid; 
                    n.Tileset = new Tileset(
                        Engine.Resource.Load<MTexture>(texturePath),
                        16, 16
                    );
                    n.SetData(layer.Tiles);
                });

                var props = layer.Properties;
                if (props.TryGetValue("collider", out var value) && value is bool collision && collision == true)
                {
                
                    var tileRects = GetRectangles(layer.Tiles);

                    foreach (var rect in tileRects)
                        Engine.Table.Create<StaticBody2D>().Set(n =>
                        {
                            n.AddChild(Engine.Table.Create<CollisionShape2D>().Set(c =>
                            {
                                c.Shape = new RectangleShape2D(rect.width * tileset.TileWidth, rect.height * tileset.TileHeight);
                            }));
                            n.LocalPosition = new Vector2(rect.x * tileset.TileWidth, rect.y * tileset.TileHeight);
                            n.SetParent(map);
                        });
                }
            }      

            return maps;
        }

        public static List<(int x, int y, int width, int height)> GetRectangles(int[,] tiles)
        {
            int w = tiles.GetLength(0);
            int h = tiles.GetLength(1);

            bool[,] used = new bool[w, h];
            var rects = new List<(int, int, int, int)>();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (used[x, y] || tiles[x, y] == 0)
                        continue;

                    int maxX = x;
                    while (maxX < w && tiles[maxX, y] != 0 && !used[maxX, y])
                        maxX++;

                    int maxY = y;
                    bool canExpand = true;

                    while (canExpand && maxY < h)
                    {
                        for (int i = x; i < maxX; i++)
                        {
                            if (tiles[i, maxY] == 0 || used[i, maxY])
                            {
                                canExpand = false;
                                break;
                            }
                        }

                        if (canExpand)
                            maxY++;
                    }

                    for (int yy = y; yy < maxY; yy++)
                    {
                        for (int xx = x; xx < maxX; xx++)
                        {
                            used[xx, yy] = true;
                        }
                    }

                    rects.Add((x, y, maxX - x, maxY - y));
                }
            }

            return rects;
        }
    }
}