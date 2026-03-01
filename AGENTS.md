这是一个展示《原神》内置的卡牌游戏《七圣召唤》卡牌的GitHub pages项目，使用C#实现。

## 项目结构
- data文件夹是所使用到的数据文件，请勿修改
- GenshinDB.Tcg：卡牌数据结构
- GenshinTcgMarkdown：解析数据，输出为markdown的业务逻辑

## 目标功能
从`./data`文件夹中加载各版本七圣召唤卡牌数据。将可获得的卡牌（`obtainable=true`）转换为markdown，写入`./markdown/<类型>/<卡牌id>/<版本号>`中，其中类型为`characters`或`action_cards`。

一些卡牌会生成衍生物。这些衍生物需要递归地写入该卡牌的markdown中