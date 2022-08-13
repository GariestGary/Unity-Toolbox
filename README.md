# Unity Toolbox
Set of architectural solutions for Unity
## Table of contents
* [Basics](#basics)
* [Update System](#update-system)
* [Dependency Injection](#dependency-injection)
* [Message System](#message-system)
* [Object Pooling](#object-pooling)
* [Save System](#save-system)
* [Travel System](#travel-system)
* [BeauRoutine](#beauroutine)

## Basics

All starts with scene named "MAIN", just open it from `Plugins/Toolbox/Scenes/MAIN.unity` and add to build settings. Scene has main [ENTRY] object, which contains all necessary components. 

From ENTRY component you can directly change global time scale, set initial scene name, which opens at game start and `On Load Event` that calls from unity's `Start()` method.

Toolbox based on using singleton classes of each tool. You can specify your own singletons by using this syntax: `public class MySingleton: Singleton<MySingleton>`

## Update System

Update system works as a wrapper for built-in unity's update system, but has some improvements, such as:
* Separate time scale for each object
* Updating certain objects with specified interval
* Update modifiers, which modifies object's delta time depending on the specified conditions

To use all power of update system, just derive your desired class from `MonoCached` and there you go!

All objects from scene initialized after the scene is loaded. To manually add object to update use `Updater.InitializeMono(desiredObject)`, it automatically picks up all `MonoCached` components from object, initializes them, and adds to process.

Updater initializes objects by this way:
1. Injects all necessary dependencies into object (see [Dependency Injection](#dependency-injection))
2. Gets all `MonoCached` components from object and it's children, and calls `Rise()` method on each
3. Then calls `Ready()` method on each, in case you want to use post initialize logic
4. Adds each `MonoCached` to process list, and calls `Activate()` method on each

There are several ways you can add or remove object from update:
* `Updater.InitializeObject(GameObject obj)` - Injects object, intializes it, and adds to process list
* `Updater.InitializeMono(MonoCached mono)` - Injects just component, initializes it, and adds to process
* `Updater.AddMonoToProcess(MonoCached mono)` - Just adds mono to process without initializing it
* `Updater.RemoveMonoFromProcess(MonoCached mono)` - Use it if you want to fully remove `MonoCached` from process. Calls `Deactivate()` and `OnRemove()` methods in that order
* `Updater.RemoveObjectFromUpdate(GameObject obj)` - Calls `Updater.RemoveMonoFromProcess(MonoCached mono)` for each `MonoCached` component in object
* `Updater.RemoveObjectsFromUpdate(GameObject[] objs)` - Does the same as `Updater.RemoveObjectFromUpdate(GameObject obj)`, but for each GameObject in array

You can use update modifiers to customize `MonoCached` time behaviour, depending on the specified conditions (e.g. if object enters some area that freezes all objects in it, you want to slow down object by reducing it's own delta time)

To add new modifier, just create new class and derive it from `UpdateModifier` class. It has `Initialize()` method to do some initial logic, and abstract method `Modify(float delta, MonoCached mono)`,
which has initial delta value and it's owner `MonoCached` class to do some checks. You must return modified delta value to apply changes.

You also can set update interval in `MonoCahed` component, just use `mono.Interval = someValue` and it will update with specified interval.

Use `Pause()` and `Resume()` methods to pause and resume it's updating accordingly

## Dependency Injection

Dependency injection allows you to specify some global instances of your classes and initialize them in other classes by just adding `[Inject]` attribute to it's field.

By default you already can inject following classes:
* Updater
* Traveler
* Resolver
* Pooler
* Messager
* Saver

### Example
```C#
using VolumeBox.Toolbox;

public class Test: MonoCached
{
  [Inject] private Messager msg;
  
  public override void Rise()
  {
    msg.Send(Message.IT_JUST_WORKS); //you can use `Messager` instance, without clearly initialize it in `Test` class
  }
}
```

All magic happens in `Resolver` class

## Message System



## Object Pooling



## Save System



## Travel System
