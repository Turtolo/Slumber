using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotTiled;
using DotTiled.Serialization;

namespace Slumber;

public static class DotTiledBridge
{
  public static List<Node> Load(string path, Loader loader)
  {
    var map = loader.LoadMap(path);

    var nodes = new List<Node>();

    var objects = HandleObjects(map);
    var maps = map.Bridge(path);

    nodes.AddRange(objects);
    nodes.AddRange(maps);

    return nodes;
  }

  public static List<CollisionNode2D> HandleObjects(this Map map)
  {
    var nodes = new List<CollisionNode2D>();

    foreach (var baseLayer in map.Layers)
    {
      if (baseLayer is not ObjectLayer layer)
        continue;

      if (layer.Name == "Node")
      {
        foreach (DotTiled.Object obj in layer.Objects)
        {
          if (obj is PointObject point)
          {
            if (obj.Name == "enemy")
            {
              new Enemy().Set(n => n.Position = new Vector2(point.X, point.Y));
            }
          }

          if (obj is RectangleObject rect)
          {
            var shape = new CollisionShape2D().Set(n =>
            {
              n.Shape = new RectangleShape2D((int)rect.Width, (int)rect.Height);
            });

            if (obj.Name == "*")
            {
              var killZone = new KillZone().Set(n =>
              {
                n.Position = new Vector2(rect.X, rect.Y);
                n.AddChild(shape);
              });
            }

            if (obj.Name == "@")
            {
              if (obj.TryGetProperty("scene", out StringProperty sceneName))
              {
                var sceneTrans = new SceneChange().Set(n => 
                {
                  n.AddChild(shape);
                  n.Position = new Vector2(rect.X, rect.Y);
                  n.SceneName = sceneName.Value;
                });
              }
            }
          }
        }
      }

      foreach (DotTiled.Object obj in layer.Objects)
      {
        if (obj is not RectangleObject rect)
          continue;

        var shape = new CollisionShape2D().Set(n =>
        {
          n.Shape = new RectangleShape2D((int)rect.Width, (int)rect.Height);
        });

        if (layer.TryGetProperty("collision", out BoolProperty collision) && collision.Value == true)
        {
          var stat = new StaticBody2D().Set(n =>
          {
            n.Position = new Vector2(rect.X, rect.Y);
            n.AddLayer(10);
            n.AddLayer(1);
            n.AddChild(shape);
          });


          if (obj.TryGetProperty("one_way", out BoolProperty oneWay) && oneWay.Value == true)
          {
            foreach (var c in stat.CollisionShapes)
              c.OneWay = oneWay.Value;
          }

          nodes.Add(stat);
        }
      }
    }

    return nodes;
  }


  public static List<Tilemap> Bridge(this Map map, string path)
  {
    List<Tilemap> result = new();

    foreach (var layer in map.Layers)
    {
      if (layer is not TileLayer tileLayer || !tileLayer.Data.HasValue)
        continue;

      var (tileset, firstGid) = ResolveTileset(map, path);

      var tileLayerData = tileLayer.Data.Value;

      int[,] data;

      int offsetX = 0;
      int offsetY = 0;

      if (tileLayerData.Chunks.HasValue)
      {
        var chunks = tileLayerData.Chunks.Value;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var chunk in chunks)
        {
          minX = Math.Min(minX, chunk.X);
          minY = Math.Min(minY, chunk.Y);

          maxX = Math.Max(maxX, chunk.X + chunk.Width);
          maxY = Math.Max(maxY, chunk.Y + chunk.Height);
        }

        int width = maxX - minX;
        int height = maxY - minY;

        offsetX = minX;
        offsetY = minY;

        data = new int[width, height];

        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++)
          {
            data[x, y] = -1;
          }
        }

        foreach (var chunk in chunks)
        {
          var gids = chunk.GlobalTileIDs;

          for (int i = 0; i < gids.Length; i++)
          {
            uint gid = gids[i];

            int localX = i % chunk.Width;
            int localY = i / chunk.Width;

            int worldX = chunk.X + localX;
            int worldY = chunk.Y + localY;

            int x = worldX - minX;
            int y = worldY - minY;

            data[x, y] = gid == 0
              ? -1
              : (int)(gid - firstGid);
          }
        }
      }

      else if (tileLayerData.GlobalTileIDs.HasValue)
      {
        data = new int[tileLayer.Width, tileLayer.Height];

        var gids = tileLayerData.GlobalTileIDs.Value;

        for (int i = 0; i < gids.Length; i++)
        {
          int x = i % tileLayer.Width;
          int y = i / tileLayer.Width;

          uint gid = gids[i];

          data[x, y] = gid == 0
            ? -1
            : (int)(gid - firstGid);
        }
      }

      else
      {
        continue;
      }

      layer.TryGetProperty("layer", out IntProperty value);

      var tilemap = new Tilemap().Set(n =>
      {
        n.Name = tileLayer.Name;
        n.Tileset = tileset;

        n.Position = new Vector2(
          offsetX * map.TileWidth,
          offsetY * map.TileHeight
        );

        n.Depth = value != null ? value.Value : 0;

        n.SetData(data);
      });

      result.Add(tilemap);
    }

    return result;
  }


  private static List<Rectangle> GetRectangles(int[,] tiles)
  {
    int w = tiles.GetLength(0);
    int h = tiles.GetLength(1);

    bool[,] used = new bool[w, h];
    var rects = new List<Rectangle>();

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

        rects.Add(new Rectangle(x, y, maxX - x, maxY - y));
      }
    }

    return rects;
  }

  private static (Opal.Graphics.Tileset tileset, uint firstGid)
    ResolveTileset(Map map, string mapPath)
  {
    foreach (var ts in map.Tilesets)
    {
      var source = ts.Image.Value.Source;

      var mapsRoot = "Maps";
      var cleanedSource = source.Value.Replace("../", "");
      var imagePath = Path.Combine(mapsRoot, cleanedSource);

      var texture = Core.Resource.Load<MTexture>(
        imagePath.Replace(".png", "")
      );

      var t = new Opal.Graphics.Tileset(
        texture,
        ts.TileWidth,
        ts.TileHeight
      );

      return (t, ts.FirstGID.Value);
    }

    throw new Exception("No tilesets in map.");
  }
}
