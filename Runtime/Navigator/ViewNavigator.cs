using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIViews
{
    public class ViewNavigator : MonoBehaviour
    {
        /// <summary>
        /// The target UIDocument of this Navigator instance. 
        /// </summary>
        [field: SerializeField]
        public UIDocument Target { get; private set; }

        /// <summary>
        /// Contains the list of registered views.
        /// </summary>
        private List<UIScript> Views = new List<UIScript>();

        /// <summary>
        /// Displays the specified view on the Navigator's target document.
        /// </summary>
        /// <param name="viewID">The viewID to be switched in</param>
        public void ShowView(string viewID)
        {
            // Get the view assigned to the given ID
            var view = GetView(viewID);
            if (view != null)
            {
                view.ShowView();
            }
        }

        /// <summary>
        /// Registers a view to the navigator. Used internally for UIScripts to register themselves.
        /// </summary>
        /// <param name="view"></param>
        public void RegisterView(UIScript view)
        {
            if (view != null)
            {
                bool doesViewAlreadyExist = Views.Any(element => element.ID == view.ID);
                if (doesViewAlreadyExist == false)
                {
                    Views.Add(view);
                }
                else
                {
                    throw new Exception($"A view with the ID \"{view.ID}\" is already registered. Views must have unique IDs.");
                }
            }
        }

        /// <summary>
        /// Hides the specified view, and removes it from the hierarchy.
        /// </summary>
        /// <param name="viewID"></param>
        public void HideView(string viewID)
        {
            UIScript view = GetView(viewID);
            if (view != null)
            {
                view.HideView();
            }
        }

        /// <summary>
        /// Clears the specified container on the target document.
        /// </summary>
        /// <param name="containerID"></param>
        public void ClearContainer(string containerID)
        {
            VisualElement container = GetTargetContainer(containerID);
            if (container != null)
            {
                container.Clear();
            }
        }

        /// <summary>
        /// Gets the VisualElement with the given name.
        /// </summary>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public VisualElement GetTargetContainer(string containerID)
        {
            VisualElement targetContainer = Target.rootVisualElement.Q(containerID);
            if (targetContainer == null)
            {
                Debug.LogWarning($"ViewNavigator could not find container <color=blue>{containerID}</color> on the targeted document.");   
            }
            return targetContainer;
        }

        /// <summary>
        /// Gets the View with the given ID.
        /// </summary>
        /// <param name="viewID"></param>
        /// <returns></returns>
        private UIScript GetView(string viewID)
        {
            var viewData = Views.Find(view => view.ID == viewID);
            if (viewData == null)
            {
                Debug.LogWarning($"ViewNavigator could not find a registered view with the key <color=blue>{viewID}</color>.");
            }
            return viewData;
        }
    }
}