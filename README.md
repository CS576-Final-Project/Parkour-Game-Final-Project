# **<font size=6><font color=808080>跑酷游戏（魔女-机器人混合版）</font></font>**
## **<font size=5>游戏类型</font>**
<font size=3> 在3D世界中的第一人称跑酷，射击。</font> <br>

---

## **<font size=5>游戏玩法</font>**
<font size=3>通过各种跑酷的技巧到达终点，中间会发生与NPC的战斗。</font> <br>
<font size=3>可能会有一个BOSS在后方追逐玩家（待定）</font> <br>

---

## **<font size=5>开发日志</font>**
- ### **当前主要冲刺目标**
  1. 为玩家制作钩锁，使其可以



小目标：
1.当蹲下以及滑铲的时候，需要同步改变Collider的高度 - 已完成
2.当检测到前方撞墙的时候，取消移动的晃动 - 细节问题不做也行 - 已完成
3.找个空把这个README换成Markdown格式
4.代码注释完善

大目标：
1.贴墙跑 - 已初步完成
2.制作钩锁，让玩家可以摆荡

可选要修复的BUG：滑铲速度在不同屏幕size下表现差异巨大，修不掉就算了

目前搞定了：
站立静止
站立走
蹲下静止
蹲下走
跳
滑铲

以及以上所有相应的镜头晃动（如果有的话）

键位设置：
走 WASD
跑 WAS + Shift (不能向后跑)
跳 空格
蹲 C
蹲走 C + WASD
滑铲 跑的时候按C

目前决定用 rigibody, 但也保留 character controller

AdjustFOVWhenSliding => 在滑铲的过程中创造拉伸镜头的效果
MouseLook => 用鼠标控制视角
PlayerMove => 控制玩家的运动部分，包括速度，移动方向以及判定玩家当前的动作状态
MoveSway => 控制运动时的晃动
ChangeCollider => 改变Collider的高度以匹配玩家的运动状态
WallRun => 贴墙跑

///以下文件只为了消除玩家运动时观察物体会乱抖的bug而创建///
SimulationOfHead
MoveMouse
原理简单的来说就是把相机摘出来然后用Empty Object占住原来的位置当替身
