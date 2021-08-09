using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectorLib;
using CrowdControl.Common;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ConnectorType = CrowdControl.Common.ConnectorType;
using Log = CrowdControl.Common.Log;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;
#if DEBUG

#endif

namespace CrowdControl.Games.Packs
{
    [UsedImplicitly]
    public class Crystalis : NESEffectPack
    {
        public Crystalis([NotNull] IPlayer player, [NotNull] Func<CrowdControlBlock, bool> responseHandler, [NotNull] Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }



        //Position
        //private const ushort ADDR_PosXl = 0x0070;
        private const ushort ADDR_PosX = 0x0090;
        //private const ushort ADDR_PosYl = 0x00CF;
        private const ushort ADDR_PosY = 0x00EF;
        private const ushort ADDR_Controller = 0x0048;
        private const ushort ADDR_CURRENT_AREA = 0x006C;
        private const ushort ADDR_MENU = 0x01F9;

        //Sprites
        private const ushort ADDR_SPRITE_RAM = 0x0200;
        private const ushort ADDR_SPRITE_RAM_PATTERN = 0x0201;
        private const ushort ADDR_SPRITE_RAM_ATTRIBUTES = 0x0202;
        private const ushort ADDR_SPRITE_RAM_X = 0x0203;
        private const ushort ADDR_SPRITE_Y = 0x05e0;
        private const ushort ADDR_SPRITE_TYPE_BASE = 0x0000;
        private const ushort ADDR_SPRITE_STATUS_BASE = 0x000;
        private const ushort ADDR_SPRITE_POS_Y_LOW_BASE = 0x000;
        private const ushort ADDR_SPRITE_POS_Y_HIGH_BASE = 0x000;

        //UI
        private const ushort ADDR_Money1 = 0x0702;
        private const ushort ADDR_Money2 = 0x0703;
        private const ushort ADDR_EXP = 0x0704;
        private const ushort ADDR_HP = 0x03C1;
        private const ushort ADDR_MPC = 0x0708;
        private const ushort ADDR_MPF = 0x0709;
        private const ushort ADDR_HEALTH_MAX = 0x03C0;
        private const ushort ADDR_SHOP_CURRENT_PRICE1 = 0x6474;
        private const ushort ADDR_SHOP_CURRENT_PRICE2 = 0x6475;
        private const ushort ADDR_SHOP_ITEM1_PRICE1 = 0x6478;
        private const ushort ADDR_SHOP_ITEM1_PRICE2 = 0x6479;
        private const ushort ADDR_SHOP_ITEM2_PRICE1 = 0x647A;
        private const ushort ADDR_SHOP_ITEM2_PRICE2 = 0x647B;
        private const ushort ADDR_SHOP_ITEM3_PRICE1 = 0x647C;
        private const ushort ADDR_SHOP_ITEM3_PRICE2 = 0x647D;
        private const ushort ADDR_SHOP_ITEM4_PRICE1 = 0x647E;
        private const ushort ADDR_SHOP_ITEM4_PRICE2 = 0x647F;
        private const ushort ADDR_SCALING = 0x648F;
        private const ushort ADDR_LEVEL = 0x0421;
        private const ushort ADDR_U1HOOK = 0x6220;
        private const ushort ADDR_U2HOOK = 0x6221;
        private const ushort ADDR_U3HOOK = 0x6222;

        // Powerup
        private const ushort ADDR_SwordSlot1 = 0x6430;
        private const ushort ADDR_SwordSlot2 = 0x6431;
        private const ushort ADDR_SwordSlot3 = 0x6432;
        private const ushort ADDR_SwordSlot4 = 0x6433;
        private const ushort ADDR_PowerSlot1 = 0x643C;
        private const ushort ADDR_PowerSlot2 = 0x643D;
        private const ushort ADDR_PowerSlot3 = 0x643E;
        private const ushort ADDR_PowerSlot4 = 0x643F;
        private const ushort ADDR_ArmorSlot1 = 0x6434;
        private const ushort ADDR_ArmorSlot2 = 0x6435;
        private const ushort ADDR_ArmorSlot3 = 0x6436;
        private const ushort ADDR_ArmorSlot4 = 0x6437;
        private const ushort ADDR_ShieldSlot1 = 0x6438;
        private const ushort ADDR_ShieldSlot2 = 0x6439;
        private const ushort ADDR_ShieldSlot3 = 0x643A;
        private const ushort ADDR_ShieldSlot4 = 0x643B;

        //Equipable

        //Consumables
        private const ushort ADDR_ConsumeSlot1 = 0x6440;
        private const ushort ADDR_ConsumeSlot2 = 0x6441;
        private const ushort ADDR_ConsumeSlot3 = 0x6442;
        private const ushort ADDR_ConsumeSlot4 = 0x6443;
        private const ushort ADDR_ConsumeSlot5 = 0x6444;
        private const ushort ADDR_ConsumeSlot6 = 0x6445;
        private const ushort ADDR_ConsumeSlot7 = 0x6446;
        private const ushort ADDR_ConsumeSlot8 = 0x6447;

        //Spells
        private const ushort ADDR_Spell1 = 0x6458;
        private const ushort ADDR_Spell2 = 0x6469;
        private const ushort ADDR_Spell3 = 0x645A;
        private const ushort ADDR_Spell4 = 0x645B;
        private const ushort ADDR_Spell5 = 0x645C;
        private const ushort ADDR_Spell6 = 0x645D;
        private const ushort ADDR_Spell7 = 0x645E;
        private const ushort ADDR_Spell8 = 0x645F;

        //Status
        private const ushort ADDR_Condition = 0x0710;
        private const ushort ADDR_Warrior = 0x06C0;
        private const ushort ADDR_ChargeLvl = 0x0719;
        private const ushort ADDR_Speed = 0x0341;
        private const ushort ADDR_Jump = 0x0620;
        private const ushort ADDR_ScreenHit1 = 0x07D6;
        private const ushort ADDR_ScreenHit2 = 0x07D7;
        private const ushort ADDR_Equip_Sword = 0x0711;
        private const ushort ADDR_Stone = 0x05A0;

        //Boss
        private const ushort ADDR_BOSS_HEALTH1 = 0x03CD;
        private const ushort ADDR_BOSS_HEALTH2 = 0x03CE;

        //{
        //Need to fix Bosses for respawn later just a placeholder for now

        //private const ushort ADDR_Kelby = 0x0;
        //private const ushort ADDR_Sabera = 0x0;
        //private const ushort ADDR_Mado = 0x0;
        //private const ushort ADDR_Kelby2 = 0x0;
        //private const ushort ADDR_Sabera2 = 0x0;
        //private const ushort ADDR_Mado2 = 0x0;
        //private const ushort ADDR_Karmine = 0x0;
        //private const ushort ADDR_Draygon = 0x0;


        //private Dictionary<string, (string bossName, byte value, BossDefeated bossFlag, ushort address, byte limit)> _wType = new Dictionary<string, (string, byte, BossDefeated, ushort, byte)>(StringComparer.InvariantCultureIgnoreCase)
        //{
        //    {"Kelby", ("Kelby", 0, BossDefeated.Kelby, ADDR_Kelby, 0)},
        //    {"Sabera", ("Sabera", 1, BossDefeated.Sabera, ADDR_Sabera, 14)},
        //    {"Mado", ("Mado", 2, BossDefeated.Mado, ADDR_Mado, 14)},
        //    {"Kelby2", ("Kelby2,", 3, BossDefeated.Kelby2, ADDR_Kelby2, 14)},
        //    {"Sabera2", ( "Sabera2", 4, BossDefeated.Sabera2, ADDR_Sabera2, 14)},
        //    {"Mado2", ("Mado2", 5, BossDefeated.Mado2, ADDR_Mado2, 14)},
        //    {"Karmine", ("Karmine", 6, BossDefeated.Karmine, ADDR_Karmine, 14)},
        //    {"Draygon", ("Draygon", 7, BossDefeated.Draygon, ADDR_Draygon, 14)},
        //};

        //[Flags]
        //private enum BossLocation : byte
        //{
        //    Vamp1 = 0x0A,
        //    Bug = 0x1A,
        //    Kelby = 0x28,
        //    Vamp2 = 0x6C,
        //    Sabera = 0x6E,
        //    Mado = 0xF2,
        //    Kelby2 = 0xA9,
        //    Sabera2 = 0xAC,
        //    Mado2 = 0xB9,
        //    Karmine = 0xB6,
        //    Draygon = 0x9F,
        //    Draygon2 = 0xA6,
        //    Dyna = 0x5F            
        //}

        //private enum BossDefeated : byte
        //{
        //    Kelby = 0x01,
        //    Sabera = 0x02,
        //    Mado = 0x04,
        //    Kelby2 = 0x08,
        //    Sabera2 = 0x10,
        //    Mado2 = 0x20,
        //    Karmine = 0x40,
        //    Draygon = 0x80,
        //    All = 0xFF
        //}

