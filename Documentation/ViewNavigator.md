# ViewNavigator

This is the controller class that the framework builds on. Built on `Monobehaviour`, it provides access to views.


## Target
```c#
public UIDocument Target { get; private set; }
```

The target `UIDocument` that the views will be spawned into.


### RegisterView(UIView view)
```c#
public void RegisterView(UIView view)
```
Used internally by UIViews to register themselves. Internally, they are stored in a `List<UIView>`. It is not recommended to use this method.


### ShowView(string viewID)
```c#
public void ShowView(string viewID)
```
Displays the view on the [target](#Target)'s hierarchy.

It simply calls the specified View's [ShowView()](./UIScript.md#ShowView()).


### HideView(string viewID)
```c#
public void HideView(string viewID)
```
Hides the view, and removes it from the hierarchy.

It simply calls the specified View's [HideView()](./UIScript.md#HideView()).


### GetTargetContainer(string containerID)
```c#
public VisualElement GetTargetContainer(string containerID)
```
Gets the VisualElement with the specified ID. Shorthand convenience method for `visualElement.Q<>()`.

If the specified VisualElement could not be found, logs an error to the console.