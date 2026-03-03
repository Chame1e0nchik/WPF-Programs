using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameOnWPF.Components;

namespace GameOnWPF.Components
{
    public class Tilemap
    {
        public int tileSize { get; }
        public int mapWidth { get; }
        public int mapHeight { get; }

        // tileset per tile type
        private readonly Dictionary<int, TileSet> tileSets = new();

        private readonly int[,] map;
        private readonly bool[,] collisionMap;

        private readonly bool closedWorld = true;

        public Tilemap(int tileSize, int mapWidth, int mapHeight)
        {
            this.tileSize = tileSize; // increase tile size to make canvas (window) closer look (16 - exact size as screen pixel, so the whole map will fit in whole screen)
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;

            map = new int[this.mapWidth, this.mapHeight];
            collisionMap = new bool[this.mapWidth, this.mapHeight];

            LoadTiles();
        }

        public DrawingGroup Build(Uri colorMapUri)
        {
            LoadMapFromPNG(colorMapUri);

            var tilemap = new DrawingGroup();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int type = map[x, y];
                    if (!tileSets.ContainsKey(type)) continue;

                    Drawing drawing = ResolveTile(type, x, y);
                    tilemap.Children.Add(drawing);
                }
            }

            return tilemap;
        }

        private void LoadTiles()
        {
            // grass
            tileSets[0] = new TileSet
            {
                Center = LoadSprite("Grass_tile.png"),
                EdgeLeft = LoadSprite("Grass_tile.png"),
                EdgeTop = LoadSprite("Grass_tile.png"),
                EdgeDown = LoadSprite("Grass_tile.png"),
                CornerTopLeftInside = LoadSprite("Grass_tile.png"),
                CornerDownLeftInside = LoadSprite("Grass_tile.png"),
                CornerTopLeftOutside = LoadSprite("Grass_tile.png"),
                CornerDownLeftOutside = LoadSprite("Grass_tile.png")
            };

            // water
            tileSets[1] = new TileSet
            {
                Center = LoadSprite("Water_tile.png"),
                EdgeLeft = LoadSprite("Water_tile_left.png"), // left edge
                EdgeTop = LoadSprite("Grass_tile_edge_top.png"), // edge top
                EdgeDown = LoadSprite("Grass_tile_edge_down.png"), // edge bottom
                CornerTopLeftInside = LoadSprite("Water_tile_inside-left.png"), // top-left corner
                CornerDownLeftInside = LoadSprite("Water_tile_down-left.png"), // down-left corner
                CornerTopLeftOutside = LoadSprite("Water_tile_down-left-outside.png"), // top-left corner outside
                CornerDownLeftOutside = LoadSprite("Water_tile_top-left-outside.png") // down-left corner outside
            };

            // rock
            tileSets[2] = new TileSet
            {
                Center = LoadSprite("Rock_tile.png"),
                EdgeLeft = LoadSprite("Rock_tile.png"),
                EdgeTop = LoadSprite("Rock_tile.png"),
                EdgeDown = LoadSprite("Rock_tile.png"),
                CornerTopLeftInside = LoadSprite("Rock_tile.png"),
                CornerDownLeftInside = LoadSprite("Rock_tile.png"),
                CornerTopLeftOutside = LoadSprite("Rock_tile.png"),
                CornerDownLeftOutside = LoadSprite("Rock_tile.png")
            };
        }

        private BitmapImage LoadSprite(string name)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Sprites/{name}"));
        }

        private void LoadMapFromPNG(Uri uri)
        {
            var bmp = new BitmapImage(uri);

            int stride = bmp.PixelWidth * 4;
            byte[] pixels = new byte[stride * bmp.PixelHeight];

            bmp.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int i = y * stride + x * 4;

                    byte b = pixels[i];
                    byte g = pixels[i + 1];
                    byte r = pixels[i + 2];
                    int tileType = ColorToTile(r, g, b);

                    // value of pixel on this position will be set as follows: 0, 1, 2
                    map[x, y] = tileType;
                    collisionMap[x, y] = IsCollidable(tileType);
                }
            }
        }

        private int ColorToTile(byte r, byte g, byte b)
        {
            if (r == 0 && g == 255 && b == 0) return 0; // grass
            if (r == 0 && g == 0 && b == 255) return 1; // water
            if (r == 139 && g == 69 && b == 19) return 2; // rock
            return -1;
        }

        private bool IsCollidable(int tileType)
        {
            return tileType == 0; // grass is solid
        }

        public bool IsBlocked(double worldX, double worldY)
        {
            int tileX = (int)(worldX / tileSize);
            int tileY = (int)(worldY / tileSize);

            if (tileX < 0 || tileY < 0 || tileX >= mapWidth || tileY >= mapHeight)
                return closedWorld; // outside map -> collision

            return collisionMap[tileX, tileY];
        }


        #region Neigbor comparing and replacement
        private Drawing ResolveTile(int type, int x, int y)
        {
            var set = tileSets[type];
            var rect = new Rect(x * tileSize, y * tileSize, tileSize, tileSize);

            bool up = IsSame(type, x, y - 1);
            bool right = IsSame(type, x + 1, y);
            bool down = IsSame(type, x, y + 1);
            bool left = IsSame(type, x - 1, y);

            bool upLeft = IsSame(type, x - 1, y - 1);
            bool upRight = IsSame(type, x + 1, y - 1);
            bool downLeft = IsSame(type, x - 1, y + 1);
            bool downRight = IsSame(type, x + 1, y + 1);

            BitmapImage tile = set.Center!;
            Transform? transform = null;

            // OUTSIDE CORNERS (priority)

            if (up && left && !upLeft)
            {
                tile = set.CornerTopLeftOutside!;
            }
            else if (up && right && !upRight)
            {
                tile = set.CornerTopLeftOutside!;
                transform = FlipX(rect);
            }
            else if (down && left && !downLeft)
            {
                tile = set.CornerDownLeftOutside!;
            }
            else if (down && right && !downRight)
            {
                tile = set.CornerDownLeftOutside!;
                transform = FlipX(rect);
            }

            // INSIDE CORNERS

            else if (!up && !left)
            {
                tile = set.CornerTopLeftInside!;
                transform = FlipX(rect);
            }
            else if (!up && !right)
            {
                tile = set.CornerTopLeftInside!;
            }
            else if (!down && !left)
            {
                tile = set.CornerDownLeftInside!;
                transform = FlipX(rect);
            }
            else if (!down && !right)
            {
                tile = set.CornerDownLeftInside!;
            }

            // EDGES

            else if (!left)
            {
                tile = set.EdgeLeft!;
                transform = FlipX(rect);
            }
            else if (!right)
            {
                tile = set.EdgeLeft!;
            }
            else if (!up)
            {
                tile = set.EdgeDown!;
            }
            else if (!down)
            {
                tile = set.EdgeTop!;
            }

            var group = new DrawingGroup();
            group.Children.Add(new ImageDrawing(tile, rect));

            if (transform != null)
                group.Transform = transform;

            return group;
        }

        private bool IsSame(int type, int x, int y)
        {
            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
                return closedWorld;

            return map[x, y] == type;
        }


        private int GetNeighborMask(int type, int x, int y)
        {
            int mask = 0;

            if (y > 0 && map[x, y - 1] != type) mask |= 1; // up
            if (x < mapWidth - 1 && map[x + 1, y] != type) mask |= 2; // right
            if (y < mapHeight - 1 && map[x, y + 1] != type) mask |= 4; // down
            if (x > 0 && map[x - 1, y] != type) mask |= 8; // left

            // if tile have neighbor as up and left -> mask = 1 + 8 = 9
            return mask;
        }

        private static ScaleTransform FlipX(Rect rect)
        {
            return new ScaleTransform(-1, 1, rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
        #endregion
    }
}
