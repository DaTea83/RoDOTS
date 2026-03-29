# Changelog

## [0.1.11] - 2026-03-28/29

### Added
* Added AgentSpawnISingleton
* HelperCollection, added GetDistanceAndDot() extensions
* Added GameObjectAuthoring
* Added InitializeAnimatorAuthoring
* Added AnimatorTransformICleanup
* Added AgentStatsAuthoring

### Changed
* AgentSpawnISystem, now tracks how many agents are spawned with AgentSpawnISingleton
* AgentMoveISystem, now before moving towards the target agent it first rotate towards the target

### Removed
* Removed the IDispoable from ObjTransformIData, destroying gameobject will be handled by ICleanupComponent
* Removed AnimationController
* Removed AgentMoveISingleton

## [0.1.10] - 2026-03-27

### Added
* Added AnimationController
* Added FlatPlaneAuthoring.prefab

### Changed
* ObjTransformIData, now implements IDisposable, the gameobject will be destroyed when the entity is destroyed
* (Whether EntityTransformIData should also implement IDispoable is on the radar)
* Renamed AgentMoveIData to AgentMoveIEnableable and implemented IEnableableComponent

## [0.1.9] - 2026-03-25/26

### Added
* Added Runtime/Obsolete
* Added AgentMoveIData
* Added AgentMoveISystem
* Added AgentMoveNodeAuthoring
* Added AgentSpawnNodeAuthoring
* Added AgentMoveSystemAuthoring
* Added InitializeAgentMoveISystem
* Added UrpRandomColorAuthoring
* Two new overload for RandomValue()

### Changed
* Current Obsolete and ObsoleteV2 folder are now moved into Runtime/Obsolete
* Old Obsolete folder now renamed to ObsoleteV1 
* All scripts in Entities/Agents moved to ObsoleteV2 and marked obsolete
* BasicProperties renamed to BasicAttributes
* AgentScriptable renamed to AgentAttributes

### Deleted
* HelperCollection, deleted PositionRotationBlob and EntityBlob
* Deleted DemoAgent prefab

## [0.1.8] - 2026-03-24

### Changed
* Done a coding style cleanup
* GenericOverlayUIManager, TObj field moved from serializable to script itself 

## [0.1.7] - 2026-03-20

After testing I noticed that the GenericUIManager goes against on how previously UI was setup,

and the discovery of UI toolkit changes how overlay UI are setuped.

Any overlay UI from now on will gonna use the new UI Toolkit and world space canvas will be handled by GenericUIManager.

### Added
* Added GenericOverlayUIManager
* Added InitialDestroyFollowerISystem, this adds DestroyIEnableableTag to all EntityTransformIData
* EnumCollection, added EVisualElements
* UiCollection, added GetVisualElement()

### Changed
* GenericUIManager changed name to GenericWorldUIManager
* EntityFollowerISystem, Entities with EntityTransformIData will now be destroyed when the game object is null

### Removed
* Removed GenericEventManager
* CameraController, removed IsCameraReady and RunFadeScreen()
* CameraController, removed blackScreenImg and initialFadeOutTime

## [0.1.6] - 2026-03-18

### Added
* Added back the test folder
* GenericSingleton, now comes with its own get ECS.World (GetWorld())
* GenericSingleton, added GetSingletonEntity<TComponent>()
* GenericSingleton, added regions for ECS related and Async related
* GenericUIManager, poolCount is now ignored, spawn[] size is forced to 1, extra loop is removed
* GenericUIManager, now add a toggle that spawn a PhysicsCollider on the UI element
* GenericUIManager, added ECS region
* UiHelper, added TransformRect validation
* UiHelper, added id for tag
* Added Entities/UI folder
* Added UIData
* Added UiHandleSystemBase

### Removed
* GenericSingleton, removed KeepSingleton() and all child classes that using the said method

## [0.1.5] - 2026-03-16

### Fixed
* Fixed GenericAudioManager always return -1 when using GetPoolIndex()(Added an override);

### Added
* Added SpawnDelayEntityAuthoring

## Removed
* Removed old folders saves in FolderModificationData.json

### Changed
* GenericPoolingManager, field "initializeOnStart" and "collectionCheck" changed to abstract properties

## [0.1.4] - 2026-03-15

### Added

* Added GenericPoolingManager
* Added new GenericAudioManager, GenericParticleManager and GenericUIManager that inherit from GenericPoolingManager
* Added GenericSpawnManager
* HelperCollection, added "RandomValue2" and "RandomValue3"

### Changed
* Moved legacy GenericAudioManager, GenericParticleManager and GenericUIManager to ObsoleteV2 folder and marked [Obsolete]
* HelperCollection, all "RandomValue" with GameObject parameter changed to Component instead

## [0.1.3] - 2026-03-14

### Changed
* Reformatted all coding styles (no changes in functionality)

## [0.1.2] - 2026-03-13

### Added
* Added RemoveMissingScriptsEditor
* Added EditorUtils

### Changed
* Moved EditorBackgroundColor from LoadIconDisplayEditor to EditorUtils

## [0.1.1] - 2026-03-13

### Fixed
* LoadIconDisplayEditor now do extra checks when the gameobject has missing scripts

### Changed
* Renamed LoadIconDisplay to LoadIconDisplayEditor
* Renamed AnimationRecorder to AnimationRecorderEditor
* Moved CameraControllerEditor, CameraTagFollowerEditor, DestroyEntityEditor and FlatPlaneEditor to new folder called Component Editor
* Static Stuff, changed all "!=" to "is not" in CallStaticMethod, CallGenericInstanceMethod and CallInstanceMethod
