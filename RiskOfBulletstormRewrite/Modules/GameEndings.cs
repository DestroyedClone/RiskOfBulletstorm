using BepInEx;
using R2API;
using R2API.Utils;
using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Equipment.EliteEquipment;
using RiskOfBulletstormRewrite.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;
using RoR2;
using EntityStates;
using RoR2.Skills;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine.Networking;
using EntityStates.ScavMonster;
using EntityStates.Engi.EngiWeapon;
//using Aetherium.Utils;
using RoR2.CharacterAI;
using System.Net;
using BepInEx.Configuration;


namespace RiskOfBulletstormRewrite
{
    public static class GameEndings
    {
        public static GameEndingDef gedPastWon;
        public static GameEndingDef gedPastLose;
        public static void Init(ConfigFile config)
        {
            
        }

        public static void SetupGameEndings()
        {
            
        }  

        public enum ShowCredits
        {
            Default,
            Show,
            DontShow
        }

        public static GameEndingDef CreateGameEndingDef(
            string name,
            string endingTextToken,
            SerializableEntityStateType gameOverControllerState,
            bool isWin,
            Color backgroundColor,
            Color foregroundColor,
            Sprite icon,
            Material material,
            uint lunarCoinReward = 20U,
            ShowCredits showCredits = ShowCredits.Default,
            GameObject defaultKillerOverride = null
        )
        {
            GameEndingDef gameEndingDef = ScriptableObject.CreateInstance<GameEndingDef>();
            //gameEndingDef._cachedName;
            gameEndingDef.backgroundColor = backgroundColor;
            gameEndingDef.cachedName = name;
            gameEndingDef.defaultKillerOverride = defaultKillerOverride;
            gameEndingDef.endingTextToken = endingTextToken;
            gameEndingDef.foregroundColor = foregroundColor;
            //gameEndingDef.gameEndingIndex;
            gameEndingDef.gameOverControllerState = gameOverControllerState;
            gameEndingDef.icon = icon;
            gameEndingDef.isWin = isWin;
            gameEndingDef.lunarCoinReward = lunarCoinReward;
            gameEndingDef.material = material;
            (gameEndingDef as ScriptableObject).name = name;
            switch (showCredits)
            {
                case ShowCredits.Default:
                    gameEndingDef.showCredits = gameEndingDef.isWin;
                    break;
                case ShowCredits.Show:
                    gameEndingDef.showCredits = true;
                    break;
                case ShowCredits.DontShow:
                    gameEndingDef.showCredits = false;
                    break;
            }
            //gameEndingDef.showCredits;
            ContentAddition.AddGameEndingDef(gameEndingDef);
            return gameEndingDef;
        }
    }
}