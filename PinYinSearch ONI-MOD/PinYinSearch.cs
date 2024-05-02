using HarmonyLib;
using TinyPinyin;
using System;
using System.Text;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using PinYinSearch_ONI_MOD;

namespace PinYinSearch
{

    [HarmonyPatch(typeof(PlanBuildingToggle), "CheckBuildingPassesSearchFilter")]
    internal class BuildingSearch
    {
        public static bool Prefix(ref bool __result, ref Def building)
        {
            //科雷用了ToLower()，所以我也用了
            //不过获取到的是中文，应该没有区别
            string buildingName = UI.StripLinkFormatting(building.Name).ToLower();
            string inputString = BuildingGroupScreen.Instance.inputField.text.ToLower();

            //获取拼音
            String buildingPinYin = pinYinDict.getPinYin(buildingName);

            //subcategoryName不知道是什么，可能是搜索类别用的？
            //用Traverse获取了它本来的数值，应该可能大概没破坏本身的功能
            string subcategoryName = null;
            try
            {
                subcategoryName = Traverse.Create(typeof(PlanBuildingToggle)).Field("subcategoryName").ToString();
            }
            catch (Exception ex) {
                DebugUtil.LogWarningArgs(new object[]
                {
                "拼音搜索 subcategoryName报错",
                ex
            });
            }
            //          检查拼音 + 英文                          检查中文
            __result = buildingPinYin.Contains(inputString) || buildingName.Contains(inputString) || (subcategoryName != null && subcategoryName.ToUpper().Contains(inputString));
            return false;
        }
    }

    [HarmonyPatch(typeof(TreeFilterableSideScreenRow), "FilterAgainstSearch")]
    internal class ItemSearch
    {
        private Dictionary<string, string[]> _elementPinYinDict = new Dictionary<string, string[]>();

        public static bool Prefix(TreeFilterableSideScreenRow __instance, ref Tag thisCategoryTag, ref string search)
        {

            //获取两个private的变量
            List<TreeFilterableSideScreenElement> rowElements = Traverse.Create(__instance).Field("rowElements").GetValue<List<TreeFilterableSideScreenElement>>();
            MultiToggle arrowToggle = Traverse.Create(__instance).Field("arrowToggle").GetValue<MultiToggle>();
            
            //搜索类别
            bool flag = false;
            bool flag2 = thisCategoryTag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());

            //搜索元素
            search = search.ToUpper();
            foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in rowElements)
            {
                //     在搜索类别里  || 配对元素名称
                bool flag3 = flag2 || pinYinDict.getPinYin(treeFilterableSideScreenElement.GetElementTag().ProperNameStripLink()).Contains(search.ToLower());
                // 让这一个元素出现在行（Row）里
                treeFilterableSideScreenElement.gameObject.SetActive(flag3);
                flag = (flag || flag3);
            }
            // 让这一行（Row）出现在菜单里
            __instance.gameObject.SetActive(flag);
            if (search != "" && flag && arrowToggle.CurrentState == 0)
            {
                //展开这一行的所有内容
                __instance.SetArrowToggleState(true);
            }
            return false;
        }

    }


}