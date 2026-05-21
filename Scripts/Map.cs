using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MonoTile;
using Tileset = Amethyst.Graphics.Tileset;

namespace Slumber
{
  public static class MapLoader
  {

    public static List<Tilemap> ToTMap(this List<MonoMap> maps)
    {
      var tileMaps = new List<Tilemap>();

      foreach (var map in maps)
      {
        var texturePath = Path.Combine(map.FilePath, map.TileSet.ImagePath);

        if (texturePath.StartsWith(Core.Resource.ContentRoot + Path.DirectorySeparatorChar) ||
            texturePath.StartsWith(Core.Resource.ContentRoot + "/"))
        {
          texturePath = texturePath.Substring(Core.Resource.ContentRoot.Length + 1);
        }

        texturePath = Path.ChangeExtension(texturePath, null);

        Match match = Regex.Match(map.Name, @"(?<=_)(\d+)");

        int result = match.Success ? int.Parse(match.Value) : 0;

        var tMap = new Tilemap().Set(n =>
        {
          n.Name = result.ToString();
          n.Depth = result;
          n.IndexOffset = map.IndexOffset;
          n.Tileset = new Tileset(
            Core.Resource.Load<MTexture>(texturePath),
              16, 16
          );
          n.SetData(map.Grid);
        });


        if (map.Properties.TryGetValue("collider", out var value) && value is bool collision && collision == true)
        {
          var tileRects = GetRectangles(map.Grid);

          foreach (var rect in tileRects)
            Core.Index.Create<StaticBody2D>().Set(n =>
            {
              n.AddChild(Core.Index.Create<CollisionShape2D>().Set(c =>
                {
                  c.Shape = new RectangleShape2D(rect.Width * map.TileSet.TileWidth, rect.Height * map.TileSet.TileHeight);

                }));
              n.Position = new Vector2(rect.X * map.TileSet.TileWidth, rect.Y * map.TileSet.TileHeight);
              n.SetParent(tMap);
            });
        }

        tileMaps.Add(tMap);
      }

      return tileMaps;
    }

    public static List<Rectangle> GetRectangles(int[,] tiles)
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
  }
}
