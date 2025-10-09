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
            { "articuno", new ImageClass { ImageFile = "articuno.png" } },
            { "caterpie", new ImageClass { ImageFile = "caterpie.png" } },
            { "charmeleon", new ImageClass { ImageFile = "charmeleon.png" } },
            { "eevee", new ImageClass { ImageFile = "eevee.png" } },
            { "ivysaur", new ImageClass { ImageFile = "ivysaur.png" } },
            { "jigglypuff", new ImageClass { ImageFile = "jigglypuff.png" } },
            { "nidoran", new ImageClass { ImageFile = "nidoran.png" } },
            { "pigeot", new ImageClass { ImageFile = "pigeot.png" } },
            { "pikachu", new ImageClass { ImageFile = "pikachu.png" } },
            { "wartortle", new ImageClass { ImageFile = "wartotle.png" } }
        };
    }
}
