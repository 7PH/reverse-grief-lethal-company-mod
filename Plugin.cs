using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCModReverseGrief.Patches;

namespace LCModReverseGrief
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ModeBase : BaseUnityPlugin
    {
        private const string modGUID = "7ph.dev.lcmodreversegrief";
        private const string modName = "LC Mod - Reverse Grief";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ModeBase Instance;

        internal ManualLogSource mls;

        public ManualLogSource Mls
        {
            get { return mls; }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            // Initialize logging
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo(modName + " initialized");

            // Apply patches
            harmony.PatchAll(typeof(ModeBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}