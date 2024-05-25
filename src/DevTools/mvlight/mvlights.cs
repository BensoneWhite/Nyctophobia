using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace VoidSea;

public class MVLight : VoidSeaScene.VoidSeaSceneElement
{
    public MVLight(VoidSeaScene voidSeaScene, Vector2 pos, float depth) : base(voidSeaScene, pos, depth)
    {
    //reference lines 517+ on lights.cs
        this.voidSea = voidSea;
        behavior = new BGMVLightBehavior(this);
        lightAlpha = 1f;


        
    }
    public VoidSeaScene voidSea;
    public float vel = 1f;
    public MVLightBehavior behavior;
    public float lightAlpha;
    public int[] lightSprites;
    public abstract class MVLightBehavior
    {
        public MVLight mvlight;
        public Vector2 goalpos;
    
        public VoidSeaScene voidSea => mvlight.voidSea;

        public MVLightBehavior(MVLight mvlight) 
        {
            this.mvlight = mvlight;
            goalpos = mvlight.pos;
        
        }
        public virtual void Update()
        {

        }
    }
    public class BGMVLightBehavior : MVLightBehavior
    {
        public BGMVLightBehavior (MVLight mvlight) : base(mvlight)
        {
        }
        public override void Update()
        {
            base.Update();

        }
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
        behavior.Update();

    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];

        sLeaser.sprites[lightSprites[0]] = new FSprite("Futile_White");
        sLeaser.sprites[lightSprites[0]].shader = rCam.game.rainWorld.Shaders["FlatLight"];
        sLeaser.sprites[lightSprites[1]] = new FSprite("Futile_White");
        sLeaser.sprites[lightSprites[1]].shader = rCam.game.rainWorld.Shaders["FlatWaterLight"];
        sLeaser.sprites[lightSprites[2]] = new FSprite("Futile_White");
        sLeaser.sprites[lightSprites[2]].shader = rCam.game.rainWorld.Shaders["FlatLight"];
        base.InitiateSprites(sLeaser, rCam);
    
    }
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //bsed for now
        Vector2 vector12;
        vector12.x = 1f;
        vector12.y = 1f;
        float dark = 0f;
        float lightDimmed = 0f;
        vector12 = (vector12 - voidSea.convergencePoint) / depth + voidSea.convergencePoint;
        for (int num12 = 0; num12 < lightSprites.Length; num12++)
        {
            sLeaser.sprites[lightSprites[num12]].x = vector12.x;
            sLeaser.sprites[lightSprites[num12]].y = vector12.y;
            sLeaser.sprites[lightSprites[num12]].color = new Color(1f - dark * 0.5f, 1f - dark * 0.5f, 1f - dark * 0.5f);
        }
        float scale = 1f;
        sLeaser.sprites[lightSprites[0]].scale = scale * Mathf.Lerp(350f, 120f, lightDimmed) / (8f * depth);
        sLeaser.sprites[lightSprites[1]].scale = scale * Mathf.Lerp(300f, 120f, lightDimmed) / (8f * depth);
        sLeaser.sprites[lightSprites[2]].scale = scale * Mathf.Lerp(150f, 80f, lightDimmed) / (8f * depth);
        if (lightAlpha != 1f)
        {
            for (int num13 = 0; num13 < sLeaser.sprites.Length; num13++)
            {
                sLeaser.sprites[num13].alpha = lightAlpha;
                sLeaser.sprites[num13].color = new Color(lightAlpha, lightAlpha, lightAlpha);
            }
        }
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
} 