        //    private bool SpawnEnemy() =>
        //                            Connector.Read8(ADDR_PosX, out ushort xPos) &&
        //                            Connector.Read8(ADDR_PosY, out ushort yPos) &&
        //                            SpawnSprite(SpriteName.FlyingDevil, (ushort)(xPos + 24u), yPos) ! = 0xFF;
        //                        
        //    private bool SpawnSprite (byte slot, SpriteName name, ushort xPos, ushort yPos)
        //                        {
        //                            bool result = Connector.Write8(ADDR_SPRITE_POS_X_LOW_BASE + slot, unchecked((byte)xPos));
        //                            result = result && Connector.Write8(ADDR_SPRITE_POS_X_HIGH_BASE + slot, unchecked((byte)(xpos >> 8)));
        //                            result = result && Connector.Write8(ADDR_SPRITE_POS_Y_LOW_BASE + slot, unchecked((byte)ypos));
        //                            result = result && Connector.Write8(ADDR_SPRITE_POS_Y_HIGH_BASE + slot, unchecked((byte)(ypos >> 8)));
        //                            result = result && Connector.Write8(ADDR_SPRITE_TYPE_BASE + slot, (byte)name);
        //                            result = result && Connector.Write8(ADDR_SPRITE_STATUS_BASE +slot, (byte)SpriteStatus.Spawning);
        //                            
        //                            return result;
        //                        }
        //                        
        //    private byte SpawnSprite(SpriteName name, ushort xPos, ushort ypos)
        //                       {
        //                            if (Connector.Isequal8(ADDR_SPRITE_STATUS_BASE + i, (byte)SpriteStatus.Empty)
        //                                && SpawnSprite (i, name, xPos, yPos))
        //                               {
        //                                    return i;
        //                                }
        //                            return 0xff;
        //                       }
        //        
        //   private enum SpriteName : byte
        //                        {
        //                        //Flying
        //                       BlueBat	= 0x1B327
        //                        RedFlyingBug	= 0x1B387,
        //                        GreenFlyingBug	= 0x1B3CD,            
        //                        FlyingHead	= 0x1B57B,
        //                        Red_PurpleBirds	= 0x1B592,
        //                        Moth	= 0x1B6C4,
        //                        BomberBird	= 0x1B72D,
        //                        FlyingDevil	= 0x1B7EF,
        //                        
        //                        //Walking
        //                        Mummy	= 0x1B94E,
        //                        Warlock	= 0x1B967,
        //                        DraygoniaArcher	= 0x1B714,
        //                        Green_PurpleFlailGuy	= 0x1B75B,
        //                        Burt	= 0x1B7A5,
        //                        Ninja	= 0x1B7D6,
        //                        Mimic	= 0x1B5E7,
        //                        Shadowmonster	= 0x1B6A7,
        //                        BlackKnight	= 0x1B8F0,
        //                        
        //                        //Bosses
        //                        Vampire	= 0x1B356,
        //                        BigBug	= 0x1B3FC,
        //                        Kelbesque	= 0x1B4EC,
        //                        Sabera	= 0x1B6DB,
        //                        Mado	= 0x1B7BD,               
        //                        KelbesqueTwo = 0x1B806,
        //                        SaberaTwo	= 0x1B84E,
        //                        MadoTwo	= 0x1B8BF,
        //                       Karmine = 0x1B920,
        //                        Draygon	= 0x1B980,
        //                        Dyna	= 0x1BA54,
        //                        
        //                        //Robots
        //                        GreenRobots =	0x1B9F4,
        //                        WhiteRobots	= 0x1BA0D,
        //                        Helicopter =	0x1BA3D,
        //                        }                          

        //private byte CheckForSprite(SpriteType type)
        //                   return result;
        //}

        private bool TryBlackoutMode([NotNull] EffectRequest request)  //Note Scaling max requested on ROM side
        {
            if (!Connector.Read8(ADDR_CURRENT_AREA, out byte cavelocation))
            {
                DelayEffect(request); 
                return false;
            }
            if (!Connector.Read8(0x07E0, out byte blackoutmode))
            {
                DelayEffect(request);
                return false;
            }
            if (blackoutmode == 0x9A)
            {
                DelayEffect(request);
                return false;
            }
            if (cavelocation == 0x00)
            {
                DelayEffect(request);
                return false;
            }
            if ((cavelocation >= 0x01) && (cavelocation <= 0x1e))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(10),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);               
                return true;
            }
            
