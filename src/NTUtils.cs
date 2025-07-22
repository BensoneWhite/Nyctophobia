namespace Nyctophobia;

public static class NTUtils
{
    public static void UnregisterEnums(System.Type type)
    {
        IEnumerable<FieldInfo> extEnums = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType.IsSubclassOf(typeof(ExtEnumBase)));

        foreach (FieldInfo extEnum in extEnums)
        {
            object obj = extEnum.GetValue(null);
            if (obj != null)
            {
                _ = obj.GetType().GetMethod("Unregister")!.Invoke(obj, null);
                extEnum.SetValue(null, null);
            }
        }
    }

    public static void MapTextureColor(Texture2D texture, int alpha, Color32 to, bool apply = true)
    {
        Color32[] colors = texture.GetPixels32();

        for (int i = 0; i < colors.Length; i++)
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

    public static void Player_Exhausted(Player self)
    {
        self.airInLungs *= 0.2f;
        self.exhausted = true;
        self.aerobicLevel = 1f;
        self.lungsExhausted = true;
    }
}