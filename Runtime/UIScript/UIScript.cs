using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace UIViews
{
    /// <summary>
    /// Used to create <see cref="ViewNavigator"/> views.
    /// </summary>
    public class UIScript : MonoBehaviour
    {
        /// <summary>
        /// The ViewNavigator instance this script is hooked up to.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public ViewNavigator Navigator { get; private set; }

        /// <summary>
        /// The ID of the View this script controls.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public string ID { get; private set; }

        /// <summary>
        /// The name of the VisualElement this view will be wrapped into when displayed.
        /// </summary>
        public string WrapperVisualElementName
        {
            get
            {
                return $"UIView__{ID}";
            }
        }

        /// <summary>
        /// The UXML Document file containing the view to be shown.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public VisualTreeAsset UXMLDocument { get; private set; }

        /// <summary>
        /// Sets whether or not the view is static. If set to <see langword="true"/>, <see cref="UIUpdate()"/> will never run. Default is <see langword="false"/>.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public bool IsStatic { get; protected set; } = false;

        /// <summary>
        /// The current state of the view.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public bool IsViewActive { get; protected set; } = false;

        /// <summary>
        /// The dependency of this view, which must be present before this can be loaded. If none is specified, the view will be spawned in the Navigator target's root.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public UIScript Dependency { get; private set; }

        /// <summary>
        /// The ID of the view container in the target document. 
        /// This view will be attached to the VisualElement with this name as a child.
        /// </summary>
        [field: SerializeField]
        [field: HideInInspector]
        public string ContainerID { get; protected set; }

        /// <summary>
        /// Gets the view's base VisualElement (wrapper element).
        /// </summary>
        /// <returns></returns>
        public VisualElement GetViewContainer()
        {
            // Grab visual element via Navigator
            return Navigator.GetTargetContainer(WrapperVisualElementName);
        }

        /// <summary>
        /// Displays the view on the attached Navigator's target.
        /// </summary>
        public virtual void ShowView()
        {
            if (UXMLDocument != null)
            {
                // Only run if the view is not active yet
                if (IsViewActive == false)
                {
                    if (ContainerID != null && ContainerID.Length != 0)
                    {
                        // Spawn dependencies
                        SetDependenciesActive();

                        // Get target container
                        VisualElement viewTargetContainer = Navigator.GetTargetContainer(ContainerID);
                        if (viewTargetContainer != null)
                        {
                            // Clear the view container
                            viewTargetContainer.Clear();

                            VisualElement viewWrapperContainer = new VisualElement();
                            UXMLDocument.CloneTree(viewWrapperContainer);
                            viewWrapperContainer.name = WrapperVisualElementName;

                            // The normal event syntax is more useful here than the Unity one, so lambda it 

                            // OnEnterFocus
                            viewWrapperContainer.RegisterCallback<AttachToPanelEvent>(evt => {
                                OnEnterFocus();
                                IsViewActive = true;
                            });

                            // OnLeaveFocus
                            viewWrapperContainer.RegisterCallback<DetachFromPanelEvent>(evt => {
                                OnLeaveFocus();
                                IsViewActive = false;
                            });

                            viewTargetContainer.Add(viewWrapperContainer);
                        }
                    }
                    else
                    {
                        throw new Exception($"View must have a container ID set so it can be shown.");
                    }
                }
            }
        }

        /// <summary>
        /// Hides the view, and removes it from the hierarchy.
        /// </summary>
        public virtual void HideView()
        {
            if (IsViewActive == true)
            {
                VisualElement viewContainer = GetViewContainer();
                if (viewContainer != null)
                {
                    // Remove the view container from its parent
                    VisualElement viewParent = viewContainer.parent;
                    if (viewParent != null)
                    {
                        viewParent.Remove(viewContainer);
                    }
                }
            }
        }

        /// <summary>
        /// Compiles the dependency stack of this view into a list.
        /// </summary>
        /// <returns></returns>
        public List<UIScript> GetDependencyList()
        {
            List<UIScript> dependencies = new List<UIScript>();
            UIScript dependency = Dependency;
            while (dependency != null)
            {
                dependencies.Add(dependency);
                dependency = dependency.Dependency;
            }
            return dependencies;
        }

        /// <summary>
        /// Runs through the dependency stack and calls every inactive inluded view's <see cref="ShowView()"/>.
        /// </summary>
        private void SetDependenciesActive()
        {
            var deps = GetDependencyList();
            // Reverse the dependency list so the lowest dependency is first
            deps.Reverse();
            if (deps.Count > 0)
            {
                foreach (UIScript dependency in deps)
                {
                    // Check if the dependency is not active
                    if (dependency.IsViewActive == false)
                    {
                        dependency.ShowView();
                    }
                }
            }
        }

        private void Awake()
        {
            if (Navigator != null)
            {
                if (ID != null && ID.Length != 0)
                {
                    Navigator.RegisterView(this);
                }
                else
                {
                    throw new NullReferenceException($"A view must have an valid ID.");
                }
                
            }
            else
            {
                throw new NullReferenceException($"View {ID} on {this.gameObject.name} does not have a UI Navigator instance assigned.");
            }
        }

        private void Update()
        {
            // Don't run updates if the view is marked as static
            if (IsStatic == false)
            {
                // Don't run updates if the view is not active
                if (IsViewActive)
                {
                    UIUpdate();
                }
            }
        }

        /// <summary>
        /// Identical to <see cref="Update"/>, except it only runs when <see cref="IsStatic"/> is set to <see langword="false"/> and <see cref="IsViewActive"/> is <see langword="true"/>.
        /// </summary>
        protected virtual void UIUpdate()
        {

        }

        /// <summary>
        /// Runs when the view enters focus (Created).
        /// </summary>
        protected virtual void OnEnterFocus()
        {

        }

        /// <summary>
        /// Runs when the view leaves focus (Destroyed).
        /// </summary>
        protected virtual void OnLeaveFocus()
        {

        }
    }
}