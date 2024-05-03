using PinYinSearch_ONI_MOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string filter = "nitu";
            string name1 = "<link=\"FILTER\">过滤介质</link>";

            //科雷在tag.ProperName()的返回值部分有<link>标签，需要移除标签
            Match m = Regex.Match(name1, "\\>(.*?)\\<", RegexOptions.IgnoreCase);
            string textTemp = "";
            if (m.Success)
            {
                textTemp = m.Groups[1].Value;
            }
            else
            {
                textTemp = name1;
            }

            //获取拼音
            textTemp = textTemp + "|" + pinYinDict.getPinYin(textTemp);

            //tag.Name返回的是元素的英文名，会弄乱搜索结果
            Console.WriteLine(!(filter != "") || textTemp.Contains(filter));// || tag.Name.ToLower().Contains(filter);
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
