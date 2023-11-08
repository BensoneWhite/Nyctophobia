using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Witness
{
    public static class NWFlyLogic
    {
        public static void Init()
        {
            On.Player.UpdateMSC += Player_UpdateMSC;
        }

        private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
        {
            orig(self);

            if (!self.IsNightWalker(out var night))
            {
                return;
            }

            const float normalGravity = 0.9f;
            const float normalAirFriction = 0.999f;
            const float flightGravity = -0.25f;
            const float flightAirFriction = 0.83f;
            const float flightKickinDuration = 6f;

            if (night.CanFly)
            {
                if (self.animation == Player.AnimationIndex.HangFromBeam)
                {
                    night.preventFlight = 15;
                }
                else if (night.preventFlight > 0)
                {
                    night.preventFlight--;
                }

                if (night.isFlying)
                {
                    night.Wind.Volume = Mathf.Lerp(0f, 1f, night.currentFlightDuration / flightKickinDuration);

                    night.currentFlightDuration++;

                    self.AerobicIncrease(0.00001f);

                    self.gravity = Mathf.Lerp(normalGravity, flightGravity, night.currentFlightDuration / flightKickinDuration);
                    self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, night.currentFlightDuration / flightKickinDuration);


                    if (self.input[0].x > 0)
                    {
                        self.bodyChunks[0].vel.x += night.WingSpeed;
                        self.bodyChunks[1].vel.x -= 1f;
                    }
                    else if (self.input[0].x < 0)
                    {
                        self.bodyChunks[0].vel.x -= night.WingSpeed;
                        self.bodyChunks[1].vel.x += 1f;
                    }

                    if (self.room.gravity <= 0.5)
                    {
                        if (self.input[0].y > 0)
                        {
                            self.bodyChunks[0].vel.y += night.WingSpeed;
                            self.bodyChunks[1].vel.y -= 1f;
                        }
                        else if (self.input[0].y < 0)
                        {
                            self.bodyChunks[0].vel.y -= night.WingSpeed;
                            self.bodyChunks[1].vel.y += 1f;
                        }
                    }
                    else if (night.UnlockedVerticalFlight)
                    {
                        if (self.input[0].y > 0)
                        {
                            self.bodyChunks[0].vel.y += night.WingSpeed * 0.75f;
                            self.bodyChunks[1].vel.y -= 0.3f;
                        }
                        else if (self.input[0].y < 0)
                        {
                            self.bodyChunks[0].vel.y -= night.WingSpeed;
                            self.bodyChunks[1].vel.y += 0.3f;
                        }
                    }

                    night.wingStaminaRecoveryCooldown = 40;
                    night.wingStamina--;

                    if (!self.input[0].jmp || !night.CanSustainFlight())
                    {
                        night.StopFlight();
                    }
                    if (night.wingStamina > 0)
                    {
                        self.SubtractFood(1);
                    }
                }
                else
                {
                    night.Wind.Volume = Mathf.Lerp(1f, 0f, night.timeSinceLastFlight / flightKickinDuration);

                    night.timeSinceLastFlight++;

                    night.Wind.Volume = 0f;

                    if (night.wingStaminaRecoveryCooldown > 0)
                    {
                        night.wingStaminaRecoveryCooldown--;
                    }
                    else
                    {
                        night.wingStamina = Mathf.Min(night.wingStamina + night.WingStaminaRecovery, night.WingStaminaMax);
                    }

                    if (self.wantToJump > 0 && night.wingStamina > night.MinimumFlightStamina && night.CanSustainFlight())
                    {
                        night.InitiateFlight();
                    }

                    self.airFriction = normalAirFriction;
                    self.gravity = normalGravity;
                }
            }
        }
    }
}
