using PathologicalGames;
using System;
using TheForest.Utils;
using UnityEngine;
using System.Text.RegularExpressions;
using TheForest.Items;
using TheForest.Items.Inventory;

namespace ForestUtility
{
    class Utilities : MonoBehaviour
    {
        private bool bESP = false;
        private bool bCan = false;
        private bool bAni = false;
        private bool bCav = false;
        private bool bCrea = false;
        private bool bGodMode = false;
        private bool bMenu = false;
        private bool bFog = false;
        private bool bJump = false;
        private bool bViewUnlocked = false;
        private bool bInfItems = false;
        private bool bIgnore = false;
        private bool bCrosshair = true;
        private float heightChange = 5f;
        private Vector2 crosshairTarget;
        Cheats.CheatsBridge cheats = new Cheats.CheatsBridge();
        FirstPersonCharacter fpC;
        Camera mainCam;

        private void godMode(bool yea)
        {
            if (yea)
            {
                LocalPlayer.Stats.Health = 99999f;
                LocalPlayer.Stats.Energy = 99999f;
                LocalPlayer.Stats.Stamina = 99999f;
                LocalPlayer.Stats.Fullness = 99999f;
                LocalPlayer.Stats.Thirst = -99999f;
                LocalPlayer.Stats.BodyTemp = 37f;
                cheats.SetInfiniteEnergy(yea);
                cheats.SetGodMode(yea);
                return;
            }
            cheats.SetInfiniteEnergy(yea);
            cheats.SetGodMode(yea);
        }

        private static void giveAllAvailableItems()
        {
            foreach (Item item in ItemDatabase.Items)
            {
                if (item._maxAmount >= 0 && !item.MatchType((Item.Types)1024) && LocalPlayer.Inventory.InventoryItemViewsCache.ContainsKey(item._id))
                {
                    LocalPlayer.Inventory.AddItem(item._id, 100000, true, false, null);
                }
                if (item._maxAmount >= 0 && item.MatchType((Item.Types)1024) && LocalPlayer.Inventory.InventoryItemViewsCache.ContainsKey(item._id))
                {
                    LocalPlayer.Inventory.AddItem(item._id, 100000, true, false, null);
                }
            }
        }

        private void ghostMode(bool yea)
        {
            if (yea)
            {
                LocalPlayer.GameObject.layer = 31;
                return;
            }
            LocalPlayer.GameObject.layer = 18;
        }

        private void creaMode(bool yea)
        {
            cheats.SetCreative(yea);
            return;
        }

        private void drawBoxESP(Vector3 pivotPos, Vector3 headPos, Color color)
        {
            float height = headPos.y - pivotPos.y;
            float widthOffset = 1.5f;
            float width = height / widthOffset;

            Render.DrawBox(pivotPos.x - (width / 2), (float)Screen.height - pivotPos.y - height, width, height, color, 2f);
            return;
        }

        private void changeJump(bool yea, float height)
        {
            if (yea)
            {
                fpC.jumpHeight = height;
                LocalPlayer.CamFollowHead.stopAllCameraShake();
                return;
            }
            fpC.jumpHeight = 5f;
        }

        private static void infItems(bool yea)
        {
            if (yea)
            {
                LocalPlayer.Inventory.ItemFilter = new InventoryItemFilter_Unlimited();
                return;
            }
            LocalPlayer.Inventory.ItemFilter = null;
        }

        public void Awake()
        {
            try { mainCam = Camera.main; } catch { }
            try { fpC = FindObjectOfType<FirstPersonCharacter>(); } catch { }
            crosshairTarget = new Vector2(Screen.width / 2, Screen.height / 2); //Calculate a point facing straight away from us
        }

        public void Start()
        {
            
        }

        private void drawCrosshair(bool yea)
        {
            if (yea)
            {
                Render.DrawLine(new Vector2(crosshairTarget.x - 7, crosshairTarget.y), new Vector2(crosshairTarget.x + 5, crosshairTarget.y), Color.red, 2f); //Draw
                Render.DrawLine(new Vector2(crosshairTarget.x, crosshairTarget.y - 6), new Vector2(crosshairTarget.x, crosshairTarget.y + 6), Color.red, 2f); //Draw
                return;
            }
        }

