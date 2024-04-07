namespace Nyctophobia;

public class BlueLantern : Lantern
{
    public BlueLantern(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        color = new(0.196f, 0.596f, 0.965f);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        for (int i = 0; i < flicker.GetLength(0); i++)
        {
            flicker[i, 1] = flicker[i, 0];
            flicker[i, 0] += Mathf.Pow(UnityEngine.Random.value, 3f) * 0.09f * ((UnityEngine.Random.value < 0.5f) ? (-1f) : 1f);
            flicker[i, 0] = Custom.LerpAndTick(flicker[i, 0], flicker[i, 2], 0.060f, 71f / (678f * (float)Math.PI));
            if (UnityEngine.Random.value < 0.2f)
            {
                flicker[i, 2] = 1f + Mathf.Pow(UnityEngine.Random.value, 3f) * 0.2f * ((UnityEngine.Random.value < 0.5f) ? (-1f) : 1f);
            }

            flicker[i, 2] = Mathf.Lerp(flicker[i, 2], 1f, 0.019f);
        }

        if (lightSource == null)
        {
            lightSource = new LightSource(base.firstChunk.pos, environmentalLight: false, new Color(0.196f, 0.596f, 0.965f), this)
            {
                affectedByPaletteDarkness = 0.5f
            };
            room.AddObject(lightSource);
        }
        else
        {
            lightSource.setPos = base.firstChunk.pos;
            lightSource.setRad = 370f * flicker[0, 0];
            lightSource.setAlpha = 1f;
            if (lightSource.slatedForDeletetion || lightSource.room != room)
            {
                lightSource = null;
            }
        }

        lastRotation = rotation;
        if (stick != null)
        {
            base.firstChunk.pos = stick.po.pos;
            base.firstChunk.vel *= 0f;
            rotation = (stick.po.data as PlacedObject.ResizableObjectData).handlePos.normalized;
            base.firstChunk.collideWithTerrain = false;
            base.firstChunk.collideWithObjects = false;
            canBeHitByWeapons = false;
            return;
        }

        base.firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        if (grabbedBy.Count > 0)
        {
            rotation = Custom.PerpendicularVector(Custom.DirVec(base.firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
            rotation.y = 0f - Mathf.Abs(rotation.y);
        }

        if (setRotation.HasValue)
        {
            rotation = setRotation.Value;
            setRotation = null;
        }

        rotation = (rotation - Custom.PerpendicularVector(rotation) * ((base.firstChunk.ContactPoint.y < 0) ? 0.15f : 0.05f) * base.firstChunk.vel.x).normalized;
        if (base.firstChunk.ContactPoint.y < 0)
        {
            base.firstChunk.vel.x *= 0.8f;
        }

        if (abstractPhysicalObject is AbstractConsumable && grabbedBy.Count > 0 && !(abstractPhysicalObject as AbstractConsumable).isConsumed)
        {
            (abstractPhysicalObject as AbstractConsumable).Consume();
        }
    }
}
