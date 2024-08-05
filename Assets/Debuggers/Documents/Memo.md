# Memo
## キャラクター
### 衝突（Collider,Collition）
キャラクターの衝突判定について、```KinematicCharacterMotor.SetCapsuleCollisionsActivation()```や```  KinematicCharacterMotor.SetCollisionSolvingActivation()``` から設定が可能。以下はドキュメントの引用。

Walkthrough.pdf / 17p
```
Now for the implementation of the state’s logic. The most important thing is what happens in OnStateEnter and OnStateExit. Here
we control whether or not the character’s custom collision detection code will be bypassed or not with the
KinematicCharacterMotor.SetCapsuleCollisionsActivation() and KinematicCharacterMotor.SetCollisionSolvingActivation() methods.
When SetCollisionSolvingActivation is set to false, no collision solving is done whatsoever, and the character can go through
anything. SetCapsuleCollisionsActivation controls the activation of the kinematic capsule collider itself.
```

### 地形に沿った移動・スナップ（Terrain,Snap）
キャラクターの地形に沿った移動（スナップ）について、```KinematicCharacterMotor.SetCapsuleCollisionsActivation()```で設定可能。

Walkthrough.pdf / 18p
```
Finally, we need to make sure the character doesn’t try to snap to the ground while swimming. For this, we will use
KinematicCharacterMotor.SetStabilitySolvingActivation(). Setting this to false will skip all ground probing/snapping logic.
```