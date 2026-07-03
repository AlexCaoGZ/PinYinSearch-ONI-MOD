 # PinYinSearch ONI-MOD

 为《缺氧》（Oxygen Not Included）添加拼音搜索功能的 Mod。输入拼音首字母或全拼就能搜到对应的建筑、物品、元素。

 ## 功能

 - **建筑栏搜索** — 在建造菜单里用拼音搜建筑
 - **物品筛选面板** — Move'It 等筛选器里用拼音搜物品
 - **全局资源面板** — 星球总资源列表支持拼音过滤
 - **筛选器分类搜索** — 固体/液体/气体筛选器的搜索栏也支持拼音
 - **下调搜索阈值** — 原版的模糊匹配分数阈值较高，中文拼音匹配容易不命中，已调整为更宽松的阈值
 - **离线字典 + 即时生成双保险** — 常用词优先查预计算字典，未收录的中文词自动调用 TinyPinyin 即时转换

 ## 使用方法

 在游戏任意搜索框中输入中文对应的 **拼音全拼** 或 **拼音首字母** 即可：

 | 想搜的目标 | 输入 |
 |-----------|------|
 | 卫生间 | `wsj` |
 | 氧气扩散机 | `yqk` |
 | 隔热管道 | `grgd` |
 | 污染土 | `wrt` |
 | 塑料 | `sl` |

 ### 实际效果

 在建造菜单的搜索框输入 `yqk`或`yangqikuo`，即可搜到「氧气扩散机」。

 > 原版《缺氧》按中文名搜索时不会匹配拼音（只匹配汉字的 Unicode 文本），这个 Mod 通过 Harmony Patch 为每个搜索入口注入拼音关键词来补齐这一能力。

 ## Credits

 - [TinyPinyin](https://github.com/promeG/TinyPinyin) — C# 中文转拼音库
 - [pypinyin](https://github.com/mozillazg/python-pypinyin) — Python 拼音库（字典生成用）
 - [Harmony](https://github.com/pardeike/Harmony) — .NET 运行时补丁库
 - Klei Entertainment — 缺氧本体及开放的 Mod 接口

 ## License

 本项目遵循 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件。
