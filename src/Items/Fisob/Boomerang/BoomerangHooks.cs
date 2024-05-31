namespace Nyctophobia;

public static class BoomerangHooks
{
	public static void Apply()
	{
		On.AbstractPhysicalObject.AbstractObjectStick.FromString += AbstractObjectStick_FromString;
	}

	private static void AbstractObjectStick_FromString(On.AbstractPhysicalObject.AbstractObjectStick.orig_FromString orig, string[] splt, AbstractRoom room)
	{
		orig(splt, room);

		var entityID = EntityID.FromString(splt[2]);
		var entityID2 = EntityID.FromString(splt[3]);
		AbstractPhysicalObject abstractPhysicalObject = null;
		AbstractPhysicalObject abstractPhysicalObject2 = null;
		foreach (var entity in room.entities)
		{
			if (abstractPhysicalObject != null && abstractPhysicalObject2 != null)
			{
				break;
			}

			if (entity is AbstractPhysicalObject)
			{
				if (entity.ID == entityID)
				{
					abstractPhysicalObject = entity as AbstractPhysicalObject;
				}
				else if (entity.ID == entityID2)
				{
					abstractPhysicalObject2 = entity as AbstractPhysicalObject;
				}
			}
		}

		if (abstractPhysicalObject == null || abstractPhysicalObject2 == null)
		{

			return;
		}

		switch (splt[1])
		{
			case "boomerangLdgStk":
                new Boomerang.AbstractBoomerangStick(abstractPhysicalObject, abstractPhysicalObject2, int.Parse(splt[4], NumberStyles.Any, CultureInfo.InvariantCulture), int.Parse(splt[5], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(splt[6], NumberStyles.Any, CultureInfo.InvariantCulture)).unrecognizedAttributes = SaveUtils.PopulateUnrecognizedStringAttrs(splt, 7);
				break;
			case "boomerangLdgAppStk":
                new Boomerang.AbstractBoomerangAppendageStick(abstractPhysicalObject, abstractPhysicalObject2, int.Parse(splt[4], NumberStyles.Any, CultureInfo.InvariantCulture), int.Parse(splt[5], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(splt[6], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(splt[7], NumberStyles.Any, CultureInfo.InvariantCulture)).unrecognizedAttributes = SaveUtils.PopulateUnrecognizedStringAttrs(splt, 8);
				break;
		}
	}
}