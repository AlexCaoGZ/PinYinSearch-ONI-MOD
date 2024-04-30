using System.Linq;
using HarmonyLib;
using TinyPinyin;
using System;
using System.Text;
using STRINGS;

namespace PinYinSearch_ONI_MOD
{
    [HarmonyPatch(typeof(PlanBuildingToggle), "CheckBuildingPassesSearchFilter")]
    internal class PinYinSearch
    {
        public static void Postfix(ref bool __result, ref Def building)
        {
            //科雷用了ToLower()，所以我也用了
            //不过获取到的是中文，应该没有区别
            string buildingName = UI.StripLinkFormatting(building.Name).ToLower();
            String buildingPinYin = PinyinHelper.GetPinyin(buildingName);

            //获取首字母作为快捷搜索
            String[] buildingPinYinTemp = buildingPinYin.Split(' ');
            StringBuilder buildingPinYinInitTemp = new StringBuilder();
            foreach (string str in buildingPinYinTemp)
            {
                buildingPinYinInitTemp.Append(str[0]);
            }
            String buildingPinYinInit = buildingPinYinInitTemp.ToString();

            //去除拼音中的空格分隔，获取搜索输入
            buildingPinYin = buildingPinYin.Replace(" ", "").ToUpper();
            string inputString = BuildingGroupScreen.Instance.inputField.text.ToUpper();

            //subcategoryName不知道是什么，可能是给mod添加的自定义建筑用的？
            //用Traverse获取了它本来的数值，应该可能大概没破坏本身的功能
            string subcategoryName = null;
            try
            {
                subcategoryName = Traverse.Create(typeof(PlanBuildingToggle)).Field("subcategoryName").ToString();
            }
            catch (Exception ex) { }

            __result = (buildingPinYin.Contains(inputString) || buildingPinYinInit.Contains(inputString)) || (subcategoryName != null && subcategoryName.ToString().ToUpper().Contains(inputString));
        }
    }
}