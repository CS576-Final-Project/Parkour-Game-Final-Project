# 跑酷游戏（魔女-机器人混合版）
### 游戏类型
在3D世界中的第一人称跑酷，射击。 <br>

---

### 游戏玩法
通过各种跑酷的技巧到达终点，中间会发生与NPC的战斗。 <br>
可能会有一个BOSS在后方追逐玩家（待定） <br>

---

### 键位/操作
|动作|键位/操作|说明|
|-|-|-|
|向前走|W|-|
|向左走|A|-|
|向后走|S|-|
|向右走|D|-|
|跳跃|Space|-|
|跑步|WAD + Shift|不能向后跑|
|蹲下|C|-|
|蹲走|WASD + C|-|
|滑铲|WAD + Shift + C|跑步的时候按C <br> 滑铲期间转动鼠标不会改变玩家行进方向|
|左贴墙跑|WA + Shift|保持奔跑状态时才能生效 <br> 贴墙跑仅在空中判定生效 <br> 保持运动可以使坠落的速度大幅降低 <br> 视角必须保持平行于或略向墙一侧|
|右贴墙跑|WD + Shift|保持奔跑状态时才能生效 <br> 贴墙跑仅在空中判定生效 <br> 保持运动可以使坠落的速度大幅降低 <br> 视角必须保持平行于或略向墙一侧|
|武器开火|鼠标左键|**该功能尚未完成**|
|发射钩锁|**待定**|**该功能尚未完成**|

---

### 开发进程
目前决定用 rigibody, 但也保留 character controller。
>**注： 斜体为可选目标，如 无法完成 或 认为不合适 或 认为没必要 可以放弃。**
- #### 当前主要冲刺目标
  1. 为玩家制作钩锁，钩锁应当具备三种工作模式：<br>
     - >**注：** 以下所有的高低均以 **Y轴坐标** 为基准。
     - 如果目标是 **比自身坐标高很多** 的位置，那么玩家将以钩锁目标为中心进行摆荡。<br>
        - 目前的想法是设置一个专用摆荡点，当中心视角位于该点附近时，钩锁将自动链接至该点。<br>
        - **该功能只能在空中使用**。
     - 如果目标是 **比自身低** 或 **同样高** 或 **高一丢丢** 且 **不能是自己所处于的地面** 的位置，那么玩家将利用钩锁将自己拉到目标位置。
        - 目前的想法是可以将目标设定为符合条件的任一点。
        - 另一个可行的方案是设置专用的墙体用于该模式。
        - **该功能可以在地面和空中使用**。
     - _如果目标是 **敌方NPC** ，将利用钩锁将自己拉至敌方NPC的位置并击杀该NPC。_
  2. 敌方NPC应当开始制作，使用rigibody，具备基本移动功能。
  3. 玩家的攻击功能应当开始制作。

- #### 当前次要冲刺目标
   1. 斜坡上的移动需要改进，当前玩家在斜坡上的移动与在平地上的移动一样，现在希望能够让玩家在下坡时，可以以一个 **加速的过程** 滑下去并在终点依然可以以之前的速度滑出。
   2. 重力在下落时的表现有些奇怪，需要找到较为合适的参数/调整方法。
   3. _可以为跳跃的过程添加一个镜头晃动。_
   4. 为该README同步制作一个英文版。

---

### 脚本文件
|文件|说明|
|-|-|
|PlayerMove.cs|**核心文件** <br> 控制玩家在平地上的基本运动 <br> 拥有相关的method用于判定玩家当前状态|
|MoveSway.cs|**核心文件** <br> 实现玩家所有运动状态时的镜头晃动效果（如有）|
|MouseLook.cs|**核心文件** <br> 允许玩家用鼠标转动视角，并默认向视角正前方移动|
|WallRun.cs|**核心文件** <br> 允许玩家实现贴墙跑|
|ChangeCollider.cs|**核心文件** <br> 改变玩家的Collider以贴合玩家的运动状态|
|AdjustFOVWhenSliding.cs|在玩家滑铲的过程中调整相机FOV以增强速度感|
|MoveMouse.cs|为了消除玩家运动时观察物体会乱抖的BUG而创建 <br> 将摄像机安插至头部的位置 <br> 让摄像机能够按照头部的晃动完成镜头晃动|
|SimulationOfHead.cs|为了消除玩家运动时观察物体会乱抖的BUG而创建 <br> 让摄像机能够按照头部的晃动完成镜头晃动|

---

### 已完成目标
- ~~完成基本的控制，建立第一人称控制，玩家具备基本的走路，蹲下，跑步等动作~~ - （Runge Huang 10/14/2021）
- ~~完成走路，跑步，蹲走时的镜头晃动效果~~ - （Runge Huang 10/16/2021）
- ~~完成滑铲以及相应的镜头效果~~ -（Runge Huang 10/17/2021）
- ~~改善视觉效果，使得玩家移动以及变动视角的时候物体不会疯狂抖动~~ - （Runge Huang 10/19/2021）
- ~~使得玩家可以在斜面上正常移动~~ -（Runge Huang 10/20/2021）
- ~~撞墙时停止镜头晃动~~ -（Runge Huang 10/20/2021）
- ~~在玩家运动状态改变时，相应的改变Collider~~ -（Runge Huang 10/20/2021）
- ~~完成贴墙跑~~ -（Runge Huang 10/21/2021）