        public void Update()
        {
            changeJump(bJump, heightChange);
            godMode(bGodMode);
            creaMode(bCrea);
            infItems(bInfItems);
            ghostMode(bIgnore);
            NoFog(bFog);

            if (!UnityEngine.Input.anyKey || !UnityEngine.Input.anyKeyDown)
            {
                return;
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
            {
                bMenu = !bMenu;
            }

            if (bMenu)
            {
                LocalPlayer.FpCharacter.LockView(true);
                LocalPlayer.Inventory.CurrentView = 0;
                bViewUnlocked = false;
            } 
            else
            {
                if (!bViewUnlocked)
                {
                    LocalPlayer.FpCharacter.UnLockView();
                    LocalPlayer.Inventory.CurrentView = (TheForest.Items.Inventory.PlayerInventory.PlayerViews)2;
                    bViewUnlocked = true;
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Delete))
                Loader.Unload();
        }

        private void boxESP(Transform mutant, Color color, bool bStringESP = false)
        {
            CharacterController characterController = new CharacterController();

            Vector3 pivotPos = mutant.transform.position;
            Vector3 mutantHeadPos = pivotPos;

            pivotPos.y += ((characterController == null) ? 6.75f : characterController.height);
            Vector3 w2s_headpos = mainCam.WorldToScreenPoint(mutantHeadPos);
            Vector3 w2s_pivotpos = mainCam.WorldToScreenPoint(pivotPos);

            float distance = (float)Math.Round((double)Vector3.Distance(LocalPlayer.Transform.position, mutant.transform.position), 1);

            if (w2s_pivotpos.z > 0f && distance < 250f)
            {
                drawBoxESP(w2s_pivotpos, w2s_headpos, Color.red);
                Render.DrawLine(new Vector2(((float)Screen.width / 2), (float)(Screen.height)), new Vector2(w2s_pivotpos.x, (float)Screen.height - w2s_headpos.y), color, 2f);
                if (bStringESP)
                {
                    Render.DrawString(new Vector2(w2s_headpos.x, (float)Screen.height - w2s_headpos.y), UppercaseFirst(getEntName(mutant.name)) + " [" + distance.ToString() + "m]", color);
                }
            }
        }
        private static void NoFog(bool yea)
        {
            if (yea)
            {
                Scene.Atmosphere.FogStartDistance = 999f;
                return;
            }
        }

        private static string getEntName(string text)
        {
            text = text.Replace("Go", "").Replace("(Clone)", "").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("lb_", "").Replace("_small", " Small").Replace("_medium", " Medium").Replace("_large", " Large").Replace("_CaveB", " (B)").Replace("_CaveA", " (A)").Replace("_dummy", "").Replace("_base", "").Replace("_net", "").Replace("_fat", " Fat").Replace("_baby", " Baby").Replace("_creepy", " (Strong)").Replace("blueBird", "Blue Bird").Replace("mutant_male", "Male Cannibal").Replace("mutant_female", "Female Cannibal").Replace("CaveHole", "Cave").Replace("()", "").Replace("caveNarrowEntrance", "Cave Gap").Replace("redBird", "Red Bird");
            return new Regex("<[^>]+>").Replace(text, string.Empty);
        }

        private void stringESP(Transform mutant, Color color)
        {
            Vector3 pivotPos = mutant.transform.position;
            Vector3 mutantPos; mutantPos.x = pivotPos.x; mutantPos.z = pivotPos.z; mutantPos.y = pivotPos.y;
            Vector3 w2s_footpos = mainCam.WorldToScreenPoint(mutantPos);
            float distance = (float)Math.Round((double)Vector3.Distance(LocalPlayer.Transform.position, mutant.transform.position), 1);

            if (w2s_footpos.z > 0f && distance < 250f)
            {
                Render.DrawString(new Vector2(w2s_footpos.x, (float)Screen.height - w2s_footpos.y), UppercaseFirst(getEntName(mutant.name)) + " [" + distance.ToString() + "m]", color);
            }

            return;
        }

        private void stringESP(caveEntranceManager mutant, Color color)
        {
            Vector3 pivotPos = mutant.transform.position;
            Vector3 mutantPos; mutantPos.x = pivotPos.x; mutantPos.z = pivotPos.z; mutantPos.y = pivotPos.y;
            Vector3 w2s_footpos = mainCam.WorldToScreenPoint(mutantPos);
            float distance = (float)Math.Round((double)Vector3.Distance(LocalPlayer.Transform.position, mutant.transform.position), 1);


            if (w2s_footpos.z > 0f && distance < 250f)
            {
                Render.DrawString(new Vector2(w2s_footpos.x, (float)Screen.height - w2s_footpos.y), UppercaseFirst(getEntName(mutant.name)) + " [" + distance.ToString() + "m]", color);
            }

            return;
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]).ToString() + s.Substring(1);
        }

