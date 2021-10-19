小目标：当蹲下以及滑铲的时候，需要同步改变Collider的高度
大目标：贴墙跑

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