            if ((cavelocation >= 0x20) && (cavelocation <= 0x3e))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E2, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E2, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }
            if ((cavelocation >= 0x40) && (cavelocation <= 0x6b))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }
            
            if ((cavelocation >= 0x6c) && (cavelocation <= 0x6e))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.Write8(0x07E1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }
            
            if ((cavelocation >= 0x7) && (cavelocation <= 0x78))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x7c) && (cavelocation <= 0x8e))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x8f) && (cavelocation <= 0x96))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x98) && (cavelocation <= 0xc5))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA)&& Connector.Write8(0x07E1, 0x9A), /*Effect Start Condition*/
                () => Connector.Write8(0x07E0, 0x9A) && Connector.Write8(0x07E2, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(0x07E0, 0x9A) && Connector.Write8(0x07E2, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            DelayEffect(request);
                return false;
        }

        private bool TryAlterScale([NotNull] EffectRequest request, sbyte scale)  //Note Scaling max requested on ROM side
        {
            if (!Connector.Read8(ADDR_SCALING, out byte cscale))
            {
                DelayEffect(request);
                return false;
            }

            if (cscale <= 0x01)
            {
                Respond(request, EffectStatus.FailPermanent, "Scaling already at minumum.");
                return false;
            }
            if ((cscale + scale) == 0x2F)
            {
                Respond(request, EffectStatus.FailPermanent, "Scaling already at maximum.");
                return false;
            }

            if (Connector.Write8(ADDR_SCALING, (byte)(cscale + scale)))
            {
                Respond(request, EffectStatus.Success);
                return true;
            }

            DelayEffect(request);
            return false;
        }

        private bool TryAlterLevel([NotNull] EffectRequest request, sbyte level)
        {
            if (!Connector.Read8(ADDR_LEVEL, out byte clevel))
            {
                DelayEffect(request);
                return false;
            }

            if ((clevel + level) == 0x00)
            {
                Respond(request, EffectStatus.FailPermanent, "Level already at minumum.");
                return false;
            }

            if ((clevel + level) == 0x11)
            {
                Respond(request, EffectStatus.FailPermanent, "Level already at maximum.");
                return false;
            }

            if (Connector.Write8(ADDR_LEVEL, (byte)(clevel + level)))
            {
                Respond(request, EffectStatus.Success);
                return true;
            }


            DelayEffect(request);
            return false;
        }

        private bool TryGiveMoney([NotNull] EffectRequest request, sbyte sentmoney)
        {

            if (!Connector.Read8(ADDR_Money1, out byte money1))
            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_Money2, out byte money2))

            {
                DelayEffect(request);
                return false;
            }

            if (money2 >= 0xFF)
            {
                Respond(request, EffectStatus.FailPermanent, "Max Money already aquired.");
                return false;
            }

            if (money1 + sentmoney < 0xFF)
            {
                Connector.Write8(ADDR_Money1, (byte)(money1 + sentmoney));
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }
            if (money1 + sentmoney >= 0xFF)
            {
                if (money2 == 0x00)
                {
                    Connector.Write8(ADDR_Money1, (byte)(money1 + sentmoney - 256));
                    Connector.Write8(ADDR_Money2, (byte)(money2 + 1));
                    {
                        Respond(request, EffectStatus.Success);
                        return true;
                    }
                }
                if (money2 >= 0x01)
                {
                    Connector.Write8(ADDR_Money1, (byte)(money1 + sentmoney - 256));
                    Connector.Write8(ADDR_Money2, (byte)(money2 + 1));
                    {
                        Respond(request, EffectStatus.Success);
                        return true;
                    }
                }
            }
            DelayEffect(request);
            return false;
        }

        private bool TryStealMoney([NotNull] EffectRequest request, sbyte stealmoney)
        {

            if (!Connector.Read8(ADDR_Money1, out byte money1))
            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_Money2, out byte money2))

            {
                DelayEffect(request);
                return false;
            }

            if ((money1 - stealmoney) + (money2 * 256) <= 0x00)
            {
                Respond(request, EffectStatus.FailPermanent, "Money at min.");
                return false;
            }

            if (money1 - stealmoney < 0xFF)
            {
                if (money2 == 0x00)
                {
                    Connector.Write8(ADDR_Money1, (byte)(money1 - stealmoney));
                    {
                        Respond(request, EffectStatus.Success);
                        return true;
                    }
                }
                if (money2 >= 0x01)
                {
                    if (money1 - stealmoney >= 0x00)
                    {
                        Connector.Write8(ADDR_Money1, (byte)(money1 - stealmoney));
                        {
                            Respond(request, EffectStatus.Success);
                            return true;
                        }
                    }

                    if (money1 - stealmoney < 0x00)
                    {
                        Connector.Write8(ADDR_Money1, (byte)(money1 - stealmoney + 256));
                        Connector.Write8(ADDR_Money2, (byte)(money2 - 1));
                        {
                            Respond(request, EffectStatus.Success);
                            return true;
                        }
                    }
                }
            }
            DelayEffect(request);
            return false;
        }

        private bool TryGiveMP([NotNull] EffectRequest request)
        {

            if (!Connector.Read8(ADDR_MPF, out byte MPFULL))

            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_MPC, out byte MPCURRENT))



                if (MPFULL == MPCURRENT)
                {
                    Respond(request, EffectStatus.FailPermanent, "Magic already at maximum.");
                    return false;
                }

            if (MPCURRENT < MPFULL)
            {
                Connector.Write8(ADDR_MPC, (byte)(MPFULL));
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }
            DelayEffect(request);
            return false;
        }

        private bool TryTakeMP([NotNull] EffectRequest request, sbyte mpfactor)
        {

            if (!Connector.Read8(ADDR_LEVEL, out byte playerlvl))

            {
                return false;
            }

            if (!Connector.Read8(ADDR_MPC, out byte cmp))

            {
                return false;
            }

            if ((cmp - (playerlvl * mpfactor) < 0))
            {
                if (Connector.Write8(ADDR_MPC, (byte)(0)))
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }

            if (Connector.Write8(ADDR_MPC, (byte)(cmp - (playerlvl * mpfactor))))
            {
                Respond(request, EffectStatus.Success);
                return true;
            }

            DelayEffect(request);
            return false;
        }

        private bool TryHealPlayerHealth([NotNull] EffectRequest request)
        {

            if (!Connector.Read8(ADDR_HEALTH_MAX, out byte maxhealth))

            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_HP, out byte chealth))

            {
                DelayEffect(request);
                return false;
            }

            if (maxhealth == chealth)
            {
                Respond(request, EffectStatus.FailPermanent, "Health already at maximum.");
                return false;
            }

            if (maxhealth >= chealth)
            {
                Connector.Write8(ADDR_HP, (byte)(maxhealth));
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }
            DelayEffect(request);
            return false;
        }

        private bool TryHurtPlayerHealth([NotNull] EffectRequest request, sbyte factor)
        {

            if (!Connector.Read8(ADDR_LEVEL, out byte playerlvl))

            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_HP, out byte mhealth))

            {
                DelayEffect(request);
                return false;
            }

            if ((mhealth - (playerlvl * factor) < 0))
            {
                if (Connector.Write8(ADDR_HP, (byte)(0)))
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }

            if (Connector.Write8(ADDR_HP, (byte)(mhealth - (playerlvl * factor))))
            {
                Respond(request, EffectStatus.Success);
                return true;
            }

            DelayEffect(request);
            return false;
        }

        private bool TryBossLocation([NotNull] EffectRequest request, byte boss)
        {
            if (!Connector.Read8(ADDR_CURRENT_AREA, out byte bosslocation))

                if (bosslocation == boss)
                {

                    return true;
                }

            if (bosslocation != boss)
            {
                Respond(request, EffectStatus.FailPermanent, "Wrong location.");
                return false;
            }


            return true;
        }

        private bool TryAlterBossHealth([NotNull] EffectRequest request, sbyte slope)
        {

            if (!Connector.Read8(ADDR_SCALING, out byte bossscale))

            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_BOSS_HEALTH1, out byte bosschealth))

            {
                DelayEffect(request);
                return false;
            }

            if ((bosschealth) <= 0x20)
            {
                Respond(request, EffectStatus.FailPermanent, "Near Death Fail.");
                return false;
            }

            if ((bosschealth + ((bossscale * slope) / 2)) > 255)
            {
                Respond(request, EffectStatus.FailPermanent, "Health already at maximum.");
                return false;
            }

            if (((bosschealth + ((bossscale * slope) / 2))) < 255)
            {
                if (Connector.Write8(ADDR_BOSS_HEALTH1, (byte)((bosschealth + (bossscale * slope) / 2))))
                {
                    Respond(request, EffectStatus.Success);
                    return true;
                }
            }

            DelayEffect(request);
            return false;
        }

        private bool TryAlterBossHealthSabera([NotNull] EffectRequest request, sbyte slope2)
        {
            if (!Connector.Read8(ADDR_SCALING, out byte bossscale2))

            {
                DelayEffect(request);
                return false;
            }

            if (!Connector.Read8(ADDR_BOSS_HEALTH2, out byte bosschealth2))

            {
                DelayEffect(request);
                return false;
            }

            if ((bosschealth2) <= 0x20)
            {
                Respond(request, EffectStatus.FailPermanent, "Near Death Fail.");
                return false;
            }

            if ((bosschealth2 + ((bossscale2 * slope2) / 2)) > 255)
            {
                Respond(request, EffectStatus.FailPermanent, "Health already at maximum.");
                return false;
            }

            if (Connector.Write8(ADDR_BOSS_HEALTH1, (byte)((bosschealth2 + (bossscale2 * slope2) / 2))))
            {
                Respond(request, EffectStatus.Success);
                return true;
            }

            DelayEffect(request);
            return false;
        }

        public override List<Effect> Effects

        {
            get
            {
                List<Effect> effects = new List<Effect>
                {
                 
                    // General
                    new Effect("Free Shopping", "freeshops"),
                    new Effect("Mado Screen Shake Mode", "screenshakemode"),
                    new Effect("Blackout Mode", "blackout"),
                    new Effect("Camouflage Mode", "invis"),
                    //new Effect("Wild Warp", "wild"),   Note pending UI update push to main website but testing worked
                    //new Effect("Reset", "reset"),   

                    //Scaling and Leveling
                    new Effect("Scaling and Leveling", "sandl", ItemKind.Folder),
                    new Effect("Scaling Increase", "scaleup", "sandl"),
                    new Effect("Scaling Decrease", "scaledown", "sandl"),
                    new Effect("Level Increase", "levelup", "sandl"),
                    new Effect("Level Decrease", "leveldown", "sandl"),
                    
                    //Player
                    new Effect("Player Effects", "player", ItemKind.Folder),
                    new Effect("Heal Player to Full", "healplayer", "player"),
                    new Effect("Hurt Player (Scaled %)", "hurtplayer", "player"),
                    new Effect("Cure Player", "recover", "player"),

                    //Magic
                    new Effect("Magic", "magic", ItemKind.Folder),
                    new Effect("Refill Magic to Full", "magicup", "magic"),
                    new Effect("Take Magic (Scaled %)", "magicdown", "magic"),
                    
                    //Money
                    new Effect("Money", "money", ItemKind.Folder),
                    new Effect("Give Money (50 Gold)", "givemoney50", "money"),
                    new Effect("Give Money (100 Gold)", "givemoney100", "money"),
                    new Effect("Steal Money (50 Gold)", "takemoney50", "money"),
                    new Effect("Steal Money (100 Gold)", "takemoney100", "money"),
                                        
                    //Heal Boss
                    //Note all boss fights check for boss area map to see if Effect can trigger but you can despawn the effect.  Will need to add screen fight transmition animation RAM action for better trigger.
                    new Effect("Heal Boss (X% Scaled)", "healboss", ItemKind.Folder),
                    new Effect("Vampire", "healvamp1", "healboss"),
                    new Effect("Big Bug", "healbug", "healboss"),
                    new Effect("Kelbesque", "healkelby", "healboss"),
                    new Effect("Vampire2", "healvamp2", "healboss"),
                    new Effect("Sabera", "healsabera", "healboss"),
                    new Effect("Mado", "healmado", "healboss"),
                    new Effect("Kelbesque2", "healkelby2", "healboss"),
                    new Effect("Sabera2", "healsabera2", "healboss"),
                    new Effect("Mado2", "healmado2", "healboss"),
                    new Effect("Karmine", "healkarmine", "healboss"),
                    new Effect("Draygon", "healdraygon", "healboss"),
                    new Effect("Draygon2", "healdraygon2", "healboss"),
                    new Effect("Dyna", "healdyna", "healboss"),

                    //Respawn Boss
                    //Note boss fights are linked to chest drop. Will need to update ASM to ensure Boss Chests are non key items in Crowd Control Flag Setup.
                    //new Effect("Respawn Story Mode Boss", "respawnboss", ItemKind.Folder).   
                    //new Effect("Kelbesque", "respawnkelby", "respawnboss"),
                    //new Effect("Sabera", "respawnsabera", "respawnboss"),
                    //new Effect("Mado", "respawnmado", "respawnboss"),
                    //new Effect("Kelbesque2", "respawnkelby2", "respawnboss"),
                    //new Effect("Sabera2", "respawnsabera2", "respawnboss"),
                    //new Effect("Mado2", "respawnmado2", "respawnboss"),
                    //new Effect("Karmine", "respawnkarmine", "respawnboss"),
                    //new Effect("Draygon", "respawndraygon", "respawnboss"),

                    // Projectile Effects
                    new Effect("Projectile Effects", "projectile", ItemKind.Folder),
                    new Effect("Warrior Ring Mode", "lvl1shot", "projectile"),
                    new Effect("LVL 2 Warrior Ring Mode", "lvl2shot", "projectile"),
                    new Effect("TriShot Mode", "trishot", "projectile"),
                    new Effect("Thunder Mode", "thundershot", "projectile"),
                    new Effect("Lag Storm Mode", "lagshot", "projectile"),

                    //Combat Effects
                    new Effect("Timed Combat Effects", "combat", ItemKind.Folder),
                    new Effect("One Hit KO", "ohko", "combat"),
                    new Effect("Equip Wind Sword (Timed)", "windsword", "combat"),
                    new Effect("Equip Fire Sword (Timed)", "firesword", "combat"),
                    new Effect("Equip Water Sword (Timed)", "watersword", "combat"),
                    new Effect("Equip Thunder Sword (Timed)", "thundersword", "combat"),
                    new Effect("Equip Crystalis Sword (Timed)", "crystalissword", "combat"),
                    new Effect("Steal Wind Sword (Timed)", "removewindsword", "takepower"),
                    new Effect("Steal Fire Sword (Timed)", "removefiresword", "takepower"),
                    new Effect("Steal Water Sword (Timed)", "removewatersword", "takepower"),
                    new Effect("Steal Thunder Sword (Timed)", "removethundersword", "takepower"),
                    
                    // Movement Effects
                    new Effect("Movement Effects", "moveeffect", ItemKind.Folder),
                    new Effect("Jump Mode", "jump", "moveeffect"),
                    new Effect("Flight Mode", "flightmode", "moveeffect"),
                    new Effect("Heavy Mode", "heavy", "moveeffect"),
                    
                    // Status Conditions
                    new Effect("Change Condition", "changecondition", ItemKind.Folder),                    
                    new Effect("UnTimed Posion", "poison", "changecondition"),
                    new Effect("UnTimed Paralysis", "paralysis", "changecondition"),
                    new Effect("UnTimed Slime", "slime", "changecondition"),
                    new Effect("UnTimed Stone", "stone", "changecondition"),
                    new Effect("Timed Posion", "timedpoison", "changecondition"),
                    new Effect("Timed Paralysis", "timedparalysis", "changecondition"),
                    new Effect("Timed Slime", "timedslime", "changecondition"),

                    // Power Down/Upgrade
                    // Note: Ro flag is required for Crowd Control Flags.
                    new Effect("Power Upgrade","givepower", ItemKind.Folder),
                    new Effect("Give Ball of Wind", "bowind", "givepower"),
                    new Effect("Give Ball of Fire", "bofire", "givepower"),
                    new Effect("Give Ball of Water", "bowater", "givepower"),
                    new Effect("Give Ball of Thunder", "bothunder", "givepower"),
                    new Effect("Give Bracelet of Wind", "brwind",  "givepower"),
                    new Effect("Give Bracelet of Fire", "brfire",  "givepower"),
                    new Effect("Give Bracelet of Water", "brwater",  "givepower"),
                    new Effect("Give Bracelet of Thunder", "brthunder",  "givepower"),

                    new Effect("Power Downgrade","takepower", ItemKind.Folder),
                    new Effect("Steal Ball of Wind", "stealbowind", "takepower"),
                    new Effect("Steal Ball of Fire", "stealbofire",  "takepower"),
                    new Effect("Steal Ball of Water", "stealbowater", "takepower"),
                    new Effect("Steal Ball of Thunder", "stealbothunder",   "takepower"),
                    new Effect("Steal Bracelet of Wind", "stealbrwind",  "takepower"),
                    new Effect("Steal Bracelet of Fire", "stealbrfire", "takepower"),
                    new Effect("Steal Bracelet of Water", "stealbrwater", "takepower"),
                    new Effect("Steal Bracelet of Thunder", "stealbrthunder", "takepower"),
                                        
                    //Armor and Shields
                    new Effect("Give Defense Item","givedef", ItemKind.Folder),
                    new Effect("Send Sacred Shield", "sshield","givedef"),
                    new Effect("Send Psycho Shield", "pshield","givedef"),
                    new Effect("Send Battle Armor ", "barmor","givedef"),
                    new Effect("Send Psycho Armor", "parmor", "givedef"),

                    //Spells
                    // Note not sure if all are useful, but teleport and recover should be done. still lacking mapping of areas for telport and controller force inputs.
                    //new Effect("Spells","spells", ItemKind.Folder),
                    //new Effect("Use Refresh", "refresh", "spells"),
                    //new Effect("Use Telepathy", "telepathy", "spells"),
                    //new Effect("Use Teleport", "teleport", "spells"),
                    //new Effect("Use Paralysis", "paralysis", "spells"),
                    //new Effect("Use Recover", "recover", "spells"),
                    //new Effect("Use Barrier", "barrier", "spells"),
                    //new Effect("Use Change", "change", "spells"),
                    //new Effect("Use Flight", "flight", "spells"),
                    
                    //Consumables
                    
                    new Effect("Give Consumable","giveconsumable", ItemKind.Folder),
                    new Effect("Give Medical Herb", "giveherb","giveconsumable"),
                    new Effect("Give Antidote", "giveanti","giveconsumable"),
                    new Effect("Give Magic Ring", "givemr","giveconsumable"),
                    new Effect("Give Lysis Plant", "givelp","giveconsumable"),
                    new Effect("Give Warp Boots", "givewb","giveconsumable"),
                    new Effect("Give Fruit of Power", "givefop","giveconsumable"),
                    new Effect("Give Fruit of Repun", "givefor","giveconsumable"),
                    new Effect("Give Fruit of Lime", "givefol","giveconsumable"),
                    new Effect("Give Opel Statue", "giveopel","giveconsumable"),
                    new Effect("Give Fruit of Lime Care Package", "allfol","giveconsumable"),

                    new Effect("Take Consumable","takeconsumable", ItemKind.Folder),
                    new Effect("Clear Inventory", "clear","takeconsumable"),
                    new Effect("Steal Herb", "stealherb","takeconsumable"),
                    new Effect("Steal Antidote", "stealanti","takeconsumable"),
                    new Effect("Steal Magic Ring", "stealmr","takeconsumable"),
                    new Effect("Steal Lysis Plant", "steallysis","takeconsumable"),
                    new Effect("Steal Warp Boots", "stealwp","takeconsumable"),
                    new Effect("Steal Fruit of Power", "stealfop","takeconsumable"),
                    new Effect("Steal Fruit of Repun", "stealfor","takeconsumable"),
                    new Effect("Steal Fruit of Lime", "stealfol","takeconsumable"),
                    new Effect("Steal Opel Statue", "stealopel","takeconsumable"),
                   
//  To Be Built Out (ASM)
                    //Spawn Enemy
                    // Note unclear how to read spawn tables to get proper spawn or any for now.
                    //new Effect("Spawn Enemy", "spawn"),
                    
                    // Blackout Mode (need to talk with Steve)
                    //new Effect("Blackout", "blackout"),
                                        
                    // Modify Triggers
                    //new Effect("Deactivate Dolphin", "dolphin"),

                    // Cave Opening (Windmill) and Dolphin Check
                    //new Effect("Seal Windmill", "windmill"),
                    //new Effect("Reset Dolphin", "dolphin"),
                            
                };

                return effects;
            }
        }

        public override List<(string, Action)> MenuActions => new List<(string, Action)>();

        public override Game Game { get; } = new Game(104, "Crystalis", "Crystalis", "NES", ConnectorType.NESConnector);

        protected override bool IsReady(EffectRequest request)
            => Connector.IsZero8(0x004A) && Connector.Read8(0x01F9, out byte b) && (b < 0xFF);  /*Main Menu = FF*/

        protected override void RequestData(DataRequest request) => Respond(request, request.Key, null, false, $"Variable name \"{request.Key}\" not known");

        protected override void StartEffect(EffectRequest request)
        {
            if (!IsReady(request))
            {         
                DelayEffect(request);  //Note add current location = FF to fail for restart screen
                return;
            }
            string[] effectT = request.FinalCode.Split('_');
            switch (effectT[0])
            {
                case "invis":
                    {
                        var invs = RepeatAction(request,
                        TimeSpan.FromSeconds(45),
                        () => Connector.IsZero8(0x07E4), /*Effect Start Condition*/
                        () => Connector.Freeze8(0x07E4, 0xFA), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(0x07E4, 0xFA), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        invs.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started invisible mode (45s)."));
                        return;                          
                    }

                case "ohko":
                    {
                        //Requires UI refresh at start and end

                        byte origHP = 01;
                        var w = RepeatAction(request, TimeSpan.FromSeconds(30),
                            () => Connector.Read8(ADDR_HP, out origHP) && (origHP > 1),
                            () => Connector.SendMessage($"{request.DisplayViewer} sent One Hit KO Mode."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_HP), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_HP, 0x02), TimeSpan.FromSeconds(1), true, "health");
                        w.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_HP, origHP);
                            Connector.SendMessage("One Hit KO Removed.");
                        });
                        return;
                    }

                //case "wild":  //Pending UI to be pushed
                //    {
                //        TryEffect(request,
                //            () => Connector.Write8(ADDR_U2HOOK, 0x02),
                //            () => Connector.Write8(ADDR_U1HOOK, 0x01),
                //            () =>
                //            {
                //                Connector.SendMessage($"{request.DisplayViewer} wild warped you.");
                //            }
                //                    );
                //        return;
                //    }

                case "kill":
                    {
                        TryEffect(request,
                        () => Connector.IsNonZero8(ADDR_HP),
                        () => Connector.Write8(ADDR_HP, 0),
                        () => { Connector.SendMessage($"{request.DisplayViewer} killed you."); });
                        return;
                    }

                case "jump":
                    {
                        RepeatAction(request, TimeSpan.FromSeconds(15),
                      () => Connector.IsZero8(ADDR_Jump),
                      () => Connector.Write8(ADDR_Jump, 32) && Connector.SendMessage($"{request.DisplayViewer} has granted you jump mode."), TimeSpan.FromSeconds(0.5),
                      () => true, TimeSpan.FromSeconds(5),
                      () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Jump, 32), TimeSpan.FromSeconds(0.5), true)
                      .WhenCompleted.ContinueWith(t => Connector.SendMessage($"{request.DisplayViewer} has removed your jump mode."));
                        return;
                    }

                case "flightmode":  //Note Screen scroll weird in x only direction but up and down and diagronally work great.
                    {
                        var flight = RepeatAction(request,
                        TimeSpan.FromSeconds(45),
                        () => Connector.IsZero8(ADDR_Jump), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Jump, 0x20), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Jump, 0x20) && Connector.IsNonZero8(ADDR_Jump), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        flight.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started flight mode."));
                        flight.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s removed flight mode."));
                        return;
                    }

                case "screenshakemode":  //Note Screen scroll weird in x only direction but up and down and diagronally work great.
                    {
                        var shake = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_ScreenHit1), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_ScreenHit1, 0x02), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_ScreenHit2, 0x02), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        shake.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started shaking mode."));
                        shake.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s removed shaking mode."));
                        return;
                    }

                case "heavy":
                    {                        
                        var heavy = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Speed, out byte b) && (b == 0x06), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_Speed, 0x03), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Speed, 0x03), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        heavy.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} lowered your speed (45s)."));
                        return;
                    }

                case "lvl1shot":

                    if (!Connector.Read8(ADDR_Warrior, out byte charge))
                    {
                        DelayEffect(request);
                    }
                    else if ((charge) >= 0x01)
                    {
                        DelayEffect(request);
                    }
                    else
                    {                       
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Warrior), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x08), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x08), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        war.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed Warrior Ring Effect (15s)."));
                        return;

                    }
                    return;

                case "lvl2shot":


                    if (!Connector.Read8(ADDR_Warrior, out byte charge1))
                    {
                        DelayEffect(request);
                    }
                    else if ((charge1) >= 0x01)
                    {
                        DelayEffect(request);
                    }
                    else
                    {
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Warrior), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x10), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x10), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        war.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed LVL2 Warrior Ring Effect (15s)."));
                        return; 
                        
                    }
                    return;

                case "trishot":
                    if (!Connector.Read8(ADDR_Warrior, out byte charge2))
                    {
                        DelayEffect(request);
                    }
                    else if ((charge2) >= 0x01)
                    {
                        DelayEffect(request);
                    }
                    else
                    {
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Warrior), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x68), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x68), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        war.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed TriShot Effect (15s)."));
                        return;

                    }
                    return;

                case "thundershot":
                    if (!Connector.Read8(ADDR_Warrior, out byte charge3))
                    {
                        DelayEffect(request);
                    }
                    else if ((charge3) >= 0x01)
                    {
                        DelayEffect(request);
                    }
                    else
                    {
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Warrior), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x70), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x70), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        war.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed Thunder Shot Effect (15s)."));
                        return;                         
                    }
                    return;


                case "lagshot":
                    if (!Connector.Read8(ADDR_Warrior, out byte charge4))
                    {
                        DelayEffect(request);
                    }
                    else if ((charge4) >= 0x01)
                    {
                        DelayEffect(request);
                    }
                    else
                    {
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Warrior), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x78), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x78), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        war.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed Thunder Shot Effect (15s)."));
                        return;                                                
                    }
                    return;


                case "recover":
                    {
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) == 0x00)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Normal Condition Already");
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 00, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 0);
                            Connector.SendMessage($"{request.DisplayViewer} cured you");
                        }
                        return;
                    }

                case "paralysis":
                    {
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) >= 0x01)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Condition affected already");
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 01, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 01);
                            Connector.SendMessage($"{request.DisplayViewer} paralysis you");
                        }
                        return;
                    }

                case "timedparalysis":
                    {
                        var cond = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Condition, out byte b) && (b == 0x00), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_Condition, 0x01), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x01), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        cond.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} paralysised you (15s)."));
                        return;
                    }

                case "stone":
                    {
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) >= 0x01)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Condition affected already");
                        }
                        else if (!Connector.SetBits(ADDR_Stone, 255, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Stone, 255);
                            Connector.SendMessage($"{request.DisplayViewer} stoned you for 4 seconds");
                        }
                        return;
                    }

                case "poison":
                    {
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) >= 0x01)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Condition affected already");
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 03, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 03);
                            Connector.SendMessage($"{request.DisplayViewer} poisoned you");
                        }
                        return;
                    }

                case "timedpoison":
                    {
                        var cond = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Condition, out byte b) && (b == 0x00), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_Condition, 0x03), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x03), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        cond.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} poisoned you (15s)."));
                        return;                       
                    }

                case "slime":   //Note will need to fail at boss area since it would lock you there or death.
                    {
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) >= 0x01)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Condition affected already");
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 04, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 04);
                            Connector.SendMessage($"{request.DisplayViewer} slimed you");
                        }
                        return;
                    }

                case "timedslime":
                    {
                        var slime = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Condition), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Condition, 0x04), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x04) && Connector.Freeze8(ADDR_Condition, 0x04) && Connector.IsNonZero8(ADDR_Condition), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        slime.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed slimed Effect."));
                        slime.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s slimed Effect has dispered."));
                        return;
                    }

                case "bowind":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot1, out byte bwind))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwind) == 0x05)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Wind already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot1, 05, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot1, 05);
                            Connector.SendMessage($"{request.DisplayViewer} sent Ball of Wind");
                        }
                        return;
                    }

                case "bofire":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot2, out byte bfire))
                        {
                            DelayEffect(request);
                        }
                        else if ((bfire) == 0x07)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Fire already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot2, 07, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot2, 07);
                            Connector.SendMessage($"{request.DisplayViewer} sent Ball of Fire");
                        }
                        return;
                    }

                case "bowater":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot3, out byte bwater))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwater) == 0x09)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Water already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot3, 09, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot3, 09);
                            Connector.SendMessage($"{request.DisplayViewer} sent Ball of Water");
                        }
                        return;
                    }

                case "bothunder":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot4, out byte bthunder))
                        {
                            DelayEffect(request);
                        }
                        else if ((bthunder) == 0x0B)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Thunder already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot4, 0x0B, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot4, 0x0B);
                            Connector.SendMessage($"{request.DisplayViewer} sent Ball of Thunder");
                        }
                        return;
                    }

                case "brwind":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot1, out byte brwind))
                        {
                            DelayEffect(request);
                        }
                        else if ((brwind) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Send Ball of Wind First");
                        }
                        else if ((brwind) == 0x06)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Tornado Bracelet already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot1, 0x06, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot1, 0x06);
                            Connector.SendMessage($"{request.DisplayViewer} sent Tornado Bracelet.");
                        }
                        return;
                    }

                case "brfire":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot2, out byte brfire))
                        {
                            DelayEffect(request);
                        }
                        else if ((brfire) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Send Ball of Fire First");
                        }
                        else if ((brfire) == 0x08)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Flame Bracelet  already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot2, 0x08, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot2, 0x08);
                            Connector.SendMessage($"{request.DisplayViewer} sent Flame Bracelet.");
                        }
                        return;
                    }

                case "brwater":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot3, out byte brwater))
                        {
                            DelayEffect(request);
                        }
                        else if ((brwater) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Send Ball of Water First");
                        }
                        else if ((brwater) == 0x0A)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Blizzard Bracelet already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot3, 0x0A, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot3, 0x0A);
                            Connector.SendMessage($"{request.DisplayViewer} sent Blizzard Bracelet");
                        }
                        return;
                    }

                case "brthunder":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot4, out byte brthunder))
                        {
                            DelayEffect(request);
                        }
                        else if ((brthunder) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Send Ball of Thunder First");
                        }
                        else if ((brthunder) == 0x0C)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Storm Bracelet already acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot4, 0x0C, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot4, 0x0C);
                            Connector.SendMessage($"{request.DisplayViewer} sent Storm Bracelet");
                        }
                        return;
                    }

                case "stealbowind":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot1, out byte bwind))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwind) == 0x06)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Steal Tornado Bracelet First");
                        }
                        else if ((bwind) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Wind not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole Ball of Wind");
                        }
                        return;
                    }

                case "stealbofire":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot2, out byte bfire))
                        {
                            DelayEffect(request);
                        }
                        else if ((bfire) == 0x08)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Steal Flame Bracelet First");
                        }
                        else if ((bfire) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Fire not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot2, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot2, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole Ball of Fire");
                        }
                        return;
                    }

                case "stealbowater":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot3, out byte bwater))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwater) == 0x0A)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Steal Blizzard Bracelet First");
                        }
                        else if ((bwater) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Water not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot3, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot3, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole Ball of Water");
                        }
                        return;
                    }

                case "stealbothunder":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot4, out byte bthunder))
                        {
                            DelayEffect(request);
                        }
                        else if ((bthunder) == 0x0C)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Steal Storm Bracelet First");
                        }
                        else if ((bthunder) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Ball of Thunder not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot4, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot4, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole Ball of Thunder");
                        }
                        return;
                    }

                case "stealbrwind":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot1, out byte bwind))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwind) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "No Wind Upgrade not acqurired");
                        }
                        else if ((bwind) == 0x05)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Tornado Bracelet not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot1, 0x05, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot1, 0x05);
                            Connector.SendMessage($"{request.DisplayViewer} stole Tornado Bracelet");
                        }
                        return;
                    }

                case "stealbrfire":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot2, out byte bfire))
                        {
                            DelayEffect(request);
                        }
                        else if ((bfire) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "No Fire Upgrade not acqurired");
                        }
                        else if ((bfire) == 0x07)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Flame Bracelet not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot2, 0x07, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot2, 0x07);
                            Connector.SendMessage($"{request.DisplayViewer} stole Flame Bracelet.");
                        }
                        return;
                    }

                case "stealbrwater":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot3, out byte bwater))
                        {
                            DelayEffect(request);
                        }
                        else if ((bwater) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "No Water Upgrade not acqurired");
                        }
                        else if ((bwater) == 0x09)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Blizzard Bracelet not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot3, 0x09, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot3, 0x09);
                            Connector.SendMessage($"{request.DisplayViewer} stole Blizzard Bracelet.");
                        }
                        return;
                    }

                case "stealbrthunder":
                    {
                        if (!Connector.Read8(ADDR_PowerSlot4, out byte bthunder))
                        {
                            DelayEffect(request);
                        }
                        else if ((bthunder) == 0xFF)
                        {
                            Respond(request, EffectStatus.FailPermanent, "No Thunder Upgrade not acqurired");
                        }
                        else if ((bthunder) == 0x0B)
                        {
                            Respond(request, EffectStatus.FailPermanent, "Storm Bracelet not acqurired");
                        }
                        else if (!Connector.SetBits(ADDR_PowerSlot4, 0x0B, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_PowerSlot4, 0x0B);
                            Connector.SendMessage($"{request.DisplayViewer} stole Storm Bracelet.");
                        }
                        return;
                    }

                case "barmor":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ArmorSlot1, out byte barmor1))
                        {
                            DelayEffect(request);
                        }
                        else if ((barmor1) < 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ArmorSlot2, out byte barmor2))
                            {
                                DelayEffect(request);
                            }
                            //Battle Armor Sent Already Check
                            else if ((barmor1) == 0x1B)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Battle Armor aquired already slot 1");
                            }
                            else if ((barmor2) == 0x1B)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Battle Armor aquired already slot 2");
                            }

                            else if ((barmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte barmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Battle Armor Sent Already Check
                                else if ((barmor3) == 0x1B)
                                {
                                    Respond(request, EffectStatus.FailPermanent, "Battle Armor aquired already slot 3");
                                }
                                else if ((barmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte barmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Battle Armor Sent Already Check
                                    else if ((barmor4) == 0x1B)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Battle Armor aquired already slot 4");
                                    }
                                    else if ((barmor4) < 0xFF)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Slot 4 not open");
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x1B, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x1B);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x1B, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x1B);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x1B, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x1B);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x1B, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x1B);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor");
                        }
                        return;
                    }

                case "parmor":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ArmorSlot1, out byte parmor1))
                        {
                            DelayEffect(request);
                        }
                        else if ((parmor1) < 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ArmorSlot2, out byte parmor2))
                            {
                                DelayEffect(request);
                            }
                            //Psycho Armor Sent Already Check
                            else if ((parmor1) == 0x1C)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Psycho Armor aquired already slot 1");
                            }
                            else if ((parmor2) == 0x1C)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Psycho Armor aquired already slot 2");
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x1C)
                                {
                                    Respond(request, EffectStatus.FailPermanent, "Psycho Armor aquired already slot 3");
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x1C)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Psycho Armor aquired already slot 4");
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Slot 4 not open");
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x1C, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x1C);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x1C, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x1C);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x1C, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x1C);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x1C, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x1C);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor");
                        }
                        return;
                    }

                case "pshield":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ShieldSlot1, out byte pshield1))
                        {
                            DelayEffect(request);
                        }
                        else if ((pshield1) < 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ShieldSlot2, out byte pshield2))
                            {
                                DelayEffect(request);
                            }
                            //Psycho Shield Sent Already Check
                            else if ((pshield1) == 0x14)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Psycho Shield aquired already slot 1");
                            }
                            else if ((pshield2) == 0x14)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Psycho Shield aquired already slot 2");
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x14)
                                {
                                    Respond(request, EffectStatus.FailPermanent, "Psycho Shield aquired already slot 3");
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x14)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Psycho Shield aquired already slot 4");
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Slot 4 not open");
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x14, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x14);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x14, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x14);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x14, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot2, 0x14);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x14, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x14);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield");
                        }
                        return;
                    }

                case "sshield":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ShieldSlot1, out byte sshield1))
                        {
                            DelayEffect(request);
                        }
                        else if ((sshield1) < 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ShieldSlot2, out byte sshield2))
                            {
                                DelayEffect(request);
                            }
                            //Sacred Shield Sent Already Check
                            else if ((sshield1) == 0x12)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Sacred Shield aquired already slot 1");
                            }
                            else if ((sshield2) == 0x12)
                            {
                                Respond(request, EffectStatus.FailPermanent, "Sacred Shield aquired already slot 2");
                            }

                            else if ((sshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte sshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Sacred Shield Sent Already Check
                                else if ((sshield3) == 0x12)
                                {
                                    Respond(request, EffectStatus.FailPermanent, "Sacred Shield aquired already slot 3");
                                }
                                else if ((sshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte sshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Sacred Shield Sent Already Check
                                    else if ((sshield4) == 0x12)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Sacred Shield aquired already slot 4");
                                    }
                                    else if ((sshield4) < 0xFF)
                                    {
                                        Respond(request, EffectStatus.FailPermanent, "Slot 4 not open");
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x12, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x12);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x12, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x12);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x12, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot2, 0x12);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x12, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x12);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield");
                        }
                        return;
                    }

                case "clear":
                    {
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) < 0xFF)

                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) < 0xFF)

                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) < 0xFF)

                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) < 0xFF)

                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) < 0xFF)

                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) < 0xFF)

                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) < 0xFF)



                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) < 0xFF)

                                                        if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                            DelayEffect(request);

                                                        else
                                                        {
                                                            Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 8");
                                                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 7");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 6");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 5");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 4");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 3");
                        }



                        if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 2");
                        }
                        if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 1");
                        }
                        return;

                    }

                case "allfol":
                    {
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) <= 0xFF)

                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) <= 0xFF)

                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) <= 0xFF)

                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) <= 0xFF)

                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) <= 0xFF)

                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) <= 0xFF)

                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) <= 0xFF)



                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) <= 0xFF)

                                                        if (!Connector.SetBits(ADDR_ConsumeSlot8, 0x20, out _))
                                                            DelayEffect(request);

                                                        else
                                                        {
                                                            Connector.Write8(ADDR_ConsumeSlot8, 0x20);
                                                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                                                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot7, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot7, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot6, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot6, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot5, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot5, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot4, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot4, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot3, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot3, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }



                        if (!Connector.SetBits(ADDR_ConsumeSlot2, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot2, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }
                        if (!Connector.SetBits(ADDR_ConsumeSlot1, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0x20);
                            Connector.SendMessage($"{request.DisplayViewer} made you Stomp");
                        }
                        return;

                    }

                case "giveherb":
                    {
                        byte item = 0x1D;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb");
                        }
                        return;
                    }

                case "stealherb":
                    {
                        var herb = 0x1D;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != herb)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != herb)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != herb)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != herb)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != herb)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != herb)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != herb)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != herb)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Medical Herb in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Medical Herb");
                        }
                        return;
                    }

                case "giveanti":

                    {
                        byte item = 0x1E;
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Antidote.");
                        }
                        return;
                    }

                case "stealanti":
                    {
                        var anti = 0x1E;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != anti)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != anti)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != anti)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != anti)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != anti)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != anti)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != anti)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != anti)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Antidote in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Antidote");
                        }
                        return;
                    }

                case "givelp":
                    {
                        byte item = 0x1F;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant");
                        }
                        return;
                    }

                case "steallysis":
                    {
                        var lp = 0x1F;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != lp)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != lp)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != lp)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != lp)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != lp)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != lp)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != lp)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != lp)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Lysis Plant in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant");
                        }
                        return;
                    }

                case "givefol":
                    {
                        byte item = 0x20;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime");
                        }
                        return;
                    }

                case "stealfol":
                    {
                        var fol = 0x20;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != fol)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != fol)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != fol)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != fol)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != fol)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != fol)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != fol)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != fol)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Fruit of Lime in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime");
                        }
                        return;
                    }

                case "givefop":
                    {
                        byte item = 0x21;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power");
                        }
                        return;
                    }

                case "stealfop":
                    {
                        var fop = 0x21;
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != fop)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != fop)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != fop)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != fop)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != fop)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != fop)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != fop)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != fop)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Fruit of Power in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power");
                        }
                        return;
                    }

                case "givemr":
                    {
                        byte item = 0x22;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring");
                        }
                        return;
                    }

                case "stealmr":
                    {
                        var mr = 0x22;
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != mr)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != mr)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != mr)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != mr)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != mr)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != mr)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != mr)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != mr)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Magic Ring in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Magic Ring");
                        }
                        return;
                    }

                case "givefor":
                    {
                        byte item = 0x23;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun");
                        }
                        return;
                    }

                case "stealfor":
                    {
                        var fruitr = 0x23;
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != fruitr)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != fruitr)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != fruitr)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != fruitr)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != fruitr)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != fruitr)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != fruitr)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != fruitr)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Fruit of Repun in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun");
                        }
                        return;
                    }

                case "givewb":
                    {
                        byte item = 0x24;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots");
                        }
                        return;
                    }

                case "stealwp":
                    {
                        var wp = 0x24;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != wp)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != wp)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != wp)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != wp)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != wp)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != wp)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != wp)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != wp)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Warp Boots in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Warp Boots");
                        }
                        return;
                    }

                case "giveopel":
                    {
                        byte item = 0x26;

                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != 0xFF)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != 0xFF)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != 0xFF)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != 0xFF)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != 0xFF)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != 0xFF)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != 0xFF)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != 0xFF)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Slot open in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Connector.SendMessage($"{request.DisplayViewer} sent Opel Statue.");
                        }
                        return;
                    }

                case "stealopel":
                    {
                        var opel = 0x26;
                        if (!Connector.Read8(ADDR_ConsumeSlot1, out byte con1))
                        {
                            DelayEffect(request);
                        }
                        else if ((con1) != opel)
                        {
                            if (!Connector.Read8(ADDR_ConsumeSlot2, out byte con2))
                            {
                                DelayEffect(request);
                            }
                            else if ((con2) != opel)
                            {
                                if (!Connector.Read8(ADDR_ConsumeSlot3, out byte con3))
                                {
                                    DelayEffect(request);
                                }
                                else if ((con3) != opel)
                                {
                                    if (!Connector.Read8(ADDR_ConsumeSlot4, out byte con4))
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((con4) != opel)
                                    {
                                        if (!Connector.Read8(ADDR_ConsumeSlot5, out byte con5))
                                        {
                                            DelayEffect(request);
                                        }
                                        else if ((con5) != opel)
                                        {
                                            if (!Connector.Read8(ADDR_ConsumeSlot6, out byte con6))
                                            {
                                                DelayEffect(request);
                                            }
                                            else if ((con6) != opel)
                                            {
                                                if (!Connector.Read8(ADDR_ConsumeSlot7, out byte con7))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else if ((con7) != opel)
                                                {
                                                    if (!Connector.Read8(ADDR_ConsumeSlot8, out byte con8))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else if ((con8) != opel)
                                                    {
                                                        Respond(request, EffectStatus.FailPermanent, "No Opel Statue in inventory");
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} took Opel Statue");
                        }
                        return;
                    }

                case "windsword":
                    {
                        byte windsword = 01;
                        var f = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out windsword) && (windsword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} equipped Wind Sword."), TimeSpan.FromSeconds(1),
                            () => Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Equip_Sword, 0x01), TimeSpan.FromSeconds(1), true, "windsword");
                        f.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x01);
                            Connector.SendMessage("Locked Sword Removed.");
                        });
                        return;
                    }

                case "firesword":
                    {
                        byte firesword = 02;
                        var g = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out firesword) && (firesword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} equipped Fire Sword."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x02), TimeSpan.FromSeconds(1), true, "firesword");
                        g.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x02);
                            Connector.SendMessage("Locked Sword Removed.");
                        });
                        return;
                    }

                case "watersword":
                    {
                        byte watersword = 03;
                        var h = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out watersword) && (watersword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} equipped Water Sword."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x03), TimeSpan.FromSeconds(1), true, "watersword");
                        h.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, watersword);
                            Connector.SendMessage("Locked Sword Removed.");
                        });
                        return;
                    }

                case "thundersword":
                    {
                        byte thundersword = 04;
                        var j = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out thundersword) && (thundersword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} equipped Thunder Sword."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x04), TimeSpan.FromSeconds(1), true, "thundersword");
                        j.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, thundersword);
                            Connector.SendMessage("Thunder Sword removed.");
                        });
                        return;
                    }

                case "crystalissword":
                    { // Note it only stabs for now.  Need to review if I can autofill projectile to match Dyna Fight.

                        byte cysword = 05;
                        var k = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out cysword) && (cysword <= 5),
                            () => Connector.SendMessage($"{request.DisplayViewer} equipped Crystalis Sword."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x05), TimeSpan.FromSeconds(1), true, "cysword");
                        k.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x00);
                            Connector.SendMessage("Crystalis Removed. Reequip Sword");
                        });
                        return;
                    }

                case "removewindsword":
                    {
                        var rwsword = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_SwordSlot1, out byte b) && (b == 0x00), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_SwordSlot1, 0xFF), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot1, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        rwsword.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} stole your Sword of Wind (15s)."));
                        return;                                               
                    }

                case "removefiresword":
                    {
                        var rfsword = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_SwordSlot2, out byte b) && (b == 0x01), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_SwordSlot2, 0xFF), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot2, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        rfsword.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} stole your Sword of Fire (15s)."));
                        return;                                                
                    }

                case "removewatersword":
                    {
                        var rwwsword = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_SwordSlot3, out byte b) && (b == 0x02), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_SwordSlot3, 0xFF), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot3, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        rwwsword.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} stole your Sword of Water (15s)."));
                        return;                                             
                    }

                case "removethundersword":
                    {
                        var rtsword = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_SwordSlot4, out byte b) && (b == 0x03), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_SwordSlot4, 0xFF), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot4, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        rtsword.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} stole your Sword of Thunder (15s)."));
                        return;                           
                    }

                case "healvamp1":
                    {
                        byte Vamp1 = 0x0A;

                        if (TryBossLocation(request, Vamp1))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x04))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Vamp1 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healbug":
                    {
                        byte Bug = 0x1A;

                        if (TryBossLocation(request, Bug))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x04))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Bug Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healkelby":
                    {
                        byte Kelby = 0x28;

                        if (TryBossLocation(request, Kelby))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x06))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Kelby Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healvamp2":
                    {
                        byte Vamp2 = 0x6C;

                        if (TryBossLocation(request, Vamp2))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x04))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Vamp2 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healsabera":
                    {
                        byte Sabera = 0x6E;

                        if (TryBossLocation(request, Sabera))
                        {
                            {
                                if (TryAlterBossHealthSabera(request, 0x06))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Sabera Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healmado":
                    {
                        byte Mado = 0xF2;

                        if (TryBossLocation(request, Mado))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x04))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Mado Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healkelby2":
                    {
                        byte Kelby2 = 0xA9;

                        if (TryBossLocation(request, Kelby2))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x06))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Kelby2 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healsabera2":
                    {
                        byte Sabera2 = 0xAC;

                        if (TryBossLocation(request, Sabera2))
                        {
                            {
                                if (TryAlterBossHealth(request, 10))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Sabera2 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healmado2":
                    {
                        byte Mado2 = 0xB9;

                        if (TryBossLocation(request, Mado2))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x06))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Mado2 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healkarmine":
                    {
                        byte Karmine = 0xB6;

                        if (TryBossLocation(request, Karmine))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x04))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Karmine Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healdraygon":
                    {
                        byte Draygon = 0x9F;

                        if (TryBossLocation(request, Draygon))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x08))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Draygon Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healdraygon2":
                    {
                        byte Draygon2 = 0xA6;

                        if (TryBossLocation(request, Draygon2))
                        {
                            {
                                if (TryAlterBossHealth(request, 0x08))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Draygon2 Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "healdyna":
                    {
                        byte Dyna = 0x5F;

                        if (TryBossLocation(request, Dyna))
                        {
                            {
                                if (TryAlterBossHealth(request, 16))
                                {
                                    Connector.SendMessage($"{request.DisplayViewer} sent Dyna Health.");
                                }
                                return;
                            }
                        }
                        return;
                    }

                case "magicup":     //UI update pending  
                    {
                        if (TryGiveMP(request))
                        {
                            Connector.SendMessage($"{request.DisplayViewer} restored your magic to full.");
                        }
                        return;
                    }

                case "magicdown":     //UI update pending  
                    {
                        if (TryTakeMP(request, 6))
                        {
                            Connector.SendMessage($"{request.DisplayViewer} took some magic.");
                        }
                        return;
                    }

                case "healplayer":     //UI update pending  
                    {
                        if (TryHealPlayerHealth(request))
                        {
                            Connector.SendMessage($"{request.DisplayViewer} heal you to full.");
                        }
                        return;
                    }

                case "hurtplayer":      //UI update pending  
                    {
                        if (TryHurtPlayerHealth(request, 4))
                        {
                            Connector.SendMessage($"{request.DisplayViewer} hurt you.");
                        }
                        return;
                    }

                case "givemoney50":     //UI update pending                   
                    {
                        if (TryGiveMoney(request, 50))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x02);
                            //Connector.Write8(ADDR_U2HOOK, 1); 
                            Connector.SendMessage($"{request.DisplayViewer} sent 50 dollars.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "givemoney100":     //UI update pending              
                    {
                        if (TryGiveMoney(request, 100))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x02);
                            //Connector.Write8(ADDR_U2HOOK, 1); 
                            Connector.SendMessage($"{request.DisplayViewer} sent 100 dollars.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "takemoney50":     //UI update pending                   
                    {
                        if (TryStealMoney(request, 50))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x02);
                            //Connector.Write8(ADDR_U2HOOK, 1); 
                            Connector.SendMessage($"{request.DisplayViewer} stole 50 dollars.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "takemoney100":     //UI update pending but currently only work in the up direction                  
                    {
                        if (TryStealMoney(request, 100))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x02);
                            //Connector.Write8(ADDR_U2HOOK, 1); 
                            Connector.SendMessage($"{request.DisplayViewer} stole 100 dollars.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "levelup":     //UI update pending but currently only work in the up direction             
                    {
                        if (TryAlterLevel(request, 1))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x9D);
                            //Connector.Write8(ADDR_U2HOOK, 1);
                            Connector.SendMessage($"{request.DisplayViewer} sent a level.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "leveldown":   //UI update pending but currently only work in the up direction   
                    {
                        if (TryAlterLevel(request, -1))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x9D);
                            //Connector.Write8(ADDR_U2HOOK, 1);
                            Connector.SendMessage($"{request.DisplayViewer} removed a level.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "scaleup":     //Note I need to find a way to read what the "actual scaling" is at X time because it can go over max right now if you find a key item at max scale 47.
                    {
                        if (TryAlterScale(request, 1))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x9D);
                            //Connector.Write8(ADDR_U2HOOK, 1);
                            Connector.SendMessage($"{request.DisplayViewer} increased the difficulty.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "scaledown":    //UI update pending
                    {
                        if (TryAlterScale(request, -1))
                        {
                            //Connector.Write8(ADDR_U3HOOK, 0x9D);
                            //Connector.Write8(ADDR_U2HOOK, 1);
                            Connector.SendMessage($"{request.DisplayViewer} decreased the difficulty.");
                            //Connector.Write8(ADDR_U1HOOK, 1);
                        }
                        return;
                    }

                case "blackout":   
                    {
                        if (TryBlackoutMode(request))
                        {                            
                            Connector.SendMessage($"{request.DisplayViewer} made cave and forts dark.");
                        }
                        return;
                    }

                case "freeshops":   //Note need to fix the inns as they are currently zero outing money but everything else works.
                    {
                        var shop = RepeatAction(request,
                        TimeSpan.FromSeconds(30),
                        () => Connector.IsNonZero8(ADDR_LEVEL),                     /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_SHOP_ITEM1_PRICE1, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM1_PRICE2, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM2_PRICE1, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM2_PRICE2, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM3_PRICE1, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM3_PRICE2, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM4_PRICE1, 0)
                             && Connector.Freeze8(ADDR_SHOP_ITEM4_PRICE2, 0),       /*Start Action*/
                        TimeSpan.FromSeconds(1),                                    /*Retry Timer*/
                        () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsZero8(ADDR_SHOP_ITEM1_PRICE1),            /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500),                             /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        shop.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} made everything free!!!"));
                        shop.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s restored back to normal prices."));
                        return;
                    }

            }
        }

        protected override bool StopEffect(EffectRequest request)
        {
            bool result = true;
            var effect = request.FinalCode.Split('_');
            switch (effect[0])
            {
                case "invis":
                    {
                        Connector.Unfreeze(0X07E4);
                        Connector.Write8(0X07E4, 0x00);
                        return result;
                    }
                
                case "lvl1shot":
                    {
                        result = Connector.Unfreeze(ADDR_Warrior);
                        return result;
                    }

                case "lvl2shot":
                    {
                        result = Connector.Unfreeze(ADDR_Warrior);
                        return result;
                    }

                case "trishot":
                    {
                        result = Connector.Unfreeze(ADDR_Warrior);
                        return result;
                    }

                case "thundershot":
                    {
                        result = Connector.Unfreeze(ADDR_Warrior);
                        return result;
                    }

                case "lagshot":
                    {
                        result = Connector.Unfreeze(ADDR_Warrior);
                        return result;
                    }

                case "freeshops":
                    {
                        result = Connector.Unfreeze(ADDR_SHOP_ITEM1_PRICE1)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM1_PRICE2)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM2_PRICE1)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM2_PRICE2)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM3_PRICE1)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM3_PRICE2)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM4_PRICE1)
                        && Connector.Unfreeze(ADDR_SHOP_ITEM4_PRICE2);
                        return result;
                    }

                case "flightmode":
                    {
                        result = Connector.Unfreeze(ADDR_Jump);
                        return result;
                    }

                case "screenshakemode":
                    {
                        result = Connector.Unfreeze(ADDR_ScreenHit1);
                        return result;
                    }

                case "heavy":
                    {
                        Connector.Write8(ADDR_Speed, 0x06);
                        Connector.SendMessage($"{request.DisplayViewer}'s restored your speed.");
                        return result;
                    }

                case "timedparalysis":
                    {
                        Connector.Write8(ADDR_Condition, 0x00);
                        Connector.SendMessage($"{request.DisplayViewer}'s removed your paralysis.");
                        return result;
                    }

                case "timedpoison":
                    {
                        Connector.Write8(ADDR_Condition, 0x00);
                        Connector.SendMessage($"{request.DisplayViewer}'s removed your poison.");
                        return result;
                    }

                case "timedslime":
                    {
                        Connector.Unfreeze(ADDR_Condition);
                        Connector.Write8(ADDR_Condition, 0x00);
                        return result;
                    }

                case "removewindsword":
                    {
                        Connector.Write8(ADDR_SwordSlot1, 0x00);
                        Connector.SendMessage($"{request.DisplayViewer}'s gave back your Sword of Wind.");
                        return result;
                    }

                case "removefiresword":
                    {
                        Connector.Write8(ADDR_SwordSlot2, 0x01);
                        Connector.SendMessage($"{request.DisplayViewer}'s gave back your Sword of Fire.");
                        return result;
                    }

                case "removewatersword":
                    {
                        Connector.Write8(ADDR_SwordSlot3, 0x02);
                        Connector.SendMessage($"{request.DisplayViewer}'s gave back your Sword of Water.");
                        return result;
                    }

                case "removethundersword":
                    {
                        Connector.Write8(ADDR_SwordSlot4, 0x03);
                        Connector.SendMessage($"{request.DisplayViewer}'s gave back your Sword of Thunder.");
                        return result;
                    }

                case "blackout":
                    {
                        Connector.Unfreeze(0x07E0); 
                        Connector.SendMessage($"{request.DisplayViewer}'s turned the lights back on.");
                        return result;
                    }

            }
            return result;
        }

        public override bool StopAllEffects()
        {
            return false;
        }
    }
}
