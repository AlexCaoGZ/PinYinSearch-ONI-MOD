import re
import polib
from pypinyin import lazy_pinyin, Style

def main():
    # 配置文件路径
    PO_FILE_PATH = r"strings_preinstalled_zh_klei.po"
    OUTPUT_FILE_PATH = r"buildList.txt"

    print(f"正在加载 PO 文件: {PO_FILE_PATH} ...")
    try:
        po = polib.pofile(PO_FILE_PATH)
    except Exception as e:
        print(f"读取 PO 文件失败，请确保已经安装 polib (pip install polib)，且文件存在。错误: {e}")
        return

    # 使用字典来去重，键为中文名，值为 (全拼, 首字母)
    unique_items = {}

    print("正在解析并提取建筑和元素名称...")
    for entry in po:
        ctxt = entry.msgctxt or ""
        
        # 判断是否是建筑的名称 (排除皮肤 FACADES)
        is_building = "STRINGS.BUILDINGS.PREFABS." in ctxt and ".NAME" in ctxt and "FACADES" not in ctxt
        # 判断是否是元素的名称
        is_element = "STRINGS.ELEMENTS." in ctxt and ".NAME" in ctxt

        if is_building or is_element:
            msgstr = entry.msgstr
            if not msgstr:
                continue

            # 提取包含在 <link=...>名字</link> 中的真实名字
            match = re.search(r"\>(.*?)\<", msgstr)
            if match:
                item_name = match.group(1)
            else:
                # 如果没有 link 标签，则直接清理两端空白
                item_name = msgstr.strip()

            if not item_name:
                continue

            # 如果这个名字还没有被处理过，则生成拼音
            if item_name not in unique_items:
                # 生成全拼，不带声调
                full_pinyin_list = lazy_pinyin(item_name, style=Style.NORMAL)
                full_pinyin = "".join(full_pinyin_list)

                # 生成首字母
                initials_list = lazy_pinyin(item_name, style=Style.FIRST_LETTER)
                initials = "".join(initials_list)

                unique_items[item_name] = (full_pinyin, initials)

    print(f"共提取到 {len(unique_items)} 个独立词条。正在生成 C# 字典代码...")

    # 构建 C# 代码行
    output_lines = []
    for name, (full_py, init_py) in unique_items.items():
        # 转义名字里可能存在的双引号（以防万一）
        safe_name = name.replace('"', '\\"')
        
        # 按照需求生成 C# 字典 Add 语句
        line = f'Dict.Add("{safe_name}", new string[] {{ "{full_py}", "{init_py}" }});'
        output_lines.append(line)

    # 写入文件
    try:
        with open(OUTPUT_FILE_PATH, "w", encoding="utf-8") as f:
            for line in output_lines:
                f.write(line + "\n")
        print(f"生成完毕！代码已成功写入到 {OUTPUT_FILE_PATH} 🚀")
    except Exception as e:
        print(f"写入文件失败: {e}")

if __name__ == "__main__":
    main()
