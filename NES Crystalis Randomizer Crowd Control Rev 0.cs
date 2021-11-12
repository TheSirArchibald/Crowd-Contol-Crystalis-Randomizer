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
        private const ushort ADDR_INGAMEMENU = 0x004A;
        private const ushort ADDR_Blackout1 = 0x07E0;
        private const ushort ADDR_Blackout2 = 0x07E1;
        private const ushort ADDR_Blackout3 = 0x07E2;
        private const ushort ADDR_INVIS = 0x07E4;

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
            if (!Connector.Read8(ADDR_Blackout1, out byte blackoutmode1))
            {
                DelayEffect(request);
                return false;
            }
            if (!Connector.Read8(ADDR_Blackout2, out byte blackoutmode2))
            {
                DelayEffect(request);
                return false;
            }
            if (!Connector.Read8(ADDR_Blackout3, out byte blackoutmode3))
            {
                DelayEffect(request);
                return false;
            }
            if (blackoutmode1 == 0x9A)
            {
                DelayEffect(request);
                return false;
            }
            if (blackoutmode2 == 0x9A)
            {
                DelayEffect(request);
                return false;
            }
            if (blackoutmode3 == 0x9A)
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
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
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
                () => Connector.Freeze8(ADDR_Blackout3, 0x9A) && Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout3, 0x9A) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
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
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
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
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout2, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout2, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x70) && (cavelocation <= 0x78))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x7c) && (cavelocation <= 0x87))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x88) && (cavelocation <= 0x8a))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout2, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout2, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x8c) && (cavelocation <= 0x8e))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x8f) && (cavelocation <= 0x98))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
                () => true, /*Action*/
                TimeSpan.FromSeconds(0.5),
                true);
                return true;
            }

            if ((cavelocation >= 0x9c) && (cavelocation <= 0xba))
            {

                var blackout = RepeatAction(request,
                TimeSpan.FromSeconds(45),
                () => Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Effect Start Condition*/
                () => Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout2, 0x9A) && Connector.Freeze8(ADDR_Blackout3, 0x9A), /*Start Action*/
                TimeSpan.FromSeconds(1), /*Retry Timer*/
                () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Blackout1, 0x9A) && Connector.Freeze8(ADDR_Blackout3, 0x9A) && Connector.Freeze8(ADDR_Blackout3, 0x9A) && Connector.IsNonZero8(ADDR_CURRENT_AREA), /*Refresh Condtion*/
                TimeSpan.FromMilliseconds(100), /*Refresh Retry Timer*/
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

            if ((cscale) == 0x00)
            {
                DelayEffect(request);
                return false;
            }
            
            if ((cscale + scale) == 0x2F)
            {
                DelayEffect(request);
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

        //private bool TryAlterLevel([NotNull] EffectRequest request, sbyte level)
        //{
        //    if (!Connector.Read8(ADDR_LEVEL, out byte clevel))
        //    {
        //        DelayEffect(request);
        //        return false;
        //    }

        //    if ((clevel + level) == 0x00)
        //    {
        //        Respond(request, EffectStatus.FailPermanent, "Level already at minumum.");
        //        return false;
        //    }

        //    if ((clevel + level) == 0x11)
        //    {
        //        Respond(request, EffectStatus.FailPermanent, "Level already at maximum.");
        //        return false;
        //    }

        //    if (Connector.Write8(ADDR_LEVEL, (byte)(clevel + level)))
        //    {
        //        Respond(request, EffectStatus.Success);
        //        return true;
        //    }


        //    DelayEffect(request);
        //    return false;
        //}

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
                DelayEffect(request);
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
                DelayEffect(request);
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
                    DelayEffect(request);
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

            if ((cmp == 0))
            {
                DelayEffect(request);
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
                DelayEffect(request);
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
                DelayEffect(request);
                //Respond(request, EffectStatus.FailTemporary, "Wrong location.");
                return false;
            }


            return true;
        }

        private bool FixPower([NotNull] EffectRequest request)
        {
            if (!Connector.Read8(ADDR_PowerSlot1, out byte windpower))
            {
                DelayEffect(request);
                return false;
            }
            if (!Connector.Read8(ADDR_PowerSlot2, out byte firepower))
            {
                DelayEffect(request);
                return false;
            }
            if (!Connector.Read8(ADDR_PowerSlot3, out byte waterpower))
            {
                DelayEffect(request);
                return false;
            }
            if (!Connector.Read8(ADDR_PowerSlot4, out byte thunderpower))
            {
                DelayEffect(request);
                return false;
            }


            if (windpower == 0x07)
            {
                Connector.Write8(ADDR_PowerSlot1, 06);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} fixed Tornado Bracelet.");
                return true;
            }

            if (firepower == 0x09)
            {
                Connector.Write8(ADDR_PowerSlot2, 08);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} fixed Flame Bracelet.");
                return true;
            }

            if (waterpower == 0x0B)
            {
                Connector.Write8(ADDR_PowerSlot3, 0x0A);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} fixed Blizzard Braclet.");
                return true;
            }

            if (thunderpower == 0x0D)
            {
                Connector.Write8(ADDR_PowerSlot4, 0x0C);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} fixed Strorm Braclet.");
                return true;
            }

            DelayEffect(request);
            return false;
        }



        private bool TryGiveWindPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot1, out byte windpower))

                if (windpower == 0xFF)
                {
                    Connector.Write8(ADDR_PowerSlot1, 05);
                    Respond(request, EffectStatus.Success);
                    Connector.SendMessage($"{request.DisplayViewer} sent Ball of Wind.");
                    return true;
                }

            if (windpower == 0x05)
            {
                Connector.Write8(ADDR_PowerSlot1, 06);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} sent Tornado Bracelet.");
                return true;
            }

            if (windpower == 0x06)
            {
                DelayEffect(request);
                return false;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryStealWindPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot1, out byte windpower))

                if (windpower == 0xFF)
                {
                    DelayEffect(request);
                    return false;
                }

            if (windpower == 0x05)
            {
                Connector.Write8(ADDR_PowerSlot1, 0xFF);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Ball of Wind.");
                return true;
            }

            if (windpower == 0x06)
            {
                Connector.Write8(ADDR_PowerSlot1, 0x05);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Tornado Braclet.");
                return true;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryGiveFirePower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot2, out byte firepower))

                if (firepower == 0xFF)
                {
                    Connector.Write8(ADDR_PowerSlot2, 07);
                    Respond(request, EffectStatus.Success);
                    Connector.SendMessage($"{request.DisplayViewer} sent Ball of Fire.");
                    return true;
                }

            if (firepower == 0x07)
            {
                Connector.Write8(ADDR_PowerSlot2, 08);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} sent Flame Bracelet.");
                return true;
            }

            if (firepower == 0x08)
            {
                DelayEffect(request);
                return false;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryStealFirePower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot2, out byte firepower))

                if (firepower == 0xFF)
                {
                    DelayEffect(request);
                    return false;
                }

            if (firepower == 0x07)
            {
                Connector.Write8(ADDR_PowerSlot2, 0xFF);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Ball of Fire.");
                return true;
            }

            if (firepower == 0x08)
            {
                Connector.Write8(ADDR_PowerSlot2, 0x07);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Flame Braclet.");
                return true;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryGiveWaterPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot3, out byte waterpower))

                if (waterpower == 0xFF)
                {
                    Connector.Write8(ADDR_PowerSlot3, 09);
                    Respond(request, EffectStatus.Success);
                    Connector.SendMessage($"{request.DisplayViewer} sent Ball of Water.");
                    return true;
                }

            if (waterpower == 0x09)
            {
                Connector.Write8(ADDR_PowerSlot3, 0x0A);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} sent Blizzard Bracelet.");
                return true;
            }

            if (waterpower == 0x0A)
            {
                DelayEffect(request);
                return false;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryStealWaterPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot3, out byte waterpower))

                if (waterpower == 0xFF)
                {
                    DelayEffect(request);
                    return false;
                }

            if (waterpower == 0x09)
            {
                Connector.Write8(ADDR_PowerSlot3, 0xFF);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Ball of Water.");
                return true;
            }

            if (waterpower == 0x0A)
            {
                Connector.Write8(ADDR_PowerSlot3, 0x09);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Blizzard Braclet.");
                return true;
            }

            DelayEffect(request);
            return true;
        }

        private bool TryGiveThunderPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot4, out byte thunderpower))

                if (thunderpower == 0xFF)
                {
                    Connector.Write8(ADDR_PowerSlot4, 0x0B);
                    Respond(request, EffectStatus.Success);
                    Connector.SendMessage($"{request.DisplayViewer} sent Ball of Thunder.");
                    return true;
                }
            if (thunderpower == 0x0B)
            {
                Connector.Write8(ADDR_PowerSlot4, 0x0C);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} sent Strorm Bracelet.");
                return true;
            }
            if (thunderpower == 0x0A)
            {
                DelayEffect(request);
                return false;
            }
            DelayEffect(request);
            return true;
        }

        private bool TryStealThunderPower([NotNull] EffectRequest request)
        {

            if (Connector.Read8(ADDR_PowerSlot4, out byte thunderpower))

                if (thunderpower == 0xFF)
                {
                    DelayEffect(request);
                    return false;
                }

            if (thunderpower == 0x0B)
            {
                Connector.Write8(ADDR_PowerSlot4, 0xFF);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Ball of Thunder.");
                return true;
            }

            if (thunderpower == 0x0C)
            {
                Connector.Write8(ADDR_PowerSlot4, 0x0B);
                Respond(request, EffectStatus.Success);
                Connector.SendMessage($"{request.DisplayViewer} stole Strorm Braclet.");
                return true;
            }
            DelayEffect(request);
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
                DelayEffect(request);
                //Respond(request, EffectStatus.FailPermanent, "Near Death Fail.");
                return false;
            }

            if ((bosschealth + ((bossscale * slope) / 2)) > 255)
            {
                DelayEffect(request);
                //Respond(request, EffectStatus.FailTemporary, "Health already at maximum.");
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
                DelayEffect(request);
                return false;
            }

            if ((bosschealth2 + ((bossscale2 * slope2) / 2)) > 255)
            {
                DelayEffect(request);
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
                    new Effect("Heal Player to Full", "healplayer") {Price = 10, Description = "Player will be healed to full health"},
                    new Effect("Hurt Player (Scaled %)", "hurtplayer") {Price = 10, Description = "Player will be hurt"},
                    new Effect("Fix Power Upgrade", "fixpower") {Price = 0, Description = "Temporary fix for power upgrade"},
                    new Effect("Fix Scaling", "fixscale") {Price = 0, Description = "Temporary fix for over scaling"},
                    new Effect("Kill Player", "kill") {Price = 40, Description = "Instantly Kill the Player"},
                    new Effect("Free Shopping", "freeshops") {Price = 10, Description = "Temporary make Item/Armor Shops free"},
                    new Effect("Mado Screen Shake Mode", "screenshakemode") {Price = 10, Description = "Temporary make the screen shake"},
                    new Effect("Blackout Mode", "blackout") {Price = 10, Description = "Temporary make the world dark"},
                    new Effect("Camouflage Mode", "invis") {Price = 10, Description = "Temporary turn the player invisible"},
                    new Effect("Wild Warp", "wild") {Price = 40, Description = "Warp the Player back to the start location of the game"},
                    new Effect("One Hit KO", "ohko") {Price = 20, Description = "Temporary alter the players health to one.  Health will be restored after effect ends"},
                    //new Effect("Reset", "reset"),   

                    //Scaling and Leveling
                    new Effect("Scaling and Leveling", "sandl", ItemKind.Folder),
                    new Effect("Scaling Increase", "scaleup", "sandl") {Price = 10, Description = "Increase Player's Scaling by one"},
                    new Effect("Scaling Decrease", "scaledown", "sandl") {Price = 10, Description = "Decrease Player's Scaling by one"},
                    new Effect("Level Increase", "levelup", "sandl") {Price = 10, Description = "Increase Player's Level by one"},
                    new Effect("Level Decrease", "leveldown", "sandl") {Price = 10, Description = "Decrease Player's Level by one"},
                                        
                    //Magic
                    new Effect("Magic", "magic", ItemKind.Folder),
                    new Effect("Refill Magic to Full", "magicup", "magic") {Price = 10, Description = "Refill Player's Magic to Full"},
                    new Effect("Take Magic (Scaled %)", "magicdown", "magic") {Price = 4, Description = "Steal some of Player's Magic" },
                    //Money
                    new Effect("Money", "money", ItemKind.Folder),
                    new Effect("Give Money (50 Gold)", "givemoney50", "money") {Price = 2, Description = "Give 50 gold to the Player" },
                    new Effect("Give Money (100 Gold)", "givemoney100", "money") {Price = 4, Description = "Give 100 gold to the Player" },
                    new Effect("Steal Money (50 Gold)", "takemoney50", "money") {Price = 2, Description = "Steal 50 gold from the Player" },
                    new Effect("Steal Money (100 Gold)", "takemoney100", "money") {Price = 4, Description = "Steal 100 gold from the Player" },
                                        
                    //Heal Boss
                    //Note all boss fights check for boss area map to see if Effect can trigger but you can despawn the effect.  Will need to add screen fight transmition animation RAM action for better trigger.
                    new Effect("Heal Boss (X% Scaled)", "healboss", ItemKind.Folder),
                    new Effect("Vampire", "healvamp1", "healboss") {Price = 10, Description = "Heal Vampire" },
                    new Effect("Big Bug", "healbug", "healboss") {Price = 10, Description = "Heal Big Bug" },
                    new Effect("Kelbesque", "healkelby", "healboss") {Price = 10, Description = "Heal Kelbesque" },
                    new Effect("Vampire2", "healvamp2", "healboss") {Price = 10, Description = "Heal Vampire2" },
                    new Effect("Sabera", "healsabera", "healboss") {Price = 10, Description = "Heal Sabera" },
                    new Effect("Mado", "healmado", "healboss") {Price = 10, Description = "Heal Mado" },
                    new Effect("Kelbesque2", "healkelby2", "healboss") {Price = 10, Description = "Heal Kelbesque2" },
                    new Effect("Sabera2", "healsabera2", "healboss") {Price = 10, Description = "Heal Sabera2" },
                    new Effect("Mado2", "healmado2", "healboss") {Price = 10, Description = "Heal Mado2" },
                    new Effect("Karmine", "healkarmine", "healboss") {Price = 10, Description = "Heal Karmine" },
                    new Effect("Draygon", "healdraygon", "healboss") {Price = 10, Description = "Heal Draygon1" },
                    new Effect("Draygon2", "healdraygon2", "healboss") {Price = 10, Description = "Heal Draygon2" },
                    new Effect("Dyna", "healdyna", "healboss") {Price = 10, Description = "Heal Dyna" },

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
                    new Effect("Warrior Ring Mode", "lvl1shot", "projectile") {Price = 4, Description = "Temporary give player Lvl 1 projectile shots" },
                    new Effect("LVL 2 Warrior Ring Mode", "lvl2shot", "projectile") {Price = 4, Description = "Temporarygive player Lvl 2 projectile shots" },
                    new Effect("TriShot Mode", "trishot", "projectile") {Price = 4, Description = "Temporary give player Lightning Bolt projectile shots" },
                    new Effect("Thunder Mode", "thundershot", "projectile") {Price = 4, Description = "Temporary give player Thunder projectile shots" },
                    new Effect("Lag Storm Mode", "lagshot", "projectile") {Price = 4, Description = "Temporary give player Storm projectile shots" },

                   
                    // Movement Effects
                    new Effect("Movement Effects", "moveeffect", ItemKind.Folder),
                    new Effect("Jump Mode", "jump", "moveeffect") {Price = 4, Description = "Temporary make the player jump" },
                    new Effect("Flight Mode", "flightmode", "moveeffect") {Price = 4, Description = "Temporary make the player fly" },
                    new Effect("Heavy Mode", "heavy", "moveeffect") {Price = 4, Description = "Temporary make the player walk slower" },
                    
                    // Status Conditions
                    new Effect("Change Condition", "changecondition", ItemKind.Folder),
                    new Effect("Cure Player", "recover", "changecondition")  {Price = 4, Description = "Instantly cure all aliment affecting the player" },
                    new Effect("UnTimed Posion", "poison", "changecondition")  {Price = 10, Description = "Instantly poison the player" },
                    new Effect("UnTimed Paralysis", "paralysis", "changecondition")  {Price = 10, Description = "Instantly paralysis the player" },
                    new Effect("UnTimed Slime", "slime", "changecondition")  {Price = 10, Description = "Instantly slime the player" },
                    new Effect("UnTimed Stone", "stone", "changecondition")  {Price = 10, Description = "Instantly stone the player for a few seconds" },
                    new Effect("Timed Posion", "timedpoison", "changecondition")  {Price = 10, Description = "Temporary poison the player" },
                    new Effect("Timed Paralysis", "timedparalysis", "changecondition")  {Price = 10, Description = "Temporary paralysis the player" },
                    new Effect("Timed Slime", "timedslime", "changecondition")  {Price = 10, Description = "Temporary slime the player" },

                    // Power Down/Upgrade     Note: Ro flag is required for Crowd Control Flags.
                    new Effect("Power Upgrade","givepower", ItemKind.Folder),
                    new Effect("Wind Upgrade", "givewind", "givepower")  {Price = 20, Description = "Give the player a Wind Power upgrade" },
                    new Effect("Fire Upgrade", "givefire", "givepower")  {Price = 20, Description = "Give the player a Fire Power upgrade" },
                    new Effect("Water Upgrade", "givewater", "givepower")  {Price = 20, Description = "Give the player a Water Power upgrade" },
                    new Effect("Thunder Upgrade", "givethunder", "givepower")  {Price = 20, Description = "Give the player a Thunder Power upgrade" },

                    new Effect("Power Downgrade","takepower", ItemKind.Folder),
                    new Effect("Wind Downgrade", "stealwind", "takepower")  {Price = 20, Description = "Steal a Wind Power upgrade from the player" },
                    new Effect("Fire Downgrade", "stealfire",  "takepower")  {Price = 20, Description = "Steal a Fire Power upgrade from the player" },
                    new Effect("Water Downgrade", "stealwater", "takepower")  {Price = 200, Description = "Steal a Water Power upgrade from the player" },
                    new Effect("Thunder Downgrade", "stealthunder",   "takepower")  {Price = 20, Description = "Steal a Thunder Power upgrade from the player" },

                    // Sword Effects Timed
                    new Effect("Sword Effects","sword", ItemKind.Folder),
                    new Effect("Steal Wind Sword (Timed)", "removewindsword", "sword")  {Price = 10, Description = "Temporary steal the Wind Sword from the player" },
                    new Effect("Steal Fire Sword (Timed)", "removefiresword", "sword")  {Price = 10, Description = "Temporary steal the Fire Sword from the player" },
                    new Effect("Steal Water Sword (Timed)", "removewatersword", "sword")  {Price = 10, Description = "Temporary steal the Water Sword from the player" },
                    new Effect("Steal Thunder Sword (Timed)", "removethundersword", "sword")  {Price = 10, Description = "Temporary steal the Thunder Sword from the player" },
                    new Effect("Equip Wind Sword (Timed)", "windsword", "sword")  {Price = 10, Description = "Temporary force equip the Wind Sword" },
                    new Effect("Equip Fire Sword (Timed)", "firesword", "sword")  {Price = 10, Description = "Temporary force equip the Fire Sword" },
                    new Effect("Equip Water Sword (Timed)", "watersword", "sword")  {Price = 10, Description = "Temporary force equip the Water Sword" },
                    new Effect("Equip Thunder Sword (Timed)", "thundersword", "sword")  {Price = 10, Description = "Temporary force equip the Thunder Sword" },
                    new Effect("Equip Crystalis Sword (Timed)", "crystalissword", "sword")  {Price = 10, Description = "Temporary force equip Crystalis" },
                    
                                        
                    //Shields
                    new Effect("Shields","giveshield", ItemKind.Folder),
                    new Effect("Steal Best Shield", "stealbestshield","giveshield") {Price = 5, Description = "Steal best shield from the player" },
                    new Effect("Send Carapace Shield", "carpshield","giveshield") {Price = 5, Description = "Give the Carapace Shield to the player" },
                    new Effect("Send Bronze Shield", "broshield","giveshield") {Price = 5, Description = "Give the Bronze Shield to the player" },
                    new Effect("Send Platinum Shield", "platshield","giveshield") {Price = 5, Description = "Give the Platinum Shield to the player" },
                    new Effect("Send Mirror Shield", "mirrorshield","giveshield") {Price = 10, Description = "Give the Mirror Shield to the player" },
                    new Effect("Send Ceramic Shield", "cershield","giveshield") {Price = 10, Description = "Give the Ceramic Shield to the player" },
                    new Effect("Send Sacred Shield", "sshield","giveshield") {Price = 10, Description = "Give the Sacred Shield to the player" },
                    new Effect("Send Battle Shield", "bshield","giveshield") {Price = 20, Description = "Give the Battle Shield to the player" },
                    new Effect("Send Psycho Shield", "pshield","giveshield") {Price = 30, Description = "Give the Psycho Shield to the player" },
                   
                    //Armor
                    new Effect("Armors","givearmor", ItemKind.Folder),
                    new Effect("Steal Best Armor", "stealbestarmor","givearmor") {Price = 5, Description = "Steal best armor from the player" },
                    new Effect("Send Tanned Hide ", "tanhide","givearmor") {Price = 5, Description = "Give the Tanned Hide to the player" },
                    new Effect("Send Leather Armor ", "leaarmor","givearmor") {Price = 5, Description = "Give the Leather Armor to the player" },
                    new Effect("Send Bronze Armor ", "broarmor","givearmor") {Price = 5, Description = "Give the Bronze Armor to the player" },
                    new Effect("Send Platinum Armor ", "platarmor","givearmor") {Price = 10, Description = "Give the Platinum Armor to the player" },
                    new Effect("Send Soldier Suit ", "soldiersuit","givearmor") {Price = 10, Description = "Give the Soldier Suit to the player" },
                    new Effect("Send Ceramic Suit ", "cerarmor","givearmor") {Price = 10, Description = "Give the Ceramic Suit to the player" },
                    new Effect("Send Battle Armor ", "barmor","givearmor") {Price = 20, Description = "Give the Battle Armor to the player" },
                    new Effect("Send Psycho Armor", "parmor", "givearmor") {Price = 30, Description = "Give the Psycho Armor to the player" },

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
                    new Effect("Give Medical Herb", "giveherb","giveconsumable") {Price = 5, Description = "Give a Medical Herb to the player" },
                    new Effect("Give Antidote", "giveanti","giveconsumable") {Price = 5, Description = "Give an Antidote to the player" },
                    new Effect("Give Magic Ring", "givemr","giveconsumable") {Price = 5, Description = "Give a Magic Ring to the player" },
                    new Effect("Give Lysis Plant", "givelp","giveconsumable") {Price = 5, Description = "Give a Lysis Plant to the player" },
                    new Effect("Give Warp Boots", "givewb","giveconsumable") {Price = 5, Description = "Give Warp Boots to the player" },
                    new Effect("Give Fruit of Power", "givefop","giveconsumable") {Price = 5, Description = "Give a Fruit of Power to the player" },
                    new Effect("Give Fruit of Repun", "givefor","giveconsumable") {Price = 5, Description = "Give a Fruit of Repun to the player" },
                    new Effect("Give Fruit of Lime", "givefol","giveconsumable") {Price = 5, Description = "Give a Fruit of Lime to the player" },
                    new Effect("Give Opel Statue", "giveopel","giveconsumable") {Price = 10, Description = "Give an Opel Statue to the player" },
                    new Effect("Give Fruit of Lime Care Package", "allfol","giveconsumable") {Price = 10, Description = "Give delightful care fruit of the Lime Care Package to the player" },

                    new Effect("Take Consumable","takeconsumable", ItemKind.Folder),
                    new Effect("Clear Inventory", "clear","takeconsumable") {Price = 20, Description = "Clear the player's consumable inventory" },
                    new Effect("Steal Herb", "stealherb","takeconsumable") {Price = 5, Description = "Steal a Medical Herb from the player" },
                    new Effect("Steal Antidote", "stealanti","takeconsumable") {Price = 5, Description = "Steal an Antidote from the player" },
                    new Effect("Steal Magic Ring", "stealmr","takeconsumable") {Price = 5, Description = "Steal a Magic Ring from the player" },
                    new Effect("Steal Lysis Plant", "steallysis","takeconsumable") {Price = 5, Description = "Steal a Lysis Plant from the player" },
                    new Effect("Steal Warp Boots", "stealwp","takeconsumable") {Price = 5, Description = "Steal Warp Boots from the player" },
                    new Effect("Steal Fruit of Power", "stealfop","takeconsumable") {Price = 5, Description = "Steal a Fruit of Power from the player" },
                    new Effect("Steal Fruit of Repun", "stealfor","takeconsumable") {Price = 5, Description = "Steal a Fruit of Repun from the player" },
                    new Effect("Steal Fruit of Lime", "stealfol","takeconsumable") {Price = 5, Description = "Steal a Fruit of Lime from the player" },
                    new Effect("Steal Opel Statue", "stealopel","takeconsumable") {Price = 10, Description = "Steal a Opel Statue from the player" },
                   
//  To Be Built Out (ASM)
                    //Spawn Enemy
                    // Note unclear how to read spawn tables to get proper spawn or any for now.
                    //new Effect("Spawn Enemy", "spawn"),
                                                                                
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
                DelayEffect(request);
                return;
            }
            string[] effectT = request.FinalCode.Split('_');
            switch (effectT[0])
            {
                case "invis":
                    {
                        var invs = RepeatAction(request,
                        TimeSpan.FromSeconds(45),
                        () => Connector.IsZero8(ADDR_INVIS), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_INVIS, 0xFA), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_INVIS, 0xFA), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        invs.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started invisible mode (45s)."));
                        return;
                    }

                case "ohko":  //Need to fix how I do a constant update the HUD
                    {

                        byte origHP = 01;
                        var w = RepeatAction(request, TimeSpan.FromSeconds(30),
                            () => Connector.Read8(ADDR_HP, out origHP) && (origHP > 1),
                            () => Connector.Write8(ADDR_HP, 0x01),
                            TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_U2HOOK, 0x01) && Connector.Write8(ADDR_U1HOOK, 0x01) && Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_HP), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_HP, 0x01), TimeSpan.FromSeconds(2), true, "health");
                        w.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} sent One Hit KO Mode (30s)."));
                        w.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_HP, origHP);
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage("One Hit KO Removed.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        });
                        return;


                    }

                case "wild":
                    {
                        TryEffect(request,
                        () => Connector.Write8(ADDR_U2HOOK, 0x03),
                        () => Connector.Write8(ADDR_U1HOOK, 0x01),
                        () => { Connector.SendMessage($"{request.DisplayViewer} wild warped you."); });
                        return;
                    }


                //{
                //    Connector.Write8(ADDR_U2HOOK, 0x03);
                //    Connector.SendMessage($"{request.DisplayViewer} wild warped you.");
                //    Connector.Write8(ADDR_U1HOOK, 0x01);
                //    return;
                //}                    

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
                      () => Connector.Write8(ADDR_Jump, 32) && Connector.SendMessage($"{request.DisplayViewer} has granted you jump mode (15s)."), TimeSpan.FromSeconds(0.5),
                      () => true, TimeSpan.FromSeconds(5),
                      () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Jump, 32), TimeSpan.FromSeconds(0.5), true)
                      .WhenCompleted.ContinueWith(t => Connector.SendMessage($"{request.DisplayViewer} has removed your jump mode."));
                        return;
                    }

                case "flightmode":  //Note Screen scroll weird in x only direction but up and down and diagronally work great.
                    {
                        var flight = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.IsZero8(ADDR_Jump), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Jump, 0x20), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Jump, 0x20) && Connector.IsNonZero8(ADDR_Jump), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        flight.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started flight mode (15s)."));
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_ScreenHit2, 0x02), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        shake.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} started shaking mode (15s)."));
                        shake.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s removed shaking mode."));
                        return;
                    }

                case "heavy":
                    {
                        var heavy = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Speed, out byte b) && (b >= 0x06), /*Effect Start Condition*/
                        () => Connector.Write8(ADDR_Speed, 0x03), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Speed, 0x03), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        heavy.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} lowered your speed (15s)."));
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
                        byte eqsword = 01;
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Equip_Sword, out eqsword) && (eqsword > 00), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x08), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x08), /*Refresh Condtion*/
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
                        byte eqsword = 01;
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Equip_Sword, out eqsword) && (eqsword > 00), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x10), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x10), /*Refresh Condtion*/
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
                        byte eqsword = 01;
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Equip_Sword, out eqsword) && (eqsword > 00), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x68), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x68), /*Refresh Condtion*/
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
                        byte eqsword = 01;
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Equip_Sword, out eqsword) && (eqsword > 00), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x70), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x70), /*Refresh Condtion*/
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
                        byte eqsword = 01;
                        var war = RepeatAction(request,
                        TimeSpan.FromSeconds(15),
                        () => Connector.Read8(ADDR_Equip_Sword, out eqsword) && (eqsword > 00), /*Effect Start Condition*/
                        () => Connector.Freeze8(ADDR_Warrior, 0x78), /*Start Action*/
                        TimeSpan.FromSeconds(1), /*Retry Timer*/
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Freeze8(ADDR_Warrior, 0x78), /*Refresh Condtion*/
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
                            DelayEffect(request);
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 00, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 0);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} cured you.");
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
                            DelayEffect(request);
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 01, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 01);
                            Respond(request, EffectStatus.Success);
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x01), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        cond.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} paralysised you (15s)."));
                        return;
                    }

                case "stone":
                    {
                        if (!Connector.Read8(ADDR_Stone, out byte stone))
                        {
                            DelayEffect(request);
                        }
                        else if ((stone) >= 0x01)
                        {
                            DelayEffect(request);
                            return;
                        }
                        else if (!Connector.SetBits(ADDR_Stone, 255, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Stone, 255);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} stoned you for 4 seconds.");
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
                            DelayEffect(request);
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 03, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 03);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} poisoned you.");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x03), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true);
                        cond.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} poisoned you (15s)."));
                        return;
                    }

                case "slime":   //Note will need to fail at boss area since it would lock you there or death.
                    {
                        if (!Connector.Read8(ADDR_CURRENT_AREA, out byte areas))
                        {
                            DelayEffect(request);
                        }
                        //Boss area excluded because it is too troll.
                        if ((areas) == 0x0A)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0x1A)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0x28)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0x6c)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0x6e)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xf2)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xa9)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xac)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xb9)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xb6)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0x9f)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas) == 0xa6)
                        {
                            DelayEffect(request);
                            return;
                        }

                        if ((areas >= 0x58) && (areas <= 0x5f))
                        {
                            DelayEffect(request);
                            return;
                        }
                        //End Boss areas
                        if (!Connector.Read8(ADDR_Condition, out byte con))
                        {
                            DelayEffect(request);
                        }
                        else if ((con) >= 0x01)
                        {
                            DelayEffect(request);
                        }
                        else if (!Connector.SetBits(ADDR_Condition, 04, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_Condition, 04);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} slimed you.");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Condition, 0x04) && Connector.Freeze8(ADDR_Condition, 0x04) && Connector.IsNonZero8(ADDR_Condition), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        slime.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} deployed slimed Effect (15s)."));
                        slime.WhenCompleted.Then(t => Connector.SendMessage($"{request.DisplayViewer}'s slimed Effect has dispered."));
                        return;
                    }

                case "fixpower":
                    FixPower(request);
                    return;

                case "givewind":
                    TryGiveWindPower(request);
                    return;

                case "givefire":
                    TryGiveFirePower(request);
                    return;

                case "givewater":
                    TryGiveWaterPower(request);
                    return;

                case "givethunder":
                    TryGiveThunderPower(request);
                    return;

                case "stealwind":

                    TryStealWindPower(request);
                    return;


                case "stealfire":
                    TryStealFirePower(request);
                    return;

                case "stealwater":
                    TryStealWaterPower(request);
                    return;

                case "stealthunder":
                    TryStealThunderPower(request);
                    return;



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
                                DelayEffect(request);
                            }
                            else if ((barmor2) == 0x1B)
                            {
                                DelayEffect(request);
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
                                    DelayEffect(request);
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
                                        DelayEffect(request);
                                    }
                                    else if ((barmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x1B, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x1B);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor.");
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
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor" +
                                        $"");
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
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor" +
                                    $"");
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
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Battle Armor.");
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
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x1C)
                            {
                                DelayEffect(request);
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
                                    DelayEffect(request);
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
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x1C, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x1C);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor.");
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
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor.");
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
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Armor.");
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
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Hide.");
                        }
                        return;
                    }

                case "tanhide":
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
                            else if ((parmor1) == 0x15)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x15)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x15)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x15)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x15, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x15);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Tanned Hide.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x15, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x15);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Tanned Hide.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x15, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x15);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Tanned Hide.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x15, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x15);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Tanned Hide.");
                        }
                        return;
                    }

                case "leaarmor":
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
                            else if ((parmor1) == 0x16)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x16)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x16)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x16)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x16, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x16);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Leather Armor.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x16, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x16);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Leather Armor.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x16, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x16);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Leather Armor.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x16, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x16);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Leather Armor.");
                        }
                        return;
                    }

                case "broarmor":
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
                            else if ((parmor1) == 0x17)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x17)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x17)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x17)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x17, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x17);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Armor.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x17, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x17);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Armor.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x17, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x17);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Armor.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x17, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x17);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Armor.");
                        }
                        return;
                    }

                case "platarmor":
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
                            else if ((parmor1) == 0x18)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x18)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x18)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x18)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x18, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x18);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Armor.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x18, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x18);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Armor.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x18, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x18);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Armor.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x18, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x18);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Armor.");
                        }
                        return;
                    }

                case "cerarmor":
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
                            else if ((parmor1) == 0x1A)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x1A)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x1A)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x1A)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x1A, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x1A);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Armor.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x1A, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x1A);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Armor.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x1A, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x1A);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Armor.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x1A, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x1A);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Armor.");
                        }
                        return;
                    }

                case "soldiersuit":
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
                            else if ((parmor1) == 0x19)
                            {
                                DelayEffect(request);
                            }
                            else if ((parmor2) == 0x19)
                            {
                                DelayEffect(request);
                            }

                            else if ((parmor2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot3, out byte parmor3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Armor Sent Already Check
                                else if ((parmor3) == 0x19)
                                {
                                    DelayEffect(request);
                                }
                                else if ((parmor3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot4, out byte parmor4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Armor Sent Already Check
                                    else if ((parmor4) == 0x19)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((parmor4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot4, 0x19, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot4, 0x19);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Soldier Suit.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot3, 0x19, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot3, 0x19);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Soldier Suit.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot2, 0x19, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot2, 0x19);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Soldier Suit.");
                            }
                            return;
                        }

                        else if (!Connector.SetBits(ADDR_ArmorSlot1, 0x19, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot1, 0x19);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Soldier Suit.");
                        }
                        return;
                    }

                case "stealbestarmor":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ArmorSlot4, out byte pshield4))
                        {
                            DelayEffect(request);
                        }
                        else if ((pshield4) == 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ArmorSlot3, out byte pshield3))
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield3) == 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ArmorSlot2, out byte pshield2))
                                {
                                    DelayEffect(request);
                                }

                                else if ((pshield2) == 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ArmorSlot1, out byte pshield1))
                                    {
                                        DelayEffect(request);
                                    }

                                    else if ((pshield1) == 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ArmorSlot1, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ArmorSlot1, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} stole your best armor.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ArmorSlot2, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ArmorSlot2, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} stole your best armor.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ArmorSlot3, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ArmorSlot3, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} stole your best armor.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ArmorSlot4, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ArmorSlot4, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} stole your best armor.");
                        }
                        return;
                    }

                case "stealbestshield":
                    {
                        //Slot 1 Check
                        if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                        {
                            DelayEffect(request);
                        }
                        else if ((pshield4) == 0xFF)
                        {
                            //Slot 2 Check
                            if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                            {
                                DelayEffect(request);
                            }
                                                     
                            else if ((pshield3) == 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot2, out byte pshield2))
                                {
                                    DelayEffect(request);
                                }
                               
                                else if ((pshield2) == 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot1, out byte pshield1))
                                    {
                                        DelayEffect(request);
                                    }
                                    
                                    else if ((pshield1) == 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot1, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot1, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} stole your best shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot2, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot2, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} stole your best shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot3, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot3, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} stole your best shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot4, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot4, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} stole your best shield.");
                        }
                        return;
                    }

                case "carpshield":
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
                            else if ((pshield1) == 0x0D)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x0D)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x0D)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x0D)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x0D, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x0D);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Carapace Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x0D, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x0D);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Carapace Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x0D, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot2, 0x0D);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Carapace Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x0D, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x0D);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Carapace Shield.");
                        }
                        return;
                    }

                case "broshield":
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
                            else if ((pshield1) == 0x0E)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x0E)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x0E)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x0E)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x0E, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x0E);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x0E, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x0E);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x0E, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot2, 0x0E);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x0E, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x0E);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Bronze Shield.");
                        }
                        return;
                    }

                case "platshield":
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
                            else if ((pshield1) == 0x0F)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x0F)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x0F)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x0F)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x0F, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x0F);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x0F, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x0F);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x0F, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot3, 0x0F);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x0F, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x0F);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Platinum Shield.");
                        }
                        return;
                    }

                case "mirrorshield":
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
                            else if ((pshield1) == 0x10)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x10)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x10)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x10)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x10, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x10);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Mirror Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x10, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x10);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Mirror Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x10, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot3, 0x10);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Mirror Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x10, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x10);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Mirror Shield.");
                        }
                        return;
                    }

                case "cershield":
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
                            else if ((pshield1) == 0x11)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x11)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x11)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x11)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x11, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x11);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x11, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x11);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x11, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot3, 0x11);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x11, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x11);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Ceramic Shield.");
                        }
                        return;
                    }

                case "bshield":
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
                            else if ((pshield1) == 0x13)
                            {
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x13)
                            {
                                DelayEffect(request);
                            }

                            else if ((pshield2) < 0xFF)
                            {
                                //Slot 3 Check
                                if (!Connector.Read8(ADDR_ShieldSlot3, out byte pshield3))
                                {
                                    DelayEffect(request);
                                }
                                //Psycho Shield Sent Already Check
                                else if ((pshield3) == 0x13)
                                {
                                    DelayEffect(request);
                                }
                                else if ((pshield3) < 0xFF)
                                {
                                    //Slot 4 Check
                                    if (!Connector.Read8(ADDR_ShieldSlot4, out byte pshield4))
                                    {
                                        DelayEffect(request);
                                    }
                                    //Psycho Shield Sent Already Check
                                    else if ((pshield4) == 0x13)
                                    {
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x13, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x13);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Battle Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x13, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x13);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Battle Shield.");
                                }
                                return;
                            }
                            //Slot 2 Write
                            else if (!Connector.SetBits(ADDR_ShieldSlot2, 0x13, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ShieldSlot3, 0x13);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Battle Shield.");
                            }
                            return;
                        }
                        //Slot 1 Write
                        else if (!Connector.SetBits(ADDR_ShieldSlot1, 0x13, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ShieldSlot1, 0x13);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Battle Shield.");
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
                                DelayEffect(request);
                            }
                            else if ((pshield2) == 0x14)
                            {
                                DelayEffect(request);
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
                                    DelayEffect(request);
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
                                        DelayEffect(request);
                                    }
                                    else if ((pshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x14, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x14);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x14, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x14);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield.");
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
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield.");
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
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Psycho Shield.");
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
                                DelayEffect(request);
                            }
                            else if ((sshield2) == 0x12)
                            {
                                DelayEffect(request);
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
                                    DelayEffect(request);
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
                                        DelayEffect(request);
                                    }
                                    else if ((sshield4) < 0xFF)
                                    {
                                        DelayEffect(request);
                                    }
                                    //Slot 4 Write
                                    else if (!Connector.SetBits(ADDR_ShieldSlot4, 0x12, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ShieldSlot4, 0x12);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield.");
                                    }
                                    return;
                                }
                                //Slot 3 Write
                                else if (!Connector.SetBits(ADDR_ShieldSlot3, 0x12, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ShieldSlot3, 0x12);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield.");
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
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield.");
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
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} gave you Sacred Shield.");
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
                                                            Respond(request, EffectStatus.Success);
                                                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 8.");
                                                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 7.");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 6.");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 5.");
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 4.");
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 3.");
                        }



                        if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 2.");
                        }
                        if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Connector.SendMessage($"{request.DisplayViewer} stole your inventory SLOT 1.");
                        }
                        return;

                    }

                case "allfol":  //Note Community Joke for this most usless item
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
                                                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot7, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot7, 0x20);
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot6, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot6, 0x20);
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot5, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot5, 0x20);
                        }

                        if (!Connector.SetBits(ADDR_ConsumeSlot4, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot4, 0x20);
                        }


                        if (!Connector.SetBits(ADDR_ConsumeSlot3, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot3, 0x20);
                        }



                        if (!Connector.SetBits(ADDR_ConsumeSlot2, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot2, 0x20);
                        }
                        if (!Connector.SetBits(ADDR_ConsumeSlot1, 0x20, out _))
                            DelayEffect(request);

                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0x20);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} loves you so much.");
                            Connector.SendMessage($"{request.DisplayViewer} sent you a care package of 8 Fruit of Limes. Get Stomping.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Medical Herb.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Medical Herb.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
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
                                                    Respond(request, EffectStatus.Success);
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
                                                Respond(request, EffectStatus.Success);
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
                                            Respond(request, EffectStatus.Success);
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
                                        Respond(request, EffectStatus.Success);
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
                                    Respond(request, EffectStatus.Success);
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
                                Respond(request, EffectStatus.Success);
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
                            Respond(request, EffectStatus.Success);
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Antidote.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Lysis Plant.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Lysis Plant.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Lime.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Lime.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Power.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Power.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Magic Ring.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Magic Ring.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Fruit of Repun.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Fruit of Repun.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, item, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, item);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, item, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, item);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, item, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, item);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, item, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, item);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, item, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, item);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, item, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, item);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, item, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, item);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} sent Warp Boots.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Warp Boots.");
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, item, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, item);
                                                        Respond(request, EffectStatus.Success);
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
                                                    Respond(request, EffectStatus.Success);
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
                                                Respond(request, EffectStatus.Success);
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
                                            Respond(request, EffectStatus.Success);
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
                                        Respond(request, EffectStatus.Success);
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
                                    Respond(request, EffectStatus.Success);
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
                                Respond(request, EffectStatus.Success);
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
                            Respond(request, EffectStatus.Success);
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
                                                        DelayEffect(request);
                                                    }
                                                    else if (!Connector.SetBits(ADDR_ConsumeSlot8, 0xFF, out _))
                                                    {
                                                        DelayEffect(request);
                                                    }
                                                    else
                                                    {
                                                        Connector.Write8(ADDR_ConsumeSlot8, 0xFF);
                                                        Respond(request, EffectStatus.Success);
                                                        Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                                    }
                                                }
                                                else if (!Connector.SetBits(ADDR_ConsumeSlot7, 0xFF, out _))
                                                {
                                                    DelayEffect(request);
                                                }
                                                else
                                                {
                                                    Connector.Write8(ADDR_ConsumeSlot7, 0xFF);
                                                    Respond(request, EffectStatus.Success);
                                                    Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                                }
                                            }
                                            else if (!Connector.SetBits(ADDR_ConsumeSlot6, 0xFF, out _))
                                            {
                                                DelayEffect(request);
                                            }
                                            else
                                            {
                                                Connector.Write8(ADDR_ConsumeSlot6, 0xFF);
                                                Respond(request, EffectStatus.Success);
                                                Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                            }
                                        }
                                        else if (!Connector.SetBits(ADDR_ConsumeSlot5, 0xFF, out _))
                                        {
                                            DelayEffect(request);
                                        }
                                        else
                                        {
                                            Connector.Write8(ADDR_ConsumeSlot5, 0xFF);
                                            Respond(request, EffectStatus.Success);
                                            Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                        }
                                    }
                                    else if (!Connector.SetBits(ADDR_ConsumeSlot4, 0xFF, out _))
                                    {
                                        DelayEffect(request);
                                    }
                                    else
                                    {
                                        Connector.Write8(ADDR_ConsumeSlot4, 0xFF);
                                        Respond(request, EffectStatus.Success);
                                        Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                    }
                                }
                                else if (!Connector.SetBits(ADDR_ConsumeSlot3, 0xFF, out _))
                                {
                                    DelayEffect(request);
                                }
                                else
                                {
                                    Connector.Write8(ADDR_ConsumeSlot3, 0xFF);
                                    Respond(request, EffectStatus.Success);
                                    Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                                }
                            }
                            else if (!Connector.SetBits(ADDR_ConsumeSlot2, 0xFF, out _))
                            {
                                DelayEffect(request);
                            }
                            else
                            {
                                Connector.Write8(ADDR_ConsumeSlot2, 0xFF);
                                Respond(request, EffectStatus.Success);
                                Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                            }
                        }
                        else if (!Connector.SetBits(ADDR_ConsumeSlot1, 0xFF, out _))
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_ConsumeSlot1, 0xFF);
                            Respond(request, EffectStatus.Success);
                            Connector.SendMessage($"{request.DisplayViewer} took Opel Statue.");
                        }
                        return;
                    }

                case "windsword":
                    {
                        byte windsword = 01;
                        var f = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out windsword) && (windsword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} forced you to use only use Wind Sword (15s)."), TimeSpan.FromSeconds(1),
                            () => Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_Equip_Sword, 0x01), TimeSpan.FromSeconds(1), true, "sword");
                        f.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x01);
                            Connector.SendMessage("Locked Sword Removed. Reequip Sword");
                        });
                        return;
                    }

                case "firesword":
                    {
                        byte firesword = 02;
                        var g = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out firesword) && (firesword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} forced you to use only use Fire Sword (15s)."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Write8(ADDR_Equip_Sword, 0x02), TimeSpan.FromSeconds(1), true, "sword");
                        g.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x02);
                            Connector.SendMessage("Locked Sword Removed. Reequip Sword");
                        });
                        return;
                    }

                case "watersword":
                    {
                        byte watersword = 03;
                        var h = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out watersword) && (watersword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} forced you to use only use Water Sword (15s)."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x03), TimeSpan.FromSeconds(1), true, "sword");
                        h.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, watersword);
                            Connector.SendMessage("Locked Sword Removed. Reequip Sword");
                        });
                        return;
                    }

                case "thundersword":
                    {
                        byte thundersword = 04;
                        var j = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out thundersword) && (thundersword <= 05),
                            () => Connector.SendMessage($"{request.DisplayViewer} forced you to use only use Thunder Sword (15s)."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x04), TimeSpan.FromSeconds(1), true, "sword");
                        j.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, thundersword);
                            Connector.SendMessage("Locked Sword removed. Reequip Sword");
                        });
                        return;
                    }

                case "crystalissword":
                    { // Note it only stabs for now.  Need to review if I can autofill projectile to match Dyna Fight.

                        byte cysword = 05;
                        var k = RepeatAction(request, TimeSpan.FromSeconds(15),
                            () => Connector.Read8(ADDR_Equip_Sword, out cysword) && (cysword <= 5),
                            () => Connector.SendMessage($"{request.DisplayViewer} forced you to use only use Crystalis Sword (15s)."), TimeSpan.FromSeconds(1),
                            () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsNonZero8(ADDR_Equip_Sword), TimeSpan.FromSeconds(1),
                            () => Connector.Write8(ADDR_Equip_Sword, 0x05), TimeSpan.FromSeconds(1), true, "sword");
                        k.WhenCompleted.Then(t =>
                        {
                            Connector.Write8(ADDR_Equip_Sword, 0x00);
                            Connector.SendMessage("Locked Sword Removed. Reequip Sword");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot1, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true, "sword");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot2, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true, "sword");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot3, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true, "sword");
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.Write8(ADDR_SwordSlot4, 0xFF), /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(50), /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.1),
                        true, "sword");
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

                case "magicup": //Note fixed for UI update to be pushed soon
                    {
                        if (TryGiveMP(request))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} restored your magic to full.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "magicdown": //Note fixed for UI update to be pushed soon
                    {
                        if (TryTakeMP(request, 6))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} took some magic.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "healplayer": //Note fixed for UI update to be pushed soon
                    {
                        if (TryHealPlayerHealth(request))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} heal you to full.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "hurtplayer": //Note fixed for UI update to be pushed soon
                    {
                        if (TryHurtPlayerHealth(request, 4))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} hurt you.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "givemoney50": //Note fixed for UI update to be pushed soon
                    {
                        if (TryGiveMoney(request, 50))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} sent 50 dollars.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "givemoney100": //Note fixed for UI update to be pushed soon
                    {
                        if (TryGiveMoney(request, 100))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} sent 100 dollars.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "takemoney50": //Note fixed for UI update to be pushed soon
                    {
                        if (TryStealMoney(request, 50))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} stole 50 dollars.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "takemoney100": //Note fixed for UI update to be pushed soon
                    {
                        if (TryStealMoney(request, 100))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} stole 100 dollars.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                        }
                        return;
                    }

                case "levelup":   //Note fixed for UI update to be pushed soon
                    {
                        if (!Connector.Read8(ADDR_LEVEL, out byte playerlevel))
                        {
                            DelayEffect(request);
                        }

                        else if ((playerlevel) >= 0x10)
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x04);
                            Connector.SendMessage($"{request.DisplayViewer} sent a level.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                            Respond(request, EffectStatus.Success);
                        }
                        return;
                    }


                case "leveldown":  //Note fixed for UI update to be pushed soon
                    {
                        if (!Connector.Read8(ADDR_LEVEL, out byte playerlevel))
                        {
                            DelayEffect(request);
                        }
                        else if ((playerlevel) <= 0x01)
                        {
                            DelayEffect(request);
                        }

                        else
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x08);
                            Connector.SendMessage($"{request.DisplayViewer} removed a level.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                            Respond(request, EffectStatus.Success);
                        }
                        return;
                    }

                case "scaleup":  //Note bug in UI fix currenlty not working right
                    {
                        if (TryAlterScale(request, 1))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} increased the difficulty.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                            Respond(request, EffectStatus.Success);
                        }
                        return;
                    }

                case "scaledown":   //Note bug in UI fix currenlty not working right
                    {
                        if (TryAlterScale(request, -1))
                        {
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} decreased the difficulty.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                            Respond(request, EffectStatus.Success);
                        }
                        return;
                    }

                case "fixscale":   //Note fixed for UI update to be pushed soon
                    {
                        if (!Connector.Read8(ADDR_SCALING, out byte fixscale))
                        {
                            DelayEffect(request);
                        }

                        else if ((fixscale) <= 0x2F)
                        {
                            DelayEffect(request);
                        }
                        else
                        {
                            Connector.Write8(ADDR_SCALING, 0x2F); 
                            Connector.Write8(ADDR_U2HOOK, 0x01);
                            Connector.SendMessage($"{request.DisplayViewer} fixed scaling.");
                            Connector.Write8(ADDR_U1HOOK, 0x01);
                            Respond(request, EffectStatus.Success);
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
                        () => Connector.Read8(ADDR_INGAMEMENU, out byte gamemenu) && (gamemenu != 0x20) && (gamemenu != 0x10) && Connector.Read8(ADDR_MENU, out byte menu) && (menu != 0xFF) && Connector.IsZero8(ADDR_SHOP_ITEM1_PRICE1),            /*Refresh Condtion*/
                        TimeSpan.FromMilliseconds(500),                             /*Refresh Retry Timer*/
                        () => true, /*Action*/
                        TimeSpan.FromSeconds(0.5),
                        true);
                        shop.WhenStarted.Then(t => Connector.SendMessage($"{request.DisplayViewer} made item and sheild shops free (30s)."));
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
                        Connector.Unfreeze(ADDR_INVIS);
                        Connector.Write8(ADDR_INVIS, 0x00);
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
                        Connector.Unfreeze(ADDR_Blackout1);
                        Connector.Unfreeze(ADDR_Blackout2);
                        Connector.Unfreeze(ADDR_Blackout3);
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
