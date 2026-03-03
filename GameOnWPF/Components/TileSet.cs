using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameOnWPF.Components
{
    public class TileSet
    {
        public BitmapImage? Center;

        public BitmapImage? EdgeTop; // left edge
        public BitmapImage? EdgeLeft; // left edge
        public BitmapImage? EdgeDown; // left edge

        public BitmapImage? CornerTopLeftInside; // top-left corner
        public BitmapImage? CornerDownLeftInside; // down-left corner

        public BitmapImage? CornerTopLeftOutside; // top-left corner
        public BitmapImage? CornerDownLeftOutside; // down-left corner
    }
}
