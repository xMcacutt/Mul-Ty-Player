using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes.GamePlay
{
    public enum HeroState
    {
        Default = 0x0,
        Bite = 0x1,
        Sneak = 0x2,
        Walk = 0x3,
        Jog = 0x4,
        Run = 0x5,
        BreakTurn = 0x6,
        Jump = 7,
        GlideInit = 8, // literally hard coded extended glide
        Gliding = 9,
        LedgeHang = 10,
        Bouncing = 11,
        EatenByFlower = 12,
        FlowerLaunch = 13, // lerps you to last launch destination set
        DoggyPaddling = 14, // no swim unlock
        SurfaceSwimming = 15,
        SurfaceDive = 16,
        UnderwaterSwimming = 17,
        GlideMode = 18, // what the fuck is this
        Surfacing = 19,
        WaterBonk = 20,
        WaterRise = 21, // rises until you hit surface ?? maybe old version of state 22
        EnteringSharkCage = 22, // seeks bottom of cage
        SharkCageSettle = 23, // seeks top center of cage
        ExitingSharkCage = 24, // seeks bottom of cage
        EatenByLargeFish = 25,
        Falling = 26,
        FarFalling = 27, // includes the splat
        Dying = 28,
        Respawn = 29,
        ExitingDunny = 30,
        Bonked = 31, // eg running into cobweb
        KnockedOver = 32,
        CollectCelebration = 33,
        DeepBreath = 34, // during ingame cutscenes ????
        Idle = 35,
        PeeringOverLedge = 36,
        DoggyTreadingWater = 37,
        TreadingWater = 38,
        IdleUnderwater = 39,
        ShakingDry = 40,
        TheRotator = 41, // no idea, rotates ty to the left slightly???
        ChangeRang = 42, // does not lock movement
        CollectSecondRang = 43, // locks all movement, unescapable
        Waterslide = 44,
        Aiming = 45,
        Sliding = 46,
        AirDiving = 47,
        Faceplant = 48, // after a failed air bite / dive
        DiveLock = 49, // movement lock after entering water after an air dive
        CreditsLock = 50,
        Quicksand = 51,
        RexDiving = 52, // crashes outside of minigame
    }

    public enum BullState
    {
        Idle = 0,
        Walking = 1,
        Sprinting = 2, // max speed
        Airborne = 3, // includes tornado
        Bonked = 4, // running into pillar
        Splat = 5,
        Bonk = 6,
        Dying = 7,
        DunnyRespawn = 8,
        Charge = 9,
        PullingEmu = 10, // and also water tower
        Celebration = 11,
        Running = 12, // mid speed
        Gone = 13, // ??? turn invisible and cant move, can throw rang tho
    }

    public static class HeroStateUtil
    {
        public static bool HeroStateIsAirborne(HeroState hs)
        {
            switch (hs)
            {
                case HeroState.Gliding:
                case HeroState.GlideInit:
                case HeroState.GlideMode:
                case HeroState.AirDiving:
                case HeroState.Falling:
                case HeroState.FarFalling:
                case HeroState.Jump:
                    return true;
                default:
                    return false;
            }
        }
    }
}