        public void OnGUI()
        {
            GUI.color = Color.cyan;
            if (bMenu && bJump) { GUI.Box(new Rect(5, 5, 210, 250), ""); } else if (bMenu && bESP) { GUI.Box(new Rect(5, 5, 180, 250), ""); } else if (bMenu) { GUI.Box(new Rect(5, 5, 155, 250), ""); } else { GUI.Box(new Rect(5, 5, 155, 35), ""); }
            GUI.Label(new Rect(10, 10, 150, 30), "zain#7777 | ForestUtility");
            if (bMenu)
            {
                bGodMode = GUI.Toggle(new Rect(10, 25, 150, 20), bGodMode, "GodMode");
                bESP = GUI.Toggle(new Rect(10, 40, 100, 20), bESP, "ESP");
                if (bESP)
                {
                    bCan = GUI.Toggle(new Rect(110, 40, 100, 20), bCan, "Cannibal");
                    bAni = GUI.Toggle(new Rect(110, 55, 100, 20), bAni, "Animal");
                    bCav = GUI.Toggle(new Rect(110, 70, 100, 20), bCav, "Cave");
                }
                bFog = GUI.Toggle(new Rect(10, 55, 100, 20), bFog, "No Fog");
                bCrea = GUI.Toggle(new Rect(10, 70, 100, 20), bCrea, "Creative");
                bJump = GUI.Toggle(new Rect(10, 85, 100, 20), bJump, "High Jump");
                if (bJump)
                {
                    heightChange = GUI.HorizontalSlider(new Rect(110, 90, 100, 20), heightChange, 0f, 200f);
                }
                bInfItems = GUI.Toggle(new Rect(10, 100, 100, 20), bInfItems, "Infinite Items");
                bIgnore = GUI.Toggle(new Rect(10, 115, 100, 20), bIgnore, "Ghost Mode");
                bCrosshair = GUI.Toggle(new Rect(10, 130, 100, 20), bCrosshair, "Crosshair");
                if (GUI.Button(new Rect(10, 150, 100, 20), "Sleep")) { LocalPlayer.Stats.GoToSleep(); }
                if (GUI.Button(new Rect(10, 170, 100, 20), "All Items")) { giveAllAvailableItems(); }
                if (GUI.Button(new Rect(10, 190, 100, 20), "Reload")) 
                { 
                    mainCam = Camera.main;
                    fpC = FindObjectOfType<FirstPersonCharacter>();
                    crosshairTarget = new Vector2(Screen.width / 2, Screen.height / 2);
                }
            }

            drawCrosshair(bCrosshair);

            if (bESP)
            {
                if (bCan)
                    for (int i = 0; i < PoolManager.Pools["enemies"].Count; i++) { boxESP(PoolManager.Pools["enemies"][i], Color.red, true); }
                if (bAni)
                    for (int i = 0; i < PoolManager.Pools["creatures"].Count; i++) { stringESP(PoolManager.Pools["creatures"][i], Color.yellow); }
                if (bCav)
                    for (int i = 0; i < Scene.SceneTracker.caveEntrances.Count; i++) { stringESP(Scene.SceneTracker.caveEntrances[i], Color.cyan); }
                return;
            }
        }
    }
}
