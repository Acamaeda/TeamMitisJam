using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace JamminPlaceholder
{
    public class JamminPlaceholder : ModBehaviour
    {
        public static JamminPlaceholder Instance;
        public INewHorizons NewHorizons;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(JamminPlaceholder)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("TeamMitis.JamminPlaceholder").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons").GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);

        }

        public void OnStarSystemLoaded(string system)
        {
            if (system == "SolarSystem")
            {

                var planet = GameObject.Find("MyPlanet_Body");
                var moon = GameObject.Find("NutritionSource_Body");
                var alignment = planet.AddComponent<AlignWithTargetBody>();
                alignment.SetTargetBody(moon.GetAttachedOWRigidbody());
                alignment.SetUsePhysicsToRotate(true);
                ModHelper.Console.WriteLine("Alignment set up!", MessageType.Success);
            }
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }
    }


    [HarmonyPatch]
    public class ShenanigansPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Campfire), nameof(Campfire.StartRoasting))]

        public static bool Campfire_StartRoasting_Prefix()
        {
            JamminPlaceholder.Instance.ModHelper.Console.WriteLine("Beep!");
            TextTranslation.Get().SetLanguage(TextTranslation.Language.UNKNOWN);
            return true;
        }
    }

}
