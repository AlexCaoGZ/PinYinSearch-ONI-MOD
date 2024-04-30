using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyPinyin;

namespace PinYinSearch_ONI_MOD
{
    [HarmonyPatch(typeof(SingleItemSelectionSideScreenBase), "TagContainsSearchWord")]
    internal class ItemSearch
    {
        public static void Postfix(ref bool __result, ref Tag tag, ref string search)
        {
            //原本的代码：
            //return string.IsNullOrEmpty(search) || tag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());

            DebugUtil.LogWarningArgs(new object[]
            {
                "搜索的东西",
                search
            });
            //__result = buildingPinYin.Contains(inputString) || buildingPinYinInit.Contains(inputString) || buildingName.Contains(inputString) || (subcategoryName != null && subcategoryName.ToUpper().Contains(inputString));
        }
    }
}
