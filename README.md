# ParkourGame
### Game Type
First-person parkour and shooting in a 3D world. <br>

---

### Gameplay
Through a variety of parkour skills to reach the end, and there will be battles with NPCs on the way. <br>
What we are trying to do: <br>
- There will be 2 parts - parkour part and battle part. However player can use parkour skill to win the battle.
- We will make multiple levels with different difficulty settings.
- We consider making all the maps manually, or randomly generating pre-made map components and stitching them together.

---

### Key Arrangement/Operation
|Action|Key Arrangement/Operation|Instrustion|
|-|-|-|
|Walk Forward|W|-|
|Walk Left|A|-|
|Walk Backward|S|-|
|Walk Right|D|-|
|Jump|Space|-|
|Run|WAD + Shift|Cannot run backward|
|Crouch|C|-|
|Crouch Walk|WASD + C|-|
|Slide|WAD + Shift + C|Press **Crouch** when running <br> Turning the mouse during sliding does not change the player's direction of moving|
|Wall Run Left|WA + Shift|Only in effect when: <br> Running <br> Not on the ground <br> Keeping the moving can make the speed of the fall significantly reduced <br> **Considering whether to change to automatic running when the wall**|
|Wall Run Right|WD + Shift|Only in effect when: <br> Running <br> Not on the ground <br> Keeping the moving can make the speed of the fall significantly reduced <br> **Considering whether to change to automatic running when the wall**|
|Fire|Left mouse click||
|Bullet Time|Right mouse click (Hold)||

---

### Development Process
The current decision is to use rigibody, but also to keep the character controller.
>**Note: Italicized are optional objectives that can be dropped if they cannot be completed or are deemed inappropriate or unnecessary.**
- #### Major Sprint Goals.
  1. AI part.
     - AI should have a patrol function.
     - AI should know where the player was last seen and search after noticing the player disappear.
     - ... TBC

- #### Minor Sprint Goals.
   1. The movement on the slope needs to be improved, the current player movement on the slope is the same as the movement on flat ground, now want to be able to let the player in a running state down the slope, can be a **acceleration process** slide down.
   2. _Make a head bob for jumping._
   3. Make progress bars for the corresponding skills.
   4. Make health bar.

---

### C# Scripts
|Script|Instruction|
|-|-|
|PlayerMove.cs|**Core Script** <br> Player basic movement on the ground <br> Contain methods that check Player current stats|
|MoveSway.cs|**Core Script** <br> Implement the effect of camera shake during all player movement states (if it has)|
|MouseLook.cs|**Core Script** <br> Allows the player to turn the view with the mouse and move directly in front of the view by default|
|WallRun.cs|**Core Script** <br> Allows players to run on the wall|
|ChangeCollider.cs|**Core Script** <br> Change the player's Collider to fit the player's movement|
|Shooting.cs|**Core Script** <br> Allow player to shoot|
|FSMRifleman|**Core Script** <br> Enemy AI Implementation using Finite State Machine|
|AttackSingleState.cs|**Core Script** <br> Attacking State of AI|
|DieState|**Core Script** <br> Die State of AI|
|FSMState|**Core Script** <br> FSM State of AI|
|IdleState|**Core Script** <br> Idle State of AI|
|ModelState|**Core Script** <br> Template of further Finite State of AI|
|Other|For instruction of other None Core Scripts, please wait for further updating|


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
|SwitchWeaponPlace.cs|改变持枪姿势以符合各种情况|

---

### 已完成目标
- ~~完成基本的控制，建立第一人称控制，玩家具备基本的走路，蹲下，跑步等动作~~ - (Runge Huang 10/14/2021)
- ~~完成走路，跑步，蹲走时的镜头晃动效果~~ - (Runge Huang 10/16/2021)
- ~~完成滑铲以及相应的镜头效果~~ - (Runge Huang 10/17/2021)
- ~~改善视觉效果，使得玩家移动以及变动视角的时候物体不会疯狂抖动~~ - (Runge Huang 10/19/2021)
- ~~使得玩家可以在斜面上正常移动~~ - (Runge Huang 10/20/2021)
- ~~撞墙时停止镜头晃动~~ - (Runge Huang 10/20/2021)
- ~~在玩家运动状态改变时，相应的改变Collider~~ - (Runge Huang 10/20/2021)
- ~~完成贴墙跑~~ - (Runge Huang 10/21/2021)
- ~~完成空中勾索~~ - (Runge Huang 11/05/2021)
- ~~完成墙壁勾索~~ - (Runge Huang 11/07/2021)
- ~~完成子弹时间~~ - (Runge Huang 11/20/2021)
- ~~完成枪械开火~~ - (Yiquan Xiao 11/26/2021)
- ~~完成游戏菜单~~ - (Runge Huang & Xingmeng Wang)
- ~~完成基础UI~~ - (Runge Huang)
- ~~完成教程关卡~~ - (Runge Huang)
- ~~实现血量（玩家&敌人）及相关功能~~ - (Yiquan Xiao)
- ~~完善UI，加入时间及评分系统~~ - (Yiquan Xiao)
- 完成第一关(最后一关？) - (Runge Huang)

