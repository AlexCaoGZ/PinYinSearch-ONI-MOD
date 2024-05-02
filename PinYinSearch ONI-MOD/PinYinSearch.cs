﻿using HarmonyLib;
using TinyPinyin;
using System;
using System.Text;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using PinYinSearch_ONI_MOD;
using System.Runtime.CompilerServices;
using static STRINGS.WORLD_TRAITS;

namespace PinYinSearch
{

    [HarmonyPatch(typeof(PlanBuildingToggle), "CheckBuildingPassesSearchFilter")]
    internal class BuildingSearch
    {
        private Dictionary<string, string[]> _buildingPinYinDict = new Dictionary<string, string[]>();
        public static void Postfix(ref bool __result, ref Def building)
        {
            //初始化字典
            var bs = new BuildingSearch();
            if (bs._buildingPinYinDict.Count == 0)
            {
                bs.creatbuildingPinYinDict(bs._buildingPinYinDict);
                DebugUtil.LogWarningArgs(new object[]
                {
                "创建了新字典",
                bs._buildingPinYinDict.Count
                });
            }
            PinYinSearch_ONI_MOD.pinYinDict = bs._buildingPinYinDict;

            //科雷用了ToLower()，所以我也用了
            //不过获取到的是中文，应该没有区别
            string buildingName = UI.StripLinkFormatting(building.Name).ToLower();
            string inputString = BuildingGroupScreen.Instance.inputField.text.ToLower();
            String buildingPinYin = null;
            String buildingPinYinInit = null;
            bool inDict = false;

            //检查字典
            for(int i = 0; i< bs._buildingPinYinDict.Count;i++)
            {
                KeyValuePair<string, string[]> keyValue = bs._buildingPinYinDict.ElementAt(i);
                if(keyValue.Key == buildingName)
                {
                    inDict = true;
                    buildingPinYin = keyValue.Value[0];
                    buildingPinYinInit = keyValue.Value[1];
                    break;
                }
            }
            if (!inDict)
            {
                //生成拼音
                buildingPinYin = PinyinHelper.GetPinyin(buildingName).ToLower();
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
                    buildingPinYin = buildingPinYin.Replace(" ", "");
                }
                else
                {
                    //英文情况，大小写各检查一次
                    buildingPinYin = buildingName.ToUpper();
                    buildingPinYinInit = buildingName.ToLower();
                }
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

        //抽象起来了
        //你已经过线了.jpg
        //具体看generateBuildingDict.py
        private Dictionary<string, string[]> creatbuildingPinYinDict(Dictionary<string, string[]> Dict)
        {
            Dict.Add("核能配药桌", new string[2] { "henengpeiyaozhuo", "hnpyz" });
            Dict.Add("病诊站", new string[2] { "bingzhenzhan", "bzz" });
            Dict.Add("超级计算机", new string[2] { "chaojijisuanji", "cjjsj" });
            Dict.Add("飞行小动物诱惑器", new string[2] { "feixingxiaodongwuyouhuoqi", "fxxdwyhq" });
            Dict.Add("飞行小动物公寓", new string[2] { "feixingxiaodongwugongyu", "fxxdwgy" });
            Dict.Add("温度调节器", new string[2] { "wendutiaojieqi", "wdtjq" });
            Dict.Add("空气净化器", new string[2] { "kongqijinghuaqi", "kqjhq" });
            Dict.Add("藻类蒸馏器", new string[2] { "zaoleizhengliuqi", "zlzlq" });
            Dict.Add("藻类箱", new string[2] { "zaoleixiang", "zlx" });
            Dict.Add("配药桌", new string[2] { "peiyaozhuo", "pyz" });
            Dict.Add("游戏机台", new string[2] { "youxijitai", "yxjt" });
            Dict.Add("工艺品分析站", new string[2] { "gongyipinfenxizhan", "gypfxz" });
            Dict.Add("工艺品运输舱", new string[2] { "gongyipinyunshucang", "gypysc" });
            Dict.Add("太空学员离心机", new string[2] { "taikongxueyuanlixinji", "tkxylxj" });
            Dict.Add("自动采矿机", new string[2] { "zidongcaikuangji", "zdckj" });
            Dict.Add("电池", new string[2] { "dianchi", "dc" });
            Dict.Add("巨型电池", new string[2] { "juxingdianchi", "jxdc" });
            Dict.Add("电池舱", new string[2] { "dianchicang", "dcc" });
            Dict.Add("智能电池", new string[2] { "zhinengdianchi", "zndc" });
            Dict.Add("沙滩椅", new string[2] { "shatanyi", "sty" });
            Dict.Add("床铺", new string[2] { "chuangpu", "cp" });
            Dict.Add("辐射蜂巢", new string[2] { "fushefengchao", "fsfc" });
            Dict.Add("空瓶器", new string[2] { "kongpingqi", "kpq" });
            Dict.Add("空罐器", new string[2] { "kongguanqi", "kgq" });
            Dict.Add("地堡门", new string[2] { "dipumen", "dpm" });
            Dict.Add("地堡砖", new string[2] { "dipuzhuan", "dpz" });
            Dict.Add("空白画布", new string[2] { "kongbaihuabu", "kbhb" });
            Dict.Add("肖像画布", new string[2] { "xiaoxianghuabu", "xxhb" });
            Dict.Add("风景画布", new string[2] { "fengjinghuabu", "fjhb" });
            Dict.Add("货舱", new string[2] { "huocang", "hc" });
            Dict.Add("大型货舱", new string[2] { "daxinghuocang", "dxhc" });
            Dict.Add("地毯砖", new string[2] { "ditanzhuan", "dtz" });
            Dict.Add("吸顶灯", new string[2] { "xidingdeng", "xdd" });
            Dict.Add("复制人检查站", new string[2] { "fuzhirenjianchazhan", "fzrjcz" });
            Dict.Add("漂白石料斗", new string[2] { "piaobaishiliaodou", "pbsld" });
            Dict.Add("时装翻新器", new string[2] { "shizhuangfanxinqi", "szfxq" });
            Dict.Add("纺织机", new string[2] { "fangzhiji", "fzj" });
            Dict.Add("指挥舱", new string[2] { "zhihuicang", "zhc" });
            Dict.Add("内部舱门", new string[2] { "neibucangmen", "nbcm" });
            Dict.Add("望远镜", new string[2] { "wangyuanjing", "wyj" });
            Dict.Add("隔绝式望远镜", new string[2] { "gejueshiwangyuanjing", "gjswyj" });
            Dict.Add("二氧化碳引擎", new string[2] { "eryanghuatanyinqing", "eyhtyq" });
            Dict.Add("碳素脱离器", new string[2] { "tansutuoliqi", "tstlq" });
            Dict.Add("太空扫描仪", new string[2] { "taikongsaomiaoyi", "tksmy" });
            Dict.Add("堆肥堆", new string[2] { "duifeidui", "dfd" });
            Dict.Add("导热板", new string[2] { "daoreban", "drb" });
            Dict.Add("电动烤炉", new string[2] { "diandongkaolu", "ddkl" });
            Dict.Add("边角饰件", new string[2] { "bianjiaoshijian", "bjsj" });
            Dict.Add("虚拟天象仪", new string[2] { "xunitianxiangyi", "xntxy" });
            Dict.Add("工作台", new string[2] { "gongzuotai", "gzt" });
            Dict.Add("飞行小动物陷阱", new string[2] { "feixingxiaodongwuxianjing", "fxxdwxj" });
            Dict.Add("小动物放生点", new string[2] { "xiaodongwufangshengdian", "xdwfsd" });
            Dict.Add("小动物喂食器", new string[2] { "xiaodongwuweishiqi", "xdwwsq" });
            Dict.Add("小动物陷阱", new string[2] { "xiaodongwuxianjing", "xdwxj" });
            Dict.Add("小动物公寓", new string[2] { "xiaodongwugongyu", "xdwgy" });
            Dict.Add("小动物移取点", new string[2] { "xiaodongwuyiqudian", "xdwyqd" });
            Dict.Add("天花板饰件", new string[2] { "tianhuabanshijian", "thbsj" });
            Dict.Add("低温箱3000", new string[2] { "diwenxiang3000", "dwx3000" });
            Dict.Add("除污淋浴隔间", new string[2] { "chuwulinyugejian", "cwlygj" });
            Dict.Add("脱盐器", new string[2] { "tuoyanqi", "tyq" });
            Dict.Add("钻石压机", new string[2] { "zuanshiyaji", "zsyj" });
            Dict.Add("餐桌", new string[2] { "canzhuo", "cz" });
            Dict.Add("液渠", new string[2] { "yequ", "yq" });
            Dict.Add("蒸馏塔", new string[2] { "zhengliuta", "zlt" });
            Dict.Add("医务站", new string[2] { "yiwuzhan", "ywz" });
            Dict.Add("气动门", new string[2] { "qidongmen", "qdm" });
            Dict.Add("敲蛋桌", new string[2] { "qiaodanzhuo", "qdz" });
            Dict.Add("孵化器", new string[2] { "fuhuaqi", "fhq" });
            Dict.Add("电解器", new string[2] { "dianjieqi", "djq" });
            Dict.Add("逃生舱", new string[2] { "taoshengcang", "tsc" });
            Dict.Add("浓缩咖啡机", new string[2] { "nongsuokafeiji", "nskfj" });
            Dict.Add("乙醇蒸馏器", new string[2] { "yichunzhengliuqi", "yczlq" });
            Dict.Add("迷你舱", new string[2] { "minicang", "mnc" });
            Dict.Add("干板墙", new string[2] { "ganbanqiang", "gbq" });
            Dict.Add("农业站", new string[2] { "nongyezhan", "nyz" });
            Dict.Add("土培砖", new string[2] { "tupeizhuan", "tpz" });
            Dict.Add("肥料合成器", new string[2] { "feiliaohechengqi", "flhcq" });
            Dict.Add("消防滑杆", new string[2] { "xiaofanghuagan", "xfhg" });
            Dict.Add("鱼类放生点", new string[2] { "yuleifangshengdian", "ylfsd" });
            Dict.Add("喂鱼器", new string[2] { "weiyuqi", "wyq" });
            Dict.Add("捕鱼陷阱", new string[2] { "buyuxianjing", "byxj" });
            Dict.Add("电灯", new string[2] { "diandeng", "dd" });
            Dict.Add("压力板", new string[2] { "yaliban", "ylb" });
            Dict.Add("花盆", new string[2] { "huapen", "hp" });
            Dict.Add("垂吊花盆", new string[2] { "chuidiaohuapen", "cdhp" });
            Dict.Add("透明花盆", new string[2] { "touminghuapen", "tmhp" });
            Dict.Add("墙壁花盆", new string[2] { "qiangbihuapen", "qbhp" });
            Dict.Add("抽水马桶", new string[2] { "choushuimatong", "csmt" });
            Dict.Add("飞行小动物诱饵", new string[2] { "feixingxiaodongwuyouer", "fxxdwye" });
            Dict.Add("脱水机", new string[2] { "tuoshuiji", "tsj" });
            Dict.Add("充水机", new string[2] { "chongshuiji", "csj" });
            Dict.Add("伽马射线烤炉", new string[2] { "gamashexiankaolu", "gmsxkl" });
            Dict.Add("通行支架", new string[2] { "tongxingzhijia", "txzj" });
            Dict.Add("气体罐装器", new string[2] { "qitiguanzhuangqi", "qtgzq" });
            Dict.Add("气罐货舱", new string[2] { "qiguanhuocang", "qghc" });
            Dict.Add("大型气罐货舱", new string[2] { "daxingqiguanhuocang", "dxqghc" });
            Dict.Add("气体管道", new string[2] { "qitiguandao", "qtgd" });
            Dict.Add("气体管桥", new string[2] { "qitiguanqiao", "qtgq" });
            Dict.Add("气体管道病菌传感器", new string[2] { "qitiguandaobingjunchuanganqi", "qtgdbjcgq" });
            Dict.Add("气体管道元素传感器", new string[2] { "qitiguandaoyuansuchuanganqi", "qtgdyscgq" });
            Dict.Add("气体溢流阀", new string[2] { "qitiyiliufa", "qtylf" });
            Dict.Add("优先气流阀", new string[2] { "youxianqiliufa", "yxqlf" });
            Dict.Add("导热气体管道", new string[2] { "daoreqitiguandao", "drqtgd" });
            Dict.Add("气体管道温度传感器", new string[2] { "qitiguandaowenduchuanganqi", "qtgdwdcgq" });
            Dict.Add("气体筛选器", new string[2] { "qitishaixuanqi", "qtsxq" });
            Dict.Add("气体计量阀", new string[2] { "qitijiliangfa", "qtjlf" });
            Dict.Add("气体截断阀", new string[2] { "qitijieduanfa", "qtjdf" });
            Dict.Add("小型气泵", new string[2] { "xiaoxingqibeng", "xxqb" });
            Dict.Add("透气砖", new string[2] { "touqizhuan", "tqz" });
            Dict.Add("气泵", new string[2] { "qibeng", "qb" });
            Dict.Add("储气库", new string[2] { "chuqiku", "cqk" });
            Dict.Add("气体调节阀", new string[2] { "qititiaojiefa", "qttjf" });
            Dict.Add("排气口", new string[2] { "paiqikou", "pqk" });
            Dict.Add("高压排气口", new string[2] { "gaoyapaiqikou", "gypqk" });
            Dict.Add("煤炭发电机", new string[2] { "meitanfadianji", "mtfdj" });
            Dict.Add("全能打印机", new string[2] { "quannengdayinji", "qndyj" });
            Dict.Add("植物分析仪", new string[2] { "zhiwufenxiyi", "zwfxy" });
            Dict.Add("地质调谐仪", new string[2] { "dizhitiaoxieyi", "dztxy" });
            Dict.Add("玻璃熔炉", new string[2] { "bolironglu", "blrl" });
            Dict.Add("窗户砖", new string[2] { "chuanghuzhuan", "chz" });
            Dict.Add("燃气灶", new string[2] { "ranqizao", "rqz" });
            Dict.Add("雅致的纪念碑", new string[2] { "yazhidejinianbei", "yzdjnb" });
            Dict.Add("小动物衍变器", new string[2] { "xiaodongwuyanbianqi", "xdwybq" });
            Dict.Add("基座", new string[2] { "jizuo", "jz" });
            Dict.Add("太空员舱", new string[2] { "taikongyuancang", "tkyc" });
            Dict.Add("单太空员前锥", new string[2] { "dantaikongyuanqianzhui", "dtkyqz" });
            Dict.Add("手部消毒站", new string[2] { "shoubuxiaoduzhan", "sbxdz" });
            Dict.Add("打印舱", new string[2] { "dayincang", "dyc" });
            Dict.Add("液体均热器", new string[2] { "yetijunreqi", "ytjrq" });
            Dict.Add("辐射粒子蓄存器", new string[2] { "fushelizixucunqi", "fslzxcq" });
            Dict.Add("辐射粒子接合板", new string[2] { "fushelizijieheban", "fslzjhb" });
            Dict.Add("辐射粒子引擎", new string[2] { "fusheliziyinqing", "fslzyq" });
            Dict.Add("辐射粒子变向器", new string[2] { "fushelizibianxiangqi", "fslzbxq" });
            Dict.Add("辐射粒子发生器", new string[2] { "fushelizifashengqi", "fslzfsq" });
            Dict.Add("高负荷电线", new string[2] { "gaofuhedianxian", "gfhdx" });
            Dict.Add("热水浴缸", new string[2] { "reshuiyugang", "rsyg" });
            Dict.Add("液氢引擎", new string[2] { "yeqingyinqing", "yqyq" });
            Dict.Add("氢气发电机", new string[2] { "qingqifadianji", "qqfdj" });
            Dict.Add("液培砖", new string[2] { "yepeizhuan", "ypz" });
            Dict.Add("冰冷风扇", new string[2] { "binglengfengshan", "blfs" });
            Dict.Add("制冰机", new string[2] { "zhibingji", "zbj" });
            Dict.Add("冰雕塑块", new string[2] { "bingdiaosukuai", "bdsk" });
            Dict.Add("隔热气体管道", new string[2] { "gereqitiguandao", "grqtgd" });
            Dict.Add("隔热液体管道", new string[2] { "gereyetiguandao", "grytgd" });
            Dict.Add("隔热电线", new string[2] { "geredianxian", "grdx" });
            Dict.Add("隔热砖", new string[2] { "gerezhuan", "grz" });
            Dict.Add("喷气服存放柜", new string[2] { "penqifucunfanggui", "pqfcfg" });
            Dict.Add("喷气服检查站", new string[2] { "penqifujianchazhan", "pqfjcz" });
            Dict.Add("榨汁机", new string[2] { "zhazhiji", "zzj" });
            Dict.Add("石油引擎", new string[2] { "shiyouyinqing", "syyq" });
            Dict.Add("小型石油引擎", new string[2] { "xiaoxingshiyouyinqing", "xxsyyq" });
            Dict.Add("窑炉", new string[2] { "yaolu", "yl" });
            Dict.Add("梯子", new string[2] { "tizi", "tz" });
            Dict.Add("梯床", new string[2] { "tichuang", "tc" });
            Dict.Add("塑料梯子", new string[2] { "suliaotizi", "sltz" });
            Dict.Add("定位信标", new string[2] { "dingweixinbiao", "dwxb" });
            Dict.Add("火箭平台", new string[2] { "huojianpingtai", "hjpt" });
            Dict.Add("铅服存放柜", new string[2] { "qianfucunfanggui", "qfcfg" });
            Dict.Add("铅服检查站", new string[2] { "qianfujianchazhan", "qfjcz" });
            Dict.Add("液缸货舱", new string[2] { "yeganghuocang", "yghc" });
            Dict.Add("大型液缸货舱", new string[2] { "daxingyeganghuocang", "dxyghc" });
            Dict.Add("液温调节器", new string[2] { "yewentiaojieqi", "ywtjq" });
            Dict.Add("液体管道", new string[2] { "yetiguandao", "ytgd" });
            Dict.Add("液体管桥", new string[2] { "yetiguanqiao", "ytgq" });
            Dict.Add("液体管道病菌传感器", new string[2] { "yetiguandaobingjunchuanganqi", "ytgdbjcgq" });
            Dict.Add("液体管道元素传感器", new string[2] { "yetiguandaoyuansuchuanganqi", "ytgdyscgq" });
            Dict.Add("液体溢流阀", new string[2] { "yetiyiliufa", "ytylf" });
            Dict.Add("优先液流阀", new string[2] { "youxianyeliufa", "yxylf" });
            Dict.Add("导热液体管道", new string[2] { "daoreyetiguandao", "drytgd" });
            Dict.Add("液体管道温度传感器", new string[2] { "yetiguandaowenduchuanganqi", "ytgdwdcgq" });
            Dict.Add("水冷风扇", new string[2] { "shuilengfengshan", "slfs" });
            Dict.Add("液体筛选器", new string[2] { "yetishaixuanqi", "ytsxq" });
            Dict.Add("液缸燃料舱", new string[2] { "yegangranliaocang", "ygrlc" });
            Dict.Add("大型液缸燃料舱", new string[2] { "daxingyegangranliaocang", "dxygrlc" });
            Dict.Add("液体加热器", new string[2] { "yetijiareqi", "ytjrq" });
            Dict.Add("液体计量阀", new string[2] { "yetijiliangfa", "ytjlf" });
            Dict.Add("液体截断阀", new string[2] { "yetijieduanfa", "ytjdf" });
            Dict.Add("小型液泵", new string[2] { "xiaoxingyebeng", "xxyb" });
            Dict.Add("液泵", new string[2] { "yebeng", "yb" });
            Dict.Add("手压泵", new string[2] { "shouyabeng", "syb" });
            Dict.Add("储液库", new string[2] { "chuyeku", "cyk" });
            Dict.Add("液体调节阀", new string[2] { "yetitiaojiefa", "yttjf" });
            Dict.Add("排液口", new string[2] { "paiyekou", "pyk" });
            Dict.Add("自动化通知器", new string[2] { "zidonghuatongzhiqi", "zdhtzq" });
            Dict.Add("星图位置传感器", new string[2] { "xingtuweizhichuanganqi", "xtwzcgq" });
            Dict.Add("信号计数器", new string[2] { "xinhaojishuqi", "xhjsq" });
            Dict.Add("小动物传感器", new string[2] { "xiaodongwuchuanganqi", "xdwcgq" });
            Dict.Add("病菌传感器", new string[2] { "bingjunchuanganqi", "bjcgq" });
            Dict.Add("复制人运动传感器", new string[2] { "fuzhirenyundongchuanganqi", "fzrydcgq" });
            Dict.Add("气体元素传感器", new string[2] { "qitiyuansuchuanganqi", "qtyscgq" });
            Dict.Add("液体元素传感器", new string[2] { "yetiyuansuchuanganqi", "ytyscgq" });
            Dict.Add("与门", new string[2] { "yumen", "ym" });
            Dict.Add("缓冲门", new string[2] { "huanchongmen", "hcm" });
            Dict.Add("信号分配器", new string[2] { "xinhaofenpeiqi", "xhfpq" });
            Dict.Add("过滤门", new string[2] { "guolvmen", "glm" });
            Dict.Add("信号选择器", new string[2] { "xinhaoxuanzeqi", "xhxzq" });
            Dict.Add("非门", new string[2] { "feimen", "fm" });
            Dict.Add("或门", new string[2] { "huomen", "hm" });
            Dict.Add("异或门", new string[2] { "yihuomen", "yhm" });
            Dict.Add("音槌", new string[2] { "yinchui", "yc" });
            Dict.Add("辐射粒子传感器", new string[2] { "fushelizichuanganqi", "fslzcgq" });
            Dict.Add("信号接收器", new string[2] { "xinhaojieshouqi", "xhjsq" });
            Dict.Add("信号播报器", new string[2] { "xinhaobobaoqi", "xhbbq" });
            Dict.Add("光线传感器", new string[2] { "guangxianchuanganqi", "gxcgq" });
            Dict.Add("锁存器", new string[2] { "suocunqi", "scq" });
            Dict.Add("电力截断器", new string[2] { "dianlijieduanqi", "dljdq" });
            Dict.Add("气压传感器", new string[2] { "qiyachuanganqi", "qycgq" });
            Dict.Add("液压传感器", new string[2] { "yeyachuanganqi", "yycgq" });
            Dict.Add("辐射传感器", new string[2] { "fushechuanganqi", "fscgq" });
            Dict.Add("信号线组", new string[2] { "xinhaoxianzu", "xhxz" });
            Dict.Add("信号线组桥", new string[2] { "xinhaoxianzuqiao", "xhxzq" });
            Dict.Add("线组读取器", new string[2] { "xianzuduquqi", "xzdqq" });
            Dict.Add("线组写入器", new string[2] { "xianzuxieruqi", "xzxrq" });
            Dict.Add("信号开关", new string[2] { "xinhaokaiguan", "xhkg" });
            Dict.Add("温度传感器", new string[2] { "wenduchuanganqi", "wdcgq" });
            Dict.Add("周期传感器", new string[2] { "zhouqichuanganqi", "zqcgq" });
            Dict.Add("时间传感器", new string[2] { "shijianchuanganqi", "sjcgq" });
            Dict.Add("功率传感器", new string[2] { "gonglvchuanganqi", "glcgq" });
            Dict.Add("信号线", new string[2] { "xinhaoxian", "xhx" });
            Dict.Add("信号线桥", new string[2] { "xinhaoxianqiao", "xhxq" });
            Dict.Add("庄严科技运输集装箱", new string[2] { "zhuangyankejiyunshujizhuangxiang", "zykjysjzx" });
            Dict.Add("舒适的床", new string[2] { "shushidechuang", "ssdc" });
            Dict.Add("机械站", new string[2] { "jixiezhan", "jxz" });
            Dict.Add("人力发电机", new string[2] { "renlifadianji", "rlfdj" });
            Dict.Add("人力辐射粒子发生器", new string[2] { "renlifushelizifashengqi", "rlfslzfsq" });
            Dict.Add("手动气闸", new string[2] { "shoudongqizha", "sdqz" });
            Dict.Add("大理石雕塑块", new string[2] { "dalishidiaosukuai", "dlsdsk" });
            Dict.Add("按摩床", new string[2] { "anmochuang", "amc" });
            Dict.Add("反熵热量中和器", new string[2] { "fanshangreliangzhongheqi", "fsrlzhq" });
            Dict.Add("机械冲浪板", new string[2] { "jixiechonglangban", "jxclb" });
            Dict.Add("分诊床", new string[2] { "fenzhenchuang", "fzc" });
            Dict.Add("梦境合成仪", new string[2] { "mengjinghechengyi", "mjhcy" });
            Dict.Add("网格砖", new string[2] { "wanggezhuan", "wgz" });
            Dict.Add("金属精炼器", new string[2] { "jinshujinglianqi", "jsjlq" });
            Dict.Add("金属雕塑块", new string[2] { "jinshudiaosukuai", "jsdsk" });
            Dict.Add("金属砖", new string[2] { "jinshuzhuan", "jsz" });
            Dict.Add("天然气发电机", new string[2] { "tianranqifadianji", "trqfdj" });
            Dict.Add("食物压制器", new string[2] { "shiwuyazhiqi", "swyzq" });
            Dict.Add("咸乳蜡收集器", new string[2] { "xianrulashoujiqi", "xrlsjq" });
            Dict.Add("小动物直饮器", new string[2] { "xiaodongwuzhiyinqi", "xdwzyq" });
            Dict.Add("挤奶站", new string[2] { "jinaizhan", "jnz" });
            Dict.Add("植物粉碎机", new string[2] { "zhiwufensuiji", "zwfsj" });
            Dict.Add("氧气扩散器", new string[2] { "yangqikuosanqi", "yqksq" });
            Dict.Add("爆破弹组装机", new string[2] { "baopodanzuzhuangji", "bpdzzj" });
            Dict.Add("流星爆破炮", new string[2] { "liuxingbaopopao", "lxbpp" });
            Dict.Add("航天指挥站", new string[2] { "hangtianzhihuizhan", "htzhz" });
            Dict.Add("气体输入枢纽", new string[2] { "qitishurushuniu", "qtsrsn" });
            Dict.Add("火箭端口", new string[2] { "huojianduankou", "hjdk" });
            Dict.Add("火箭端口延伸桥", new string[2] { "huojianduankouyanshenqiao", "hjdkysq" });
            Dict.Add("火箭端口气体装载器", new string[2] { "huojianduankouqitizhuangzaiqi", "hjdkqtzzq" });
            Dict.Add("火箭端口液体装载器", new string[2] { "huojianduankouyetizhuangzaiqi", "hjdkytzzq" });
            Dict.Add("火箭端口固体装载器", new string[2] { "huojianduankougutizhuangzaiqi", "hjdkgtzzq" });
            Dict.Add("液体输入枢纽", new string[2] { "yetishurushuniu", "ytsrsn" });
            Dict.Add("固体输入枢纽", new string[2] { "gutishurushuniu", "gtsrsn" });
            Dict.Add("丰碑底基", new string[2] { "fengbeidiji", "fbdj" });
            Dict.Add("丰碑中段", new string[2] { "fengbeizhongduan", "fbzd" });
            Dict.Add("丰碑顶部", new string[2] { "fengbeidingbu", "fbdb" });
            Dict.Add("生机组构仪", new string[2] { "shengjizugouyi", "sjzgy" });
            Dict.Add("镶边砖", new string[2] { "xiangbianzhuan", "xbz" });
            Dict.Add("基础前锥", new string[2] { "jichuqianzhui", "jcqz" });
            Dict.Add("钻头前锥", new string[2] { "zuantouqianzhui", "ztqz" });
            Dict.Add("研究性反应堆", new string[2] { "yanjiuxingfanyingdui", "yjxfyd" });
            Dict.Add("材料研究终端", new string[2] { "cailiaoyanjiuzhongduan", "clyjzd" });
            Dict.Add("自动卸物箱", new string[2] { "zidongxiewuxiang", "zdxwx" });
            Dict.Add("原油精炼器", new string[2] { "yuanyoujinglianqi", "yyjlq" });
            Dict.Add("油井", new string[2] { "youjing", "yj" });
            Dict.Add("环轨货舱", new string[2] { "huanguihuocang", "hghc" });
            Dict.Add("轨道数据收集实验仪", new string[2] { "guidaoshujushoujishiyanyi", "gdsjsjsyy" });
            Dict.Add("矿石洗涤器", new string[2] { "kuangshixidiqi", "ksxdq" });
            Dict.Add("户外厕所", new string[2] { "huwaicesuo", "hwcs" });
            Dict.Add("固体氧化剂舱", new string[2] { "gutiyanghuajicang", "gtyhjc" });
            Dict.Add("大型固体氧化剂舱", new string[2] { "daxinggutiyanghuajicang", "dxgtyhjc" });
            Dict.Add("液体氧化剂舱", new string[2] { "yetiyanghuajicang", "ytyhjc" });
            Dict.Add("氧气面罩存放柜", new string[2] { "yangqimianzhaocunfanggui", "yqmzcfg" });
            Dict.Add("氧气面罩检查站", new string[2] { "yangqimianzhaojianchazhan", "yqmzjcz" });
            Dict.Add("氧气面罩站", new string[2] { "yangqimianzhaozhan", "yqmzz" });
            Dict.Add("氧石精炼炉", new string[2] { "yangshijinglianlu", "ysjll" });
            Dict.Add("公园标识", new string[2] { "gongyuanbiaozhi", "gybz" });
            Dict.Add("三层蛋糕", new string[2] { "sancengdangao", "scdg" });
            Dict.Add("石油发电机", new string[2] { "shiyoufadianji", "syfdj" });
            Dict.Add("点唱机器人", new string[2] { "dianchangjiqiren", "dcjqr" });
            Dict.Add("先锋着陆器", new string[2] { "xianfengzhuoluqi", "xfzlq" });
            Dict.Add("先锋舱", new string[2] { "xianfengcang", "xfc" });
            Dict.Add("像素屏", new string[2] { "xiangsuping", "xsp" });
            Dict.Add("种植箱", new string[2] { "zhongzhixiang", "zzx" });
            Dict.Add("塑料砖", new string[2] { "suliaozhuan", "slz" });
            Dict.Add("聚合物压塑器", new string[2] { "juhewuyasuqi", "jhwysq" });
            Dict.Add("电控站", new string[2] { "diankongzhan", "dkz" });
            Dict.Add("大型变压器", new string[2] { "daxingbianyaqi", "dxbyq" });
            Dict.Add("变压器", new string[2] { "bianyaqi", "byq" });
            Dict.Add("机械气闸", new string[2] { "jixieqizha", "jxqz" });
            Dict.Add("气压电闸", new string[2] { "qiyadianzha", "qydz" });
            Dict.Add("液压电闸", new string[2] { "yeyadianzha", "yydz" });
            Dict.Add("辐射灯", new string[2] { "fushedeng", "fsd" });
            Dict.Add("星际发射器", new string[2] { "xingjifasheqi", "xjfsq" });
            Dict.Add("载货拆包器", new string[2] { "zaihuochaibaoqi", "zhcbq" });
            Dict.Add("照料站", new string[2] { "zhaoliaozhan", "zlz" });
            Dict.Add("口粮箱", new string[2] { "kouliangxiang", "klx" });
            Dict.Add("冰箱", new string[2] { "bingxiang", "bx" });
            Dict.Add("研究站", new string[2] { "yanjiuzhan", "yjz" });
            Dict.Add("研究舱", new string[2] { "yanjiucang", "yjc" });
            Dict.Add("技能涤除器", new string[2] { "jinengdichuqi", "jndcq" });
            Dict.Add("碎石机", new string[2] { "suishiji", "ssj" });
            Dict.Add("火箭驾驶舱", new string[2] { "huojianjiashicang", "hjjsc" });
            Dict.Add("火箭控制台", new string[2] { "huojiankongzhitai", "hjkzt" });
            Dict.Add("火箭", new string[2] { "huojian", "hj" });
            Dict.Add("火箭窗户", new string[2] { "huojianchuanghu", "hjch" });
            Dict.Add("气体输入配件", new string[2] { "qitishurupeijian", "qtsrpj" });
            Dict.Add("太空员舱气体输入端口", new string[2] { "taikongyuancangqitishuruduankou", "tkycqtsrdk" });
            Dict.Add("气体输出配件", new string[2] { "qitishuchupeijian", "qtscpj" });
            Dict.Add("太空员舱气体输出端口", new string[2] { "taikongyuancangqitishuchuduankou", "tkycqtscdk" });
            Dict.Add("液体输入配件", new string[2] { "yetishurupeijian", "ytsrpj" });
            Dict.Add("太空员舱液体输入端口", new string[2] { "taikongyuancangyetishuruduankou", "tkycytsrdk" });
            Dict.Add("液体输出配件", new string[2] { "yetishuchupeijian", "ytscpj" });
            Dict.Add("太空员舱液体输出端口", new string[2] { "taikongyuancangyetishuchuduankou", "tkycytscdk" });
            Dict.Add("电源插座配件", new string[2] { "dianyuanchazuopeijian", "dyczpj" });
            Dict.Add("运输存放配件", new string[2] { "yunshucunfangpeijian", "yscfpj" });
            Dict.Add("运输装载配件", new string[2] { "yunshuzhuangzaipeijian", "yszzpj" });
            Dict.Add("拓荒者部署舱", new string[2] { "tuohuangzhebushucang", "thzbsc" });
            Dict.Add("火箭墙体", new string[2] { "huojianqiangti", "hjqt" });
            Dict.Add("技能公示板", new string[2] { "jinenggongshiban", "jngsb" });
            Dict.Add("铁锈脱氧机", new string[2] { "tiexiutuoyangji", "txtyj" });
            Dict.Add("桑拿浴室", new string[2] { "sangnayushi", "snys" });
            Dict.Add("制图舱", new string[2] { "zhitucang", "ztc" });
            Dict.Add("侦察着陆器", new string[2] { "zhenchazhuoluqi", "zczlq" });
            Dict.Add("侦察舱", new string[2] { "zhenchacang", "zcc" });
            Dict.Add("侦察窝", new string[2] { "zhenchawo", "zcw" });
            Dict.Add("大型雕塑块", new string[2] { "daxingdiaosukuai", "dxdsk" });
            Dict.Add("修剪站", new string[2] { "xiujianzhan", "xjz" });
            Dict.Add("淋浴隔间", new string[2] { "linyugejian", "lygj" });
            Dict.Add("单人游戏机台", new string[2] { "danrenyouxijitai", "dryxjt" });
            Dict.Add("泥浆分离器", new string[2] { "nijiangfenliqi", "njflq" });
            Dict.Add("小型固体氧化剂舱", new string[2] { "xiaoxinggutiyanghuajicang", "xxgtyhjc" });
            Dict.Add("雕塑块", new string[2] { "diaosukuai", "dsk" });
            Dict.Add("汽水机", new string[2] { "qishuiji", "qsj" });
            Dict.Add("太阳能板", new string[2] { "taiyangnengban", "tynb" });
            Dict.Add("太阳能舱", new string[2] { "taiyangnengcang", "tync" });
            Dict.Add("固体燃料推进器", new string[2] { "gutiranliaotuijinqi", "gtrltjq" });
            Dict.Add("运输轨道", new string[2] { "yunshuguidao", "ysgd" });
            Dict.Add("运输轨桥", new string[2] { "yunshuguiqiao", "ysgq" });
            Dict.Add("运输轨道病菌传感器", new string[2] { "yunshuguidaobingjunchuanganqi", "ysgdbjcgq" });
            Dict.Add("运输轨道元素传感器", new string[2] { "yunshuguidaoyuansuchuanganqi", "ysgdyscgq" });
            Dict.Add("运输装载器", new string[2] { "yunshuzhuangzaiqi", "yszzq" });
            Dict.Add("运输存放器", new string[2] { "yunshucunfangqi", "yscfq" });
            Dict.Add("运输轨道温度传感器", new string[2] { "yunshuguidaowenduchuanganqi", "ysgdwdcgq" });
            Dict.Add("固体筛选器", new string[2] { "gutishaixuanqi", "gtsxq" });
            Dict.Add("轨道计量器", new string[2] { "guidaojiliangqi", "gdjlq" });
            Dict.Add("轨道截断器", new string[2] { "guidaojieduanqi", "gdjdq" });
            Dict.Add("自动清扫器", new string[2] { "zidongqingsaoqi", "zdqsq" });
            Dict.Add("轨道滑槽", new string[2] { "guidaohuacao", "gdhc" });
            Dict.Add("空间加热器", new string[2] { "kongjianjiareqi", "kjjrq" });
            Dict.Add("生物货舱", new string[2] { "shengwuhuocang", "swhc" });
            Dict.Add("小动物货舱", new string[2] { "xiaodongwuhuocang", "xdwhc" });
            Dict.Add("香料研磨器", new string[2] { "xiangliaoyanmoqi", "xlymq" });
            Dict.Add("蛞蝓蛋", new string[2] { "kuoyudan", "kyd" });
            Dict.Add("电弧蛞蝓", new string[2] { "dianhukuoyu", "dhky" });
            Dict.Add("蒸汽引擎", new string[2] { "zhengqiyinqing", "zqyq" });
            Dict.Add("[弃用] 蒸汽涡轮机", new string[2] { "[qiyong] zhengqiwolunji", "[qy] zqwlj" });
            Dict.Add("蒸汽涡轮机", new string[2] { "zhengqiwolunji", "zqwlj" });
            Dict.Add("贴纸炸弹", new string[2] { "tiezhizhadan", "tzzd" });
            Dict.Add("存储箱", new string[2] { "cunchuxiang", "ccx" });
            Dict.Add("智能存储箱", new string[2] { "zhinengcunchuxiang", "znccx" });
            Dict.Add("存储砖", new string[2] { "cunchuzhuan", "ccz" });
            Dict.Add("升华站", new string[2] { "shenghuazhan", "shz" });
            Dict.Add("糖素引擎", new string[2] { "tangsuyinqing", "tsyq" });
            Dict.Add("太空服锻造台", new string[2] { "taikongfuduanzaotai", "tkfdzt" });
            Dict.Add("气压服存放柜", new string[2] { "qiyafucunfanggui", "qyfcfg" });
            Dict.Add("气压服检查站", new string[2] { "qiyafujianchazhan", "qyfjcz" });
            Dict.Add("日光灯", new string[2] { "riguangdeng", "rgd" });
            Dict.Add("分子熔炉", new string[2] { "fenzironglu", "fzrl" });
            Dict.Add("扫扫基站", new string[2] { "saosaojizhan", "ssjz" });
            Dict.Add("电闸", new string[2] { "dianzha", "dz" });
            Dict.Add("共线电话", new string[2] { "gongxiandianhua", "gxdh" });
            Dict.Add("温控开关", new string[2] { "wenkongkaiguan", "wkkg" });
            Dict.Add("变温板", new string[2] { "bianwenban", "bwb" });
            Dict.Add("砖块", new string[2] { "zhuankuai", "zk" });
            Dict.Add("观光舱", new string[2] { "guanguangcang", "ggc" });
            Dict.Add("运载管道", new string[2] { "yunzaiguandao", "yzgd" });
            Dict.Add("运载管道入口", new string[2] { "yunzaiguandaorukou", "yzgdrk" });
            Dict.Add("运载管道渡口", new string[2] { "yunzaiguandaodukou", "yzgddk" });
            Dict.Add("水中城堡", new string[2] { "shuizhongchengbao", "szcb" });
            Dict.Add("铀素离心机", new string[2] { "yousulixinji", "yslxj" });
            Dict.Add("垂直风洞", new string[2] { "chuizhifengdong", "czfd" });
            Dict.Add("墙排马桶", new string[2] { "qiangpaimatong", "qpmt" });
            Dict.Add("洗手盆", new string[2] { "xishoupen", "xsp" });
            Dict.Add("洗手池", new string[2] { "xishouchi", "xsc" });
            Dict.Add("饮水机", new string[2] { "yinshuiji", "ysj" });
            Dict.Add("净水器", new string[2] { "jingshuiqi", "jsq" });
            Dict.Add("电线", new string[2] { "dianxian", "dx" });
            Dict.Add("电线桥", new string[2] { "dianxianqiao", "dxq" });
            Dict.Add("高负荷电线接合板", new string[2] { "gaofuhedianxianjieheban", "gfhdxjhb" });
            Dict.Add("导线", new string[2] { "daoxian", "dx" });
            Dict.Add("导线桥", new string[2] { "daoxianqiao", "dxq" });
            Dict.Add("高负荷导线接合板", new string[2] { "gaofuhedaoxianjieheban", "gfhdxjhb" });
            Dict.Add("高负荷导线", new string[2] { "gaofuhedaoxian", "gfhdx" });
            Dict.Add("木料燃烧器", new string[2] { "muliaoranshaoqi", "mlrsq" });
            return Dict;
        }
    }

