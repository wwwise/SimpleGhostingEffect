

# SimpleGhostingEffect

基于Unity3D的模型拖尾重影特效插件，重影效果为3D渲染，不依赖任何PostEffect等后处理着色器，简单易用

## Demo


效果图

![Demo](https://github.com/wwwise/SimpleGhostingEffect/blob/master/doc/img/screenshot.jpg)

![Demo](https://github.com/wwwise/SimpleGhostingEffect/blob/master/doc/img/screenshot2.jpg)

## Usage

1.创建一个带 **Animator** 的角色对象

2.把 **SimpleGhosting** 脚本添加到角色对象上

3.可在inspector中设置脚本参数

![Inspector](https://github.com/wwwise/SimpleGhostingEffect/blob/master/doc/img/inspector.jpg)

* **参数说明**
    * **Color Over Timeline** 拖影的渐变颜色
    * **Life Time** 每块拖影的消失时间
    * **Max Ghosts Count** 拖影的数量长度
    * **Color Over Timeline** 拖影的渐变颜色
    * **Snap Distance** 生成下一个拖影的长度间距
    * **Snap Interval** 生成下一个拖影的时间间距
    * **Shader Name** 可选自定义着色器

4.使角色播放动画，运行时可通过脚本属性 **SimpleGhosting.snapping** = true或false开启或关闭拖影效果。
