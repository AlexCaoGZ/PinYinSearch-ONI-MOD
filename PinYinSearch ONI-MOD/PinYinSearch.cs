using HarmonyLib;
using PinYinSearch_ONI_MOD;
using ProcGen.Noise;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PinYinSearch
{

    //建筑栏搜索功能
    [HarmonyPatch(typeof(SearchUtil), "MakeBuildingDefCache")]
    internal class BuildingSearch
    {
        public static bool Prefix(SearchUtil.BuildingDefCache __instance, ref SearchUtil.BuildingDefCache __result, ref BuildingDef def)
        {
            SearchUtil.NameDescSearchTermsCache nameDescSearchTerms = new SearchUtil.NameDescSearchTermsCache
            {
                nameDesc = new SearchUtil.NameDescCache
                {
                    name = new SearchUtil.MatchCache
                    {
                        //这里是唯一变动的地方，其他都是科雷原版代码
                        text = SearchUtil.Canonicalize(def.Name + pinYinDict.getPinYin(def.Name))
                    },
                    desc = new SearchUtil.MatchCache
                    {
                        text = SearchUtil.CanonicalizePhrase(def.Desc)
                    }
                },
                searchTerms = def.SearchTerms
            };
            SearchUtil.MatchCache effect = new SearchUtil.MatchCache
            {
                text = SearchUtil.CanonicalizePhrase(def.Effect)
            };
            List<SearchUtil.NameDescCache> list = new List<SearchUtil.NameDescCache>();
            ListPool<ComplexRecipe, PlanBuildingToggle>.PooledList pooledList = ListPool<ComplexRecipe, PlanBuildingToggle>.Allocate();
            BuildingDef.CollectFabricationRecipes(def.PrefabID, pooledList);
            foreach (ComplexRecipe complexRecipe in pooledList)
            {
                list.Add(new SearchUtil.NameDescCache
                {
                    name = new SearchUtil.MatchCache
                    {
                        text = SearchUtil.Canonicalize(complexRecipe.GetUIName(false))
                    },
                    desc = new SearchUtil.MatchCache
                    {
                        text = SearchUtil.CanonicalizePhrase(complexRecipe.description)
                    }
                });
            }
            pooledList.Recycle();
            __result = new SearchUtil.BuildingDefCache
            {
                nameDescSearchTerms = nameDescSearchTerms,
                effect = effect,
                recipes = list
            };

            return false;
        }
    }

    //修改搜索评分threshold
    [HarmonyPatch(typeof(SearchUtil.BuildingDefCache), "IsPassingScore")]
    internal class changePassingScore
    {
        public static bool Prefix(SearchUtil.BuildingDefCache __instance, ref bool __result)
        {
            //科雷学之magic number
            __result = __instance.Score >= 67;
            return false;
        }
    }

    //多物体筛选菜单（例如move it的筛选栏）
    [HarmonyPatch(typeof(TreeFilterableSideScreenRow), "FilterAgainstSearch")]
    internal class ItemSearch
    {

        public static bool Prefix(TreeFilterableSideScreenRow __instance, ref Tag thisCategoryTag, ref string search)
        {

            //获取两个private的变量
            List<TreeFilterableSideScreenElement> rowElements = Traverse.Create(__instance).Field("rowElements").GetValue<List<TreeFilterableSideScreenElement>>();
            MultiToggle arrowToggle = Traverse.Create(__instance).Field("arrowToggle").GetValue<MultiToggle>();

            //这科雷搜建筑用tolower()，搜元素用toupper()，不怕哪天儿给自己整个活儿吗？
            //搜索类别
            bool flag = false;
            bool flag2 = thisCategoryTag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());

            //搜索元素
            search = search.ToUpper();
            foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in rowElements)
            {
                string rowName = treeFilterableSideScreenElement.GetElementTag().ProperNameStripLink();
                rowName = rowName + "|" + pinYinDict.getPinYin(rowName);
                //     在搜索类别里  || 配对元素名称
                bool flag3 = flag2 || rowName.Contains(search.ToLower());
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

    //星球总资源搜索栏
    [HarmonyPatch(typeof(AllResourcesScreen), "PassesSearchFilter")]
    internal class globalSearch
    {

        public static bool Prefix(ref bool __result, ref Tag tag, ref string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                __result = false;
            }
            else
            {
                //科雷这里用的是ToUpper()
                //但我上面用的全部都是ToLower()
                //还是和我自己保持一致的好
                filter = filter.ToLower();

                //科雷在tag.ProperName()的返回值部分有<link>标签，需要移除标签
                Match m = Regex.Match(tag.ProperName(), "\\>(.*?)\\<", RegexOptions.IgnoreCase);
                string textTemp = "";
                if (m.Success)
                {
                    textTemp = m.Groups[1].Value;
                }
                else
                {
                    textTemp = tag.ProperName();
                }

                //获取拼音
                string text = textTemp + "|" + pinYinDict.getPinYin(textTemp);

                //tag.Name返回的是元素的英文名，会弄乱搜索结果
                __result = text.Contains(filter);// || tag.Name.ToLower().Contains(filter);
            }

            return false;
        }
    }

    //筛选器搜索栏
    [HarmonyPatch(typeof(SingleItemSelectionSideScreenBase), "TagContainsSearchWord")]
    internal class FilterSearch
    {

        public static bool Prefix(SingleItemSelectionSideScreenBase __instance, ref bool __result, ref Tag tag, ref string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                __result = false;
            }
            else
            {
                //科雷这里用的是ToUpper()
                //但我上面用的全部都是ToLower()
                //还是和我自己保持一致的好
                search = search.ToLower();
                //科雷在tag.ProperName()的返回值部分有<link>标签，需要移除标签
                string textTemp = tag.ProperNameStripLink();
                //获取拼音
                string text = textTemp + "|" + pinYinDict.getPinYin(textTemp);
                __result = text.Contains(search);
            }
            return false;
        }
    }

}