    [HarmonyPatch(typeof(TreeFilterableSideScreenRow), "FilterAgainstSearch")]
    internal class ItemSearch
    {
        private Dictionary<string, string[]> _elementPinYinDict = new Dictionary<string, string[]>();

        public static bool Prefix(TreeFilterableSideScreenRow __instance, ref Tag thisCategoryTag, ref string search)
        {
            //初始化字典
            var itemSearch = new ItemSearch();
            if (itemSearch._elementPinYinDict.Count == 0)
            {
                itemSearch.creatElementPinYinDict(itemSearch._elementPinYinDict);
            }

            //获取两个private的变量
            List<TreeFilterableSideScreenElement> rowElements = Traverse.Create(__instance).Field("rowElements").GetValue<List<TreeFilterableSideScreenElement>>();
            MultiToggle arrowToggle = Traverse.Create(__instance).Field("arrowToggle").GetValue<MultiToggle>();
            
            bool flag = false;
            bool flag2 = thisCategoryTag.ProperNameStripLink().ToUpper().Contains(search.ToUpper());
            search = search.ToUpper();
            foreach (TreeFilterableSideScreenElement treeFilterableSideScreenElement in rowElements)
            {
                bool flag3 = flag2 || itemSearch.getPinYin(itemSearch, treeFilterableSideScreenElement.GetElementTag().ProperNameStripLink()).Contains(search.ToLower());
                treeFilterableSideScreenElement.gameObject.SetActive(flag3);
                flag = (flag || flag3);
            }
            __instance.gameObject.SetActive(flag);
            if (search != "" && flag && arrowToggle.CurrentState == 0)
            {
                __instance.SetArrowToggleState(true);
            }
            return false;
        }
        private string getPinYin(ItemSearch itemSearch, string elementName)
        {

            String elementPinYin = null;
            bool inDict = false;

            //检查字典
            for (int i = 0; i < itemSearch._elementPinYinDict.Count; i++)
            {
                KeyValuePair<string, string[]> keyValue = itemSearch._elementPinYinDict.ElementAt(i);
                if (keyValue.Key == elementName)
                {
                    inDict = true;
                    elementPinYin = elementName + keyValue.Value[0] + keyValue.Value[1];
                    break;
                }
            }
            if (!inDict)
            {
                //生成拼音
                elementPinYin = PinyinHelper.GetPinyin(elementName).ToLower();
                //当拼音后的长度超原本长度两倍以上的时候将其认为是非中文
                //StringBuilder有默认长度，避免来一个超长的mod建筑名字塞爆StringBuilder
                if (elementPinYin.Length > elementName.Length * 2)
                {
                    //获取首字母作为快捷搜索
                    String[] elementPinYinTemp = elementPinYin.Split(' ');
                    StringBuilder elementPinYinInitTemp = new StringBuilder(64);
                    foreach (string str in elementPinYinTemp)
                    {
                        elementPinYinInitTemp.Append(str[0]);
                    }
                    elementPinYin = elementPinYin + elementPinYinInitTemp.ToString();

                    //去除拼音中的空格分隔，获取搜索输入
                    elementPinYin = elementPinYin.Replace(" ", "");
                }
                else
                {
                    elementPinYin = elementName.ToLower();
                }
            }
            return elementPinYin;
        }

