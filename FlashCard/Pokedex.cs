using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard
{
    public class Pokedex
    {
        public static Dictionary<string, ImageClass> pokedex = new Dictionary<string, ImageClass>()
        {
            { "articuno", new ImageClass { ImageFile = "articuno_black.png" } },
            { "caterpie", new ImageClass { ImageFile = "caterpie_black.png" } },
            { "charmeleon", new ImageClass { ImageFile = "charmeleon_black.png" } },
            { "eevee", new ImageClass { ImageFile = "eevee_black.png" } },
            { "ivysaur", new ImageClass { ImageFile = "ivysaur_black.png" } },
            { "jigglypuff", new ImageClass { ImageFile = "jigglypuff_black.png" } },
            { "nidoran", new ImageClass { ImageFile = "nidoran_black.png" } },
            { "pigeot", new ImageClass { ImageFile = "pigeot_black.png" } },
            { "pikachu", new ImageClass { ImageFile = "pikachu_black.png" } },
            { "wartortle", new ImageClass { ImageFile = "wartortle_black.png" } }
        };
    }
}
