using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyPinyin;

namespace PinYinSearch_ONI_MOD
{
    public class tester
    {
        static void Main(string[] args)
        {
            //科雷用了ToLower()，所以我也用了
            //不过获取到的是中文，应该没有区别
            string inputString = "POINT";
            string buildingName = "hauling point".ToLower();
            String buildingPinYin = PinyinHelper.GetPinyin(buildingName);
            String buildingPinYinInit = null;

            if (buildingPinYin.Length > buildingName.Length*2)
            {
                //获取首字母作为快捷搜索
                String[] buildingPinYinTemp = buildingPinYin.Split(' ');
                StringBuilder buildingPinYinInitTemp = new StringBuilder();
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
                buildingPinYin = buildingName;
                buildingPinYinInit = buildingName;
            }

            Console.WriteLine(buildingPinYin.Contains(inputString) || buildingPinYinInit.Contains(inputString));
            Console.ReadKey();
        }
        /*
         * 这是给缺氧调试时候用debug
        DebugUtil.LogWarningArgs(new object[]
            {
                "给出的名字",
                buildingName
        });
        */
    }
}
