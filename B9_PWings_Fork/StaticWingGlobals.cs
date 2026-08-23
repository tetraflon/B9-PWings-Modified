using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WingProcedural
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class StaticWingGlobals : MonoBehaviour
    {
        public static List<WingTankConfiguration> wingTankConfigurations = new List<WingTankConfiguration>();

        public static Shader wingShader;

        public static GameObject handlesRoot, normalHandles, ctrlSurfHandles, hingeIndicator,
                                 handleLength, handleWidthRootFront, handleWidthRootBack, handleWidthTipFront, handleWidthTipBack,
                                 handleLeadingRoot, handleLeadingTip, handleTrailingRoot, handleTrailingTip;
        public static GameObject ctrlHandleLength1, ctrlHandleLength2,
                                 ctrlHandleRootWidthOffset, ctrlHandleTipWidthOffset,
                                 ctrlHandleTrailingRoot, ctrlHandleTrailingTip;

        private static string _bundlePath;

        public static bool loadingAssets = false;
        public static StaticWingGlobals Instance;

        private void Awake()
        {
            _bundlePath = KSPUtil.ApplicationRootPath + "GameData"
                + Path.DirectorySeparatorChar + "B9_Aerospace_ProceduralWings"
                + Path.DirectorySeparatorChar + "AssetBundles"
                + Path.DirectorySeparatorChar;

            if (Instance != null) Destroy(Instance);
            Instance = this;
        }

        // Load the platform-specific bundle so the previous (default) wing shader is used.
        public string BundlePath
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXPlayer: return _bundlePath + "pwings_macosx.bundle";
                    case RuntimePlatform.WindowsPlayer: return _bundlePath + "pwings_windows.bundle";
                    case RuntimePlatform.LinuxPlayer: return _bundlePath + "pwings_linux.bundle";
                    default: return _bundlePath + "pwings_windows.bundle";
                }
            }
        }

        public void Start()
        {
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("ProceduralWingFuelSetups"))
            {
                ConfigNode[] fuelNodes = node.GetNodes("FuelSet");
                for (int i = 0; i < fuelNodes.Length; ++i)
                {
                    wingTankConfigurations.Add(new WingTankConfiguration(fuelNodes[i]));
                }
            }

            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("ProceduralWingDefaults"))
            {
                var defaults = new WingDefaults(node);
                defaults.Apply();
            }

            Debug.Log("[B9PW] start bundle load process");

            StartCoroutine(LoadBundleAssets());
        }

        public IEnumerator LoadBundleAssets()
        {
            Debug.Log("[B9PW] Aquiring bundle data");

            // 1) Dedicated shader bundle: may carry an updated compositing shader.
            //    If it loads and contains our shader, prefer it; otherwise fall back to the legacy bundle's shader.
            string shaderBundlePath = BundlePath.Replace("pwings_windows.bundle", "pwings_shader_windows.bundle")
                                                .Replace("pwings_linux.bundle", "pwings_shader_linux.bundle")
                                                .Replace("pwings_macosx.bundle", "pwings_shader_macosx.bundle");
            AssetBundle shaderBundle = AssetBundle.LoadFromFile(shaderBundlePath);

            bool shaderFromDedicated = false;
            if (shaderBundle != null)
            {
                Shader[] objects = shaderBundle.LoadAllAssets<Shader>();
                for (int i = 0; i < objects.Length; ++i)
                {
                    if (objects[i].name == "KSP/Specular Layered")
                    {
                        wingShader = objects[i];
                        shaderFromDedicated = true;
                        Debug.Log($"[B9PW] Wing shader \"{wingShader.name}\" loaded (shader bundle). Supported? {wingShader.isSupported}");
                    }
                }
            }
            else
            {
                Debug.Log("[B9PW] No dedicated shader bundle found, will load shader from legacy bundle");
            }

            // 2) Legacy bundle: ALWAYS load it - it carries the editor handle gizmos (handlesRoot),
            //    which are required by UpdateHandleGizmos. Its shader is only used as a fallback.
            AssetBundle legacyBundle = AssetBundle.LoadFromFile(BundlePath);

            if (legacyBundle != null)
            {
                if (!shaderFromDedicated)
                {
                    Shader[] objects = legacyBundle.LoadAllAssets<Shader>();
                    for (int i = 0; i < objects.Length; ++i)
                    {
                        if (objects[i].name == "KSP/Specular Layered")
                        {
                            wingShader = objects[i];
                            Debug.Log($"[B9PW] Wing shader \"{wingShader.name}\" loaded (legacy bundle). Supported? {wingShader.isSupported}");
                        }
                    }
                }

                #region Handle gizmos---CarnationRED 2020-6
                GameObject[] objects1 = legacyBundle.LoadAllAssets<GameObject>();
                for (int i = 0; i < objects1.Length; i++)
                {
                    GameObject item = objects1[i];
                    if (item.name.Equals("handlesRoot"))
                    {
                        handlesRoot = Instantiate(item);
                        break;
                    }
                }
                if (handlesRoot)
                {
                    handlesRoot.transform.SetParent(null, false);
                    handlesRoot.SetActive(false);
                    DontDestroyOnLoad(handlesRoot);

                    normalHandles = handlesRoot.transform.Find("Normal").gameObject;
                    ctrlSurfHandles = handlesRoot.transform.Find("CtrlSurf").gameObject;
                    hingeIndicator = handlesRoot.transform.Find("RotateAxis").gameObject;
                    foreach (Transform obj in normalHandles.transform)
                        obj.gameObject.AddComponent<EditorHandle>();

                    foreach (Transform obj in ctrlSurfHandles.transform)
                        obj.gameObject.AddComponent<EditorHandle>();

                    handleLength = normalHandles.transform.Find("handleLength").gameObject;
                    handleWidthRootFront = normalHandles.transform.Find("handleWidthRootFront").gameObject;
                    handleWidthRootBack = normalHandles.transform.Find("handleWidthRootBack").gameObject;
                    handleWidthTipFront = normalHandles.transform.Find("handleWidthTipFront").gameObject;
                    handleWidthTipBack = normalHandles.transform.Find("handleWidthTipBack").gameObject;
                    handleLeadingRoot = normalHandles.transform.Find("handleLeadingRoot").gameObject;
                    handleLeadingTip = normalHandles.transform.Find("handleLeadingTip").gameObject;
                    handleTrailingRoot = normalHandles.transform.Find("handleTrailingRoot").gameObject;
                    handleTrailingTip = normalHandles.transform.Find("handleTrailingTip").gameObject;

                    ctrlHandleLength1 = ctrlSurfHandles.transform.Find("ctrlHandleLength1").gameObject;
                    ctrlHandleLength2 = ctrlSurfHandles.transform.Find("ctrlHandleLength2").gameObject;
                    ctrlHandleRootWidthOffset = ctrlSurfHandles.transform.Find("ctrlHandleRootWidthOffset").gameObject;
                    ctrlHandleTipWidthOffset = ctrlSurfHandles.transform.Find("ctrlHandleTipWidthOffset").gameObject;
                    ctrlHandleTrailingRoot = ctrlSurfHandles.transform.Find("ctrlHandleTrailingRoot").gameObject;
                    ctrlHandleTrailingTip = ctrlSurfHandles.transform.Find("ctrlHandleTrailingTip").gameObject;
                }
                #endregion

                yield return null;
                yield return null; // unknown how neccesary this is

                Debug.Log("[B9PW] unloading legacy bundle");
                legacyBundle.Unload(false); // unload the raw asset bundle
            }
            else
            {
                Debug.Log("[B9PW] Error: Found no asset bundle to load");
            }

            if (shaderBundle != null)
            {
                yield return null;
                yield return null;
                Debug.Log("[B9PW] unloading shader bundle");
                shaderBundle.Unload(false);
            }
        }

        /// <summary>
        /// Check that the handles are on the correct layer and correct them if necessary.
        /// Sometimes KSP changes the layer that the handles are on (picking up a part through a handle triggers this).
        /// </summary>
        /// <returns>true if the layers needed fixing.</returns>
        public static bool CheckHandleLayers()
        {
            bool changed = false;
            if (handlesRoot && handlesRoot.layer != 2)
            {
                Debug.Log($"[B9PW] handlesRoot was on layer {handlesRoot.layer}, resetting to 2.");
                handlesRoot.SetLayerRecursive(2); // Recursively set all the layers back to 2.
                changed = true;
            }
            return changed;
        }
    }
}