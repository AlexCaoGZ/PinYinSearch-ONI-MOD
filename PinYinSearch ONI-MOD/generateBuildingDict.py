import json
import fileinput
import re
from pypinyin import lazy_pinyin


class poData:
    '''
    poData类，里面保存了po文件的数据
    '''
    #msgctxt
    #msgid
    #msgstr
    
    def __init__(self, msgctxt:str="", msgid:list=[], msgstr:list=[]) -> None:
        '''
        poData的init
        '''
        self.msgctxt = msgctxt
        self.msgid = msgid
        self.msgstr = msgstr

    def assignID(self, ID:int)->None:
        self.ID = ID

    
    def isBuildingName(self) ->bool:
        '''
        判断这个物体是不是建筑
        返回true false
        '''
        if self.msgctxt.__contains__("STRINGS.BUILDINGS.PREFABS.") and self.msgctxt.__contains__(".NAME") and not self.msgctxt.__contains__("FACADES"):
            for text in self.msgstr:
                if text.__contains__("link"):
                    self.namePinYin=""
                    self.namePinYinInit=""
                    self.buildingName = re.search(r"\>(.*?)\<",self.msgstr[0]).group(1)
                    namePinYin = lazy_pinyin(self.buildingName,style=0)
                    namePinYinInit = lazy_pinyin(self.buildingName,style=4)
                    for item in namePinYin:
                        self.namePinYin = self.namePinYin + item
                    for item in namePinYinInit:
                        self.namePinYinInit = self.namePinYinInit + item
                    return True
        else:
            return False
        

def makePoData(textList:list=[]) -> poData:
    '''
    使用文本来生成一个poData类
    '''
    msgid = []
    msgstr= []
    msgctxt=""
    msgidFlag = False
    msgstrFlag = False
    for line in textList:
        #科雷的po文件太整蛊了，使用#.行来获取msgctxt
        if line.startswith("#."):
            msgctxt = line.split('#. ')[1]
        #msgctxt不要了
        elif line.startswith('msgctxt'):
            next
        elif line.startswith('msgid'):
            msgid.append(line.split('msgid ')[1])
            msgidFlag = True
        elif line.startswith('msgstr '):
            msgidFlag = False
            msgstrFlag = True
            msgstr.append(line.split('msgstr')[1])
        else:
            if msgidFlag == True:
                msgid.append(line)
            elif msgstrFlag == True:
                msgstr.append(line)
    return poData(msgctxt,msgid,msgstr)


def main():
    #读取po文件
    with open('strings_preinstalled_zh_klei.po',encoding='utf-8') as file:
        content = file.readlines()
    #注意headerFlag，在测试时我把po文件的header去除了
    #以后更新的话要恢复这个跳过header的设置
    tempList = []
    poDataList = []
    headerFlag = True
    #生成poDataBlock
    for line in content:
        line = line.strip()
        if not line.startswith(r"#~"):
            if len(line)!=0:
                tempList.append(line)
            else:
                if headerFlag == False:
                    headerFlag = True
                else:
                    poDataList.append(makePoData(tempList))
                tempList.clear()

    #给poData指定ID
    buildingList = []
    for i,poData in enumerate(poDataList):
        poData.assignID(i)
        if poData.isBuildingName():
            buildingList.append(poData)

    #科雷的po文件里面有大量重名，必须去除
    #上面的指定ID也是为了去除重名服务的
    for building1 in buildingList:
        for building2 in buildingList:
            if building1.buildingName == building2.buildingName and building1.ID != building2.ID:
                buildingList.remove(building1)

    #构建Dict.Add（）函数
    #Dict.Add("bb", new string[3] { "Mumbai", "London", "New York" });
    outputList = []
    for building in buildingList:
        temp = "Dict.Add(\""+building.buildingName+"\", new string[2] { \""+building.namePinYin+"\", \""+building.namePinYinInit+"\" });"
        outputList.append(temp)

    #输出
    with open('buildList.txt',encoding='utf-8',mode="w") as file:
        for line in outputList:
            file.write(line+'\n')

if __name__ == "__main__":
    main()