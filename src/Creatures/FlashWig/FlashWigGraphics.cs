namespace Nyctophobia;

public class FlashWigGraphics : DropBugGraphics
{
    private int FirstMandibleLightSprite;
    private int PatternMeshSprite;
    private int PatternType;

    private new readonly FlashWig bug;
    private readonly LightSource[] MandibleLights = new LightSource[2];
    private readonly float[] MandibleLightFlicker = new float[2];
    private readonly LightSource[] BodyLights = new LightSource[2];
    private readonly float[] BodyLightFlicker = new float[2];

    public FlashWigGraphics(PhysicalObject ow) : base(ow)
    {
        bug = (FlashWig)ow;

        var state = Random.state;
        Random.InitState(bug.abstractCreature.ID.RandomSeed);

        antennaeLength = Mathf.Lerp(1f, 2f, Random.value);
        coloredAntennae = Random.value < 0.5f ? Random.value : 0.1f;
        PatternType = Random.Range(1, 4);
        if (Random.value < 0.03f)
        {
            PatternType = -1;
        }

        Random.state = state;
    }

    private int MandibleLightSprite(int side, int part)
    {
        return FirstMandibleLightSprite + side * 2 + part;
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);

        FirstMandibleLightSprite = sLeaser.sprites.Length;
        PatternMeshSprite = sLeaser.sprites.Length + 4;

        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 5);

        for (var side = 0; side < 2; side++)
        {
            sLeaser.sprites[MandibleSprite(side, 1)] = new FSprite("FlashWig_MandibleB");
            sLeaser.sprites[MandibleLightSprite(side, 0)] = new FSprite("FlashWig_MandibleALights");
            sLeaser.sprites[MandibleLightSprite(side, 1)] = new FSprite("FlashWig_MandibleBLights");

            sLeaser.sprites[PincherSprite(side)].RemoveFromContainer();
            sLeaser.sprites[PincherSprite(side)] = TriangleMesh.MakeLongMesh(8, pointyTip: false, customColor: true);
        }

        var patternMesh = TriangleMesh.MakeLongMesh(12, pointyTip: false, customColor: false);
        sLeaser.sprites[PatternMeshSprite] = patternMesh;
        patternMesh.element = Futile.atlasManager.GetElementWithName("FlashWig_PatternMesh" + PatternType);
        for (var i = patternMesh.vertices.Length - 1; i >= 0; i--)
        {
            var perc = i / 2 / (float)(patternMesh.vertices.Length / 2);

            Vector2 uv;
            if (i % 2 == 0)
                uv = new Vector2(perc, 0f);
            else if (i < patternMesh.vertices.Length - 1)
                uv = new Vector2(perc, 1f);
            else
                uv = new Vector2(1f, 0f);

            // Map UV values to the element
            uv.x = Mathf.Lerp(patternMesh.element.uvBottomLeft.x, patternMesh.element.uvTopRight.x, uv.x);
            uv.y = Mathf.Lerp(patternMesh.element.uvBottomLeft.y, patternMesh.element.uvTopRight.y, uv.y);

            patternMesh.UVvertices[i] = uv;
        }

        AddToContainer(sLeaser, rCam, null);
    }

    public override void AddToContainer(SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);

        if (sLeaser.sprites.Length <= FirstMandibleLightSprite) return;

        sLeaser.sprites[PatternMeshSprite].MoveInFrontOfOtherNode(sLeaser.sprites[MeshSprite]);

        for (var side = 0; side < 2; side++)
        {
            for (var part = 0; part < 2; part++)
            {
                sLeaser.sprites[MandibleLightSprite(side, part)].MoveInFrontOfOtherNode(sLeaser.sprites[MandibleSprite(side, part)]);
            }

            sLeaser.sprites[PincherSprite(side)].MoveBehindOtherNode(sLeaser.sprites[MeshSprite]);
        }
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        var value = Mathf.Lerp(lastDeepCeilingMode, deepCeilingMode, timeStacker);
        value = Custom.SCurve(Mathf.InverseLerp(0.1f, 0.5f, value), 0.4f);

        for (var side = 0; side < 2; side++)
        {
            for (var part = 0; part < 2; part++)
            {
                sLeaser.sprites[MandibleLightSprite(side, part)].x = sLeaser.sprites[MandibleSprite(side, part)].x;
                sLeaser.sprites[MandibleLightSprite(side, part)].y = sLeaser.sprites[MandibleSprite(side, part)].y;
                sLeaser.sprites[MandibleLightSprite(side, part)].anchorY = sLeaser.sprites[MandibleSprite(side, part)].anchorY;
                sLeaser.sprites[MandibleLightSprite(side, part)].rotation = sLeaser.sprites[MandibleSprite(side, part)].rotation;
                sLeaser.sprites[MandibleLightSprite(side, part)].scaleX = sLeaser.sprites[MandibleSprite(side, part)].scaleX;
                sLeaser.sprites[MandibleLightSprite(side, part)].scaleY = sLeaser.sprites[MandibleSprite(side, part)].scaleY;
                sLeaser.sprites[MandibleLightSprite(side, part)].color = bug.EffectColor;
                sLeaser.sprites[MandibleLightSprite(side, part)].alpha = bug.LightIntensity * (part == 0 ? 0.85f : 1);
            }

            var antenna = (TriangleMesh)sLeaser.sprites[AntennaSprite(side)];
            for (var j = 0; j < 18; j++)
            {
                var vertex = antenna.verticeColors.Length - 1 - j;
                var defaultColor = Color.Lerp(currSkinColor, shineColor, Mathf.InverseLerp(antenna.verticeColors.Length / 2, antenna.verticeColors.Length - 1, vertex) * coloredAntennae * Mathf.Lerp(1f, 0.75f, value));
                antenna.verticeColors[vertex] = Color.Lerp(bug.EffectColor, defaultColor, 0.5f + j * 0.025f);
            }

            sLeaser.sprites[WingSprite(side)].isVisible = false;

            var pincher = (TriangleMesh)sLeaser.sprites[PincherSprite(side)];

            for (var i = 0; i < pincher.verticeColors.Length; i++)
            {
                pincher.verticeColors[i] = Color.Lerp(currSkinColor, Color.Lerp(bug.EffectColor, currSkinColor, i * 2.5f / pincher.verticeColors.Length - 0.2f), bug.LightIntensity);
            }
        }

        for (var i = 0; i < 10; i++)
        {
            sLeaser.sprites[SegmentSprite(i)].isVisible = false;
            sLeaser.sprites[ShineMeshSprite].isVisible = false;
        }

        var bodyMesh = (TriangleMesh)sLeaser.sprites[MeshSprite];
        var patternMesh = (TriangleMesh)sLeaser.sprites[PatternMeshSprite];

        patternMesh.color = bug.EffectColor;
        patternMesh.alpha = bug.LightIntensity;

        for (var i = 0; i < bodyMesh.vertices.Length && i < patternMesh.vertices.Length; i++)
        {
            patternMesh.vertices[i] = bodyMesh.vertices[i];
        }
    }

    public override void Update()
    {
        base.Update();

        for (var side = 0; side < MandibleLights.Length; side++)
        {
            var mandibleLight = MandibleLights[side];
            MandibleLightFlicker[side] = Mathf.Clamp(MandibleLightFlicker[side] + Random.Range(-0.1f, 0.1f), 0.5f, 1.5f);

            if (mandibleLight != null)
            {
                mandibleLight.stayAlive = true;
                mandibleLight.setRad = bug.LightIntensity * 50 * MandibleLightFlicker[side];
                mandibleLight.setPos = mandibles[side].pos;
                mandibleLight.alpha = bug.LightIntensity < 0.1f ? 0 : 0.6f;
                if (mandibleLight.slatedForDeletetion)
                {
                    MandibleLights[side] = null;
                }
            }
            else
            {
                MandibleLights[side] = mandibleLight = new LightSource(bug.mainBodyChunk.pos, environmentalLight: false, bug.EffectColor, bug);
                mandibleLight.requireUpKeep = true;
                mandibleLight.setRad = 0;
                mandibleLight.setAlpha = 0.6f;
                bug.room.AddObject(mandibleLight);
            }

            var bodyLight = BodyLights[side];
            BodyLightFlicker[side] = Mathf.Clamp(BodyLightFlicker[side] + Random.Range(-0.1f, 0.1f), 0.5f, 1.5f);

            if (bodyLight != null)
            {
                bodyLight.stayAlive = true;
                bodyLight.setRad = bug.LightIntensity * 100 * BodyLightFlicker[side];
                bodyLight.setPos = bug.bodyChunks[side + 1].pos;
                bodyLight.alpha = bug.LightIntensity < 0.1f ? 0 : 0.6f;
                if (bodyLight.slatedForDeletetion)
                {
                    BodyLights[side] = null;
                }
            }
            else
            {
                BodyLights[side] = bodyLight = new LightSource(bug.mainBodyChunk.pos, environmentalLight: false, bug.EffectColor, bug);
                bodyLight.requireUpKeep = true;
                bodyLight.setRad = 0;
                bodyLight.setAlpha = 0.6f;
                bug.room.AddObject(bodyLight);
            }
        }
    }
}