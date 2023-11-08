using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Witness
{
    public static class NWUtils
    {
        public static bool PlayerHasCustomTail(PlayerGraphics pg)
        {
            if (!ModManager.ActiveMods.Any(x => x.id == "dressmyslugcat"))
            {
                return true;
            }

            return PlayerHasCustomTailDMS(pg);
        }

        public static bool PlayerHasCustomTailDMS(PlayerGraphics pg)
        {
            return !(DressMySlugcat.Customization.For(pg)?.CustomTail?.EffectiveCustTailShape ?? false);
        }

        public static void MapTextureColor(Texture2D texture, int alpha, Color32 to, bool apply = true)
        {
            var colors = texture.GetPixels32();

            for (var i = 0; i < colors.Length; i++)
            {
                if (colors[i].a == alpha)
                {
                    colors[i] = to;
                }
            }

            texture.SetPixels32(colors);

            if (apply)
            {
                texture.Apply(false);
            }
        }
    }
}
