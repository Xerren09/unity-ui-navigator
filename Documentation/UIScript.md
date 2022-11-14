# UIView

This is the main class that the framework builds on. Built on `Monobehaviour`, it provides the core functionality for Views.

While most of the fields here have a `protected` setter, generally it is not recommended to change their values in code, as it can easily break functionality.

## Navigator
```c#
public ViewNavigator Navigator { get; private set; }
```

The ViewNavigator instance this script is hooked up to. On `Awake`, the script will automatically register itself (See [RegisterView](./ViewNavigator.md#RegisterView(UIView view)) for more).


### ID
```c#
public string ID { get; private set; }
```
The ID of the View this script controls.

This must be unique, as it will be used to refer to this specific view.


### WrapperVisualElementName
```c#
public string WrapperVisualElementName
```
The name of the VisualElement this view will be wrapped into when displayed.

When views are spawned, they are always wrapped into a default `VisualElement`, with the name `UIView__{ID}`.


### UXMLDocument
```c#
public VisualTreeAsset UXMLDocument { get; private set; }
```
The UXML Document file containing the view to be shown.


### IsStatic
```c#
public bool IsStatic { get; protected set; } = false;
```
Sets whether or not the view is static. If set to `true`, [UIUpdate()](#UIUpdate()) will never run. Defaults to false.


### IsViewActive
```c#
public bool IsViewActive { get; protected set; } = false;
```
The current state of the view.

Indicates wether or not the view is currently visible.

As `UIUpdate()` is meant to run active UI code, and the value of this field affects dependency handling, manually changing the value of this is dangerous. 


### Dependency
```c#
public UIView Dependency { get; protected set; }
```
The dependency of this view, which must be present before this can be loaded. If none is specified, the view will be spawned in the Navigator target's root at the specified `ContainerID`.

An important note here, is that dependency chains are supported. If a referenced view also has a dependency (and so on), the chain will be traversed and every necessary View will be displayed in order. This also means that only the topmost View needs to be manually called, and every other one will be automatically handled. Already active views in the dependency chain will not be called.


### ContainerID
```c#
public string ContainerID { get; protected set; }
```
The ID of the view container in the target document. This view will be attached to the VisualElement with this name as a child.

The value of this is is set from a custom dropdown menu in the inspector, whose values are loaded dynamically based on the dependency settings. The list of possible values contain the names of all named VisualElements ___either___ in the `ViewNavigator`'s base target document (if no dependency is selected), ___or___ in the selected dependency's set document.


### GetViewContainer()
```c#
public VisualElement GetViewContainer()
```
Gets the view's base VisualElement (wrapper element).


### ShowView()
```c#
public virtual void ShowView()
```
Displays the view on the attached Navigator's target.

Views are loaded into the [targeted VisualElement](#ContainerID), regardless if it already has children.

Although not meant to be overridden, custom implementations may be necessary when creating reusable UI elements.

### HideView()
```c#
public virtual void HideView()
```
Hides the view, and removes it from the hierarchy.

Views are removed from the [targeted VisualElement](#ContainerID) individually, preserving other children.

Although not meant to be overridden, custom implementations may be necessary when creating reusable UI elements.

### GetDependencyList()
```c#
public List<UIView> GetDependencyList()
```
Compiles the dependency stack of this view into a list.

This list contains the entire dependency chain, until a dependency is `null`, with the current view's dependency being the first in the list.

## Control methods

### UIUpdate()
```c#
protected virtual void UIUpdate()
```
Identical to Unity's `Update`, except it only runs when [IsStatic](#IsStatic) is set to `false` and [IsViewActive](#IsViewActive) is `true`.

This overridable method is meant to be used in custom scripts as the general UI update loop, to ensure the UI code is only ran when the referenced objects actually exist.


### OnEnterFocus()
```c#
protected virtual void OnEnterFocus()
```
Runs when the view enters focus (Created).

This overridable method is meant to be used in custom scripts as an initializer (essentially like Unity's `Start`). Getting the references of `VisualElement`s safely, as at this point they are already available.


### OnLeaveFocus()
```c#
protected virtual void OnLeaveFocus()
```
Runs when the view leaves focus (Destroyed).

This overridable method is meant to be used in custom scripts as a "finalizer". Any cleanup should be performed here, before the UI elements are destroyed.

Note that since the View's `VisualElement`s are destroyed when they leave the main hierarchy, removing event subscriptions to any of said elements is not necessary.


## Custom Inspector
UIViews default to a custom inspector, that allows for design time setup. To override it, simply define a new custom inspector for the newly creates custom View.

Important to note that the inspector code uses reflection to set the property values. Since these are autoproperies, they have automatically generated backing fields, which _can_ break if Unity's naming scheme ever changes (although unlikely to happen).