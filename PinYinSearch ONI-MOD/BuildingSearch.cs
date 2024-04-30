using System.Linq;
using HarmonyLib;
using TinyPinyin;
using System;
using System.Text;
using STRINGS;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingSearch
{
    [HarmonyPatch(typeof(PlanBuildingToggle), "CheckBuildingPassesSearchFilter")]
    internal class BuildingSearch
    {
        public static void Postfix(ref bool __result, ref Def building)
        {
            //科雷用了ToLower()，所以我也用了
            //不过获取到的是中文，应该没有区别
            string buildingName = UI.StripLinkFormatting(building.Name).ToLower();
            string inputString = BuildingGroupScreen.Instance.inputField.text.ToUpper();
            String buildingPinYin = PinyinHelper.GetPinyin(buildingName);
            String buildingPinYinInit = null;

            //当拼音后的长度超原本长度两倍以上的时候将其认为是非中文
            //StringBuilder有默认长度，避免来一个超长的mod建筑名字塞爆StringBuilder
            if (buildingPinYin.Length > buildingName.Length * 2)
            {
                //获取首字母作为快捷搜索
                String[] buildingPinYinTemp = buildingPinYin.Split(' ');
                StringBuilder buildingPinYinInitTemp = new StringBuilder(64);
                foreach (string str in buildingPinYinTemp)
                {
                    buildingPinYinInitTemp.Append(str[0]);
                }
                buildingPinYinInit = buildingPinYinInitTemp.ToString();

                //去除拼音中的空格分隔，获取搜索输入
                buildingPinYin = buildingPinYin.Replace(" ", "").ToUpper();
            }
            else
            {
                //英文情况，大小写各检查一次
                buildingPinYin = buildingName.ToUpper();
                buildingPinYinInit = buildingName.ToLower();
            }

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

            __result = buildingPinYin.Contains(inputString) || buildingPinYinInit.Contains(inputString) || buildingName.Contains(inputString) || (subcategoryName != null && subcategoryName.ToUpper().Contains(inputString));
        }
    }

    [HarmonyPatch(typeof(TreeFilterableSideScreenRow), "FilterAgainstSearch")]
    internal class ItemSearch
    {
        //获取TreeFilterableSideScreenRow class里面的SetArrowToggleState()
        static MethodInfo mSetArrowToggleState = typeof(TreeFilterableSideScreenRow).GetMethod("SetArrowToggleState");
        static FastInvokeHandler SetArrowToggleState = MethodInvoker.GetHandler(mSetArrowToggleState, true);
        public static void Postfix(ref Tag thisCategoryTag, ref string search)
        {
            //获取rowElements和arrowToggle
            List< TreeFilterableSideScreenElement > rowElements = Traverse.Create(typeof(TreeFilterableSideScreenRow)).Field("rowElements").GetValue<List<TreeFilterableSideScreenElement>>();
            MultiToggle arrowToggle = Traverse.Create(typeof(TreeFilterableSideScreenRow)).Field("arrowToggle").GetValue<MultiToggle>();

            bool flag = false;
            bool flag2 = thisCategoryTag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());
            search = search.ToUpper();
            foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in rowElements)
            {
                bool flag3 = flag2 || treeFilterableSideScreenElement.GetElementTag().ProperNameStripLink().ToUpper().Contains(search.ToUpper());
                treeFilterableSideScreenElement.gameObject.SetActive(flag3);
                flag = (flag || flag3);
            }
            //base.gameObject.SetActive(flag);
            if (search != "" && flag && arrowToggle.CurrentState == 0)
            {
                SetArrowToggleState(true);
            }
        }
    }
}