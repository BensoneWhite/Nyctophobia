﻿namespace Nyctophobia;

public class RedFlareBombAbstract : AbstractConsumable
{
    public RedFlareBombAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableObjectData) : base(world, NTEnums.AbstractObjectType.RedFlareBomb, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableObjectData)
    {
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new RedFlareBomb(this, world);
    }
}