        //Ah, here we go again.
        private Dictionary<string, string[]> creatElementPinYinDict(Dictionary<string, string[]> Dict)
        {
            Dict.Add("气凝胶", new string[2] { "qiningjiao", "qnj" });
            Dict.Add("藻类", new string[2] { "zaolei", "zl" });
            Dict.Add("铝", new string[2] { "lv", "l" });
            Dict.Add("气态铝", new string[2] { "qitailv", "qtl" });
            Dict.Add("铝矿", new string[2] { "lvkuang", "lk" });
            Dict.Add("沥青", new string[2] { "liqing", "lq" });
            Dict.Add("漂白石", new string[2] { "piaobaishi", "pbs" });
            Dict.Add("瓶装水", new string[2] { "pingzhuangshui", "pzs" });
            Dict.Add("砖料", new string[2] { "zhuanliao", "zl" });
            Dict.Add("浓盐水", new string[2] { "nongyanshui", "nys" });
            Dict.Add("浓盐冰", new string[2] { "nongyanbing", "nyb" });
            Dict.Add("煤炭", new string[2] { "meitan", "mt" });
            Dict.Add("二氧化碳", new string[2] { "eryanghuatan", "eyht" });
            Dict.Add("碳纤维", new string[2] { "tanxianwei", "txw" });
            Dict.Add("气态碳", new string[2] { "qitaitan", "qtt" });
            Dict.Add("水泥", new string[2] { "shuini", "sn" });
            Dict.Add("水泥混合料", new string[2] { "shuinihunheliao", "snhhl" });
            Dict.Add("陶瓷", new string[2] { "taoci", "tc" });
            Dict.Add("液态氯", new string[2] { "yetailv", "ytl" });
            Dict.Add("氯气", new string[2] { "lvqi", "lq" });
            Dict.Add("粘土", new string[2] { "niantu", "nt" });
            Dict.Add("钴", new string[2] { "gu", "g" });
            Dict.Add("气态钴", new string[2] { "qitaigu", "qtg" });
            Dict.Add("钴矿", new string[2] { "gukuang", "gk" });
            Dict.Add("复合物", new string[2] { "fuhewu", "fhw" });
            Dict.Add("污染氧", new string[2] { "wuranyang", "wry" });
            Dict.Add("铜", new string[2] { "tong", "t" });
            Dict.Add("气态铜", new string[2] { "qitaitong", "qtt" });
            Dict.Add("堆芯熔融物", new string[2] { "duixinrongrongwu", "dxrrw" });
            Dict.Add("遗传生物软泥", new string[2] { "yichuanshengwuruanni", "ycswrn" });
            Dict.Add("原油", new string[2] { "yuanyou", "yy" });
            Dict.Add("碎冰", new string[2] { "suibing", "sb" });
            Dict.Add("碎岩", new string[2] { "suiyan", "sy" });
            Dict.Add("铜矿", new string[2] { "tongkuang", "tk" });
            Dict.Add("贫铀", new string[2] { "pinyou", "py" });
            Dict.Add("钻石", new string[2] { "zuanshi", "zs" });
            Dict.Add("泥土", new string[2] { "nitu", "nt" });
            Dict.Add("污染冰", new string[2] { "wuranbing", "wrb" });
            Dict.Add("污染水", new string[2] { "wuranshui", "wrs" });
            Dict.Add("银金矿", new string[2] { "yinjinkuang", "yjk" });
            Dict.Add("浓缩铀", new string[2] { "nongsuoyou", "nsy" });
            Dict.Add("乙醇", new string[2] { "yichun", "yc" });
            Dict.Add("气态乙醇", new string[2] { "qitaiyichun", "qtyc" });
            Dict.Add("核尘埃", new string[2] { "hechenai", "hca" });
            Dict.Add("肥料", new string[2] { "feiliao", "fl" });
            Dict.Add("黄铁矿", new string[2] { "huangtiekuang", "htk" });
            Dict.Add("化石", new string[2] { "huashi", "hs" });
            Dict.Add("富勒烯", new string[2] { "fuleixi", "flx" });
            Dict.Add("玻璃", new string[2] { "boli", "bl" });
            Dict.Add("金", new string[2] { "jin", "j" });
            Dict.Add("金汞齐", new string[2] { "jingongqi", "jgq" });
            Dict.Add("气态金", new string[2] { "qitaijin", "qtj" });
            Dict.Add("花岗岩", new string[2] { "huagangyan", "hgy" });
            Dict.Add("石墨", new string[2] { "shimo", "sm" });
            Dict.Add("塑料质", new string[2] { "suliaozhi", "slz" });
            Dict.Add("氦气", new string[2] { "haiqi", "hq" });
            Dict.Add("氢气", new string[2] { "qingqi", "qq" });
            Dict.Add("冰", new string[2] { "bing", "b" });
            Dict.Add("火成岩", new string[2] { "huochengyan", "hcy" });
            Dict.Add("铁", new string[2] { "tie", "t" });
            Dict.Add("气态铁", new string[2] { "qitaitie", "qtt" });
            Dict.Add("铁矿", new string[2] { "tiekuang", "tk" });
            Dict.Add("异构树脂", new string[2] { "yigoushuzhi", "ygsz" });
            Dict.Add("深渊晶石", new string[2] { "shenyuanjingshi", "syjs" });
            Dict.Add("铅", new string[2] { "qian", "q" });
            Dict.Add("气态铅", new string[2] { "qitaiqian", "qtq" });
            Dict.Add("石灰", new string[2] { "shihui", "sh" });
            Dict.Add("液态二氧化碳", new string[2] { "yetaieryanghuatan", "yteyht" });
            Dict.Add("液态氦", new string[2] { "yetaihai", "yth" });
            Dict.Add("液态氢", new string[2] { "yetaiqing", "ytq" });
            Dict.Add("液态甲烷", new string[2] { "yetaijiawan", "ytjw" });
            Dict.Add("液态氧", new string[2] { "yetaiyang", "yty" });
            Dict.Add("液态磷", new string[2] { "yetailin", "ytl" });
            Dict.Add("液态丙烷", new string[2] { "yetaibingwan", "ytbw" });
            Dict.Add("液态硫", new string[2] { "yetailiu", "ytl" });
            Dict.Add("镁铁质岩", new string[2] { "meitiezhiyan", "mtzy" });
            Dict.Add("岩浆", new string[2] { "yanjiang", "yj" });
            Dict.Add("汞", new string[2] { "gong", "g" });
            Dict.Add("汞蒸气", new string[2] { "gongzhengqi", "gzq" });
            Dict.Add("天然气", new string[2] { "tianranqi", "trq" });
            Dict.Add("咸乳", new string[2] { "xianru", "xr" });
            Dict.Add("咸乳蜡", new string[2] { "xianrula", "xrl" });
            Dict.Add("凝冻咸乳", new string[2] { "ningdongxianru", "ndxr" });
            Dict.Add("熔融铝", new string[2] { "rongronglv", "rrl" });
            Dict.Add("熔融碳", new string[2] { "rongrongtan", "rrt" });
            Dict.Add("熔融钴", new string[2] { "rongronggu", "rrg" });
            Dict.Add("熔融铜", new string[2] { "rongrongtong", "rrt" });
            Dict.Add("熔融玻璃", new string[2] { "rongrongboli", "rrbl" });
            Dict.Add("熔融金", new string[2] { "rongrongjin", "rrj" });
            Dict.Add("熔融铁", new string[2] { "rongrongtie", "rrt" });
            Dict.Add("熔融铅", new string[2] { "rongrongqian", "rrq" });
            Dict.Add("熔融铌", new string[2] { "rongrongni", "rrn" });
            Dict.Add("熔融盐", new string[2] { "rongrongyan", "rry" });
            Dict.Add("熔融钢", new string[2] { "rongronggang", "rrg" });
            Dict.Add("熔融蔗糖", new string[2] { "rongrongzhetang", "rrzt" });
            Dict.Add("熔融合成气", new string[2] { "rongronghechengqi", "rrhcq" });
            Dict.Add("熔融钨", new string[2] { "rongrongwu", "rrw" });
            Dict.Add("熔融二硒化钨", new string[2] { "rongrongerxihuawu", "rrexhw" });
            Dict.Add("熔融铀", new string[2] { "rongrongyou", "rry" });
            Dict.Add("泥巴", new string[2] { "niba", "nb" });
            Dict.Add("液态石脑油", new string[2] { "yetaishinaoyou", "ytsny" });
            Dict.Add("铌", new string[2] { "ni", "n" });
            Dict.Add("气态铌", new string[2] { "qitaini", "qtn" });
            Dict.Add("液态核废料", new string[2] { "yetaihefeiliao", "ythfl" });
            Dict.Add("黑曜石", new string[2] { "heiyaoshi", "hys" });
            Dict.Add("氧气", new string[2] { "yangqi", "yq" });
            Dict.Add("氧石", new string[2] { "yangshi", "ys" });
            Dict.Add("石油", new string[2] { "shiyou", "sy" });
            Dict.Add("磷酸盐结晶", new string[2] { "linsuanyanjiejing", "lsyjj" });
            Dict.Add("磷矿", new string[2] { "linkuang", "lk" });
            Dict.Add("精炼磷", new string[2] { "jinglianlin", "jll" });
            Dict.Add("气态磷", new string[2] { "qitailin", "qtl" });
            Dict.Add("塑料", new string[2] { "suliao", "sl" });
            Dict.Add("绿藻浮渣", new string[2] { "lvzaofuzha", "lzfz" });
            Dict.Add("丙烷", new string[2] { "bingwan", "bw" });
            Dict.Add("镭", new string[2] { "lei", "l" });
            Dict.Add("精炼碳", new string[2] { "jingliantan", "jlt" });
            Dict.Add("浮土", new string[2] { "futu", "ft" });
            Dict.Add("液态树脂", new string[2] { "yetaishuzhi", "ytsz" });
            Dict.Add("气态岩", new string[2] { "qitaiyan", "qty" });
            Dict.Add("铁锈", new string[2] { "tiexiu", "tx" });
            Dict.Add("盐", new string[2] { "yan", "y" });
            Dict.Add("气态盐", new string[2] { "qitaiyan", "qty" });
            Dict.Add("盐水", new string[2] { "yanshui", "ys" });
            Dict.Add("沙子", new string[2] { "shazi", "sz" });
            Dict.Add("砂水泥", new string[2] { "shashuini", "ssn" });
            Dict.Add("砂岩", new string[2] { "shayan", "sy" });
            Dict.Add("沉积岩", new string[2] { "chenjiyan", "cjy" });
            Dict.Add("建筑板材", new string[2] { "jianzhubancai", "jzbc" });
            Dict.Add("菌泥", new string[2] { "junni", "jn" });
            Dict.Add("雪", new string[2] { "xue", "x" });
            Dict.Add("固态二氧化碳", new string[2] { "gutaieryanghuatan", "gteyht" });
            Dict.Add("固态氯", new string[2] { "gutailv", "gtl" });
            Dict.Add("固态原油", new string[2] { "gutaiyuanyou", "gtyy" });
            Dict.Add("固态乙醇", new string[2] { "gutaiyichun", "gtyc" });
            Dict.Add("固态氢", new string[2] { "gutaiqing", "gtq" });
            Dict.Add("固态汞", new string[2] { "gutaigong", "gtg" });
            Dict.Add("固态甲烷", new string[2] { "gutaijiawan", "gtjw" });
            Dict.Add("固态石脑油", new string[2] { "gutaishinaoyou", "gtsny" });
            Dict.Add("固态核废料", new string[2] { "gutaihefeiliao", "gthfl" });
            Dict.Add("固态氧", new string[2] { "gutaiyang", "gty" });
            Dict.Add("固态石油", new string[2] { "gutaishiyou", "gtsy" });
            Dict.Add("固态丙烷", new string[2] { "gutaibingwan", "gtbw" });
            Dict.Add("固态树脂", new string[2] { "gutaishuzhi", "gtsz" });
            Dict.Add("固态超级冷却剂", new string[2] { "gutaichaojilengqueji", "gtcjlqj" });
            Dict.Add("固态合成气", new string[2] { "gutaihechengqi", "gthcq" });
            Dict.Add("固态粘性凝胶", new string[2] { "gutaizhanxingningjiao", "gtzxnj" });
            Dict.Add("高硫天然气", new string[2] { "gaoliutianranqi", "gltrq" });
            Dict.Add("蒸汽", new string[2] { "zhengqi", "zq" });
            Dict.Add("钢", new string[2] { "gang", "g" });
            Dict.Add("气态钢", new string[2] { "qitaigang", "qtg" });
            Dict.Add("蔗糖", new string[2] { "zhetang", "zt" });
            Dict.Add("硫", new string[2] { "liu", "l" });
            Dict.Add("硫蒸气", new string[2] { "liuzhengqi", "lzq" });
            Dict.Add("超级冷却剂", new string[2] { "chaojilengqueji", "cjlqj" });
            Dict.Add("气态超级冷却剂", new string[2] { "qitaichaojilengqueji", "qtcjlqj" });
            Dict.Add("隔热质", new string[2] { "gerezhi", "grz" });
            Dict.Add("合成气", new string[2] { "hechengqi", "hcq" });
            Dict.Add("导热质", new string[2] { "daorezhi", "drz" });
            Dict.Add("测试元素", new string[2] { "ceshiyuansu", "csys" });
            Dict.Add("污染泥", new string[2] { "wuranni", "wrn" });
            Dict.Add("污染土", new string[2] { "wurantu", "wrt" });
            Dict.Add("钨", new string[2] { "wu", "w" });
            Dict.Add("二硒化钨", new string[2] { "erxihuawu", "exhw" });
            Dict.Add("气态二硒化钨", new string[2] { "qitaierxihuawu", "qtexhw" });
            Dict.Add("气态钨", new string[2] { "qitaiwu", "qtw" });
            Dict.Add("中子质", new string[2] { "zhongzizhi", "zzz" });
            Dict.Add("铀矿", new string[2] { "youkuang", "yk" });
            Dict.Add("真空", new string[2] { "zhenkong", "zk" });
            Dict.Add("粘性凝胶流体", new string[2] { "zhanxingningjiaoliuti", "zxnjlt" });
            Dict.Add("虚空", new string[2] { "xukong", "xk" });
            Dict.Add("水", new string[2] { "shui", "s" });
            Dict.Add("黑钨矿", new string[2] { "heiwukuang", "hwk" });
            Dict.Add("黄饼", new string[2] { "huangbing", "hb" });

            return Dict;
        }

    }


}