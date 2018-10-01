using System.Linq;
using UnityEngine;
using UComponent = UnityEngine.Component;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods for UnityEngine.Component.
    /// </summary>
    public static class UComponentExtensions
    {
        #region Static Methods

        /// <summary>
        /// Enables or disables the colliders of all children and self if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="enabled"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetColliderRecursively(this UComponent component, bool enabled, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Collider>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.enabled = enabled);
        }

        /// <summary>
        /// Sets the isTrigger of all children and self collider if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="isTrigger"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetColliderIsTriggerRecursively(this UComponent component, bool isTrigger, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Collider>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.isTrigger = isTrigger);
        }

        /// <summary>
        /// Sets the layers of all children and self if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="layer"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetLayerRecursively(this UComponent component, int layer, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Transform>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.gameObject.layer = Mathf.Clamp(layer, 0, 32));
        }

        /// <summary>
        /// Enables or disables the renderers of all children and self if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="enabled"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetRendererRecursively(this UComponent component, bool enabled, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Renderer>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.enabled = enabled);
        }

        /// <summary>
        /// Sets the sorting orders of all children and self renderer if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetRendererSortingOrderRecursively(this UComponent component, int sortingOrder, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Renderer>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.sortingOrder = sortingOrder);
        }

        /// <summary>
        /// Sets the tags of all children and self if /includeSelf/.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="tag"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeInactive"></param>
        public static void SetTagRecursively(this UComponent component, string tag, bool includeSelf, bool includeInactive = false)
        {
            component.GetComponentsInChildren<Transform>(includeInactive).Where(x => ((x.transform.Equals(component.transform)) ? (includeSelf) : (true))).Select(x => x.gameObject.tag = tag);
        }

        /// <summary>
        /// Gets the collision mask of this /component/.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static int PhysicsCollisionMask(this UComponent component)
        {
            return ULayerMaskExtensions.GetPhysicsCollisionMask(component.gameObject.layer);
        }

        /// <summary>
        /// Returns the full name of gameObject.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static string FullName(this UComponent component)
        {
            return ((component.transform.parent == null) ? (component.name) : (component.transform.parent.FullName() + "." + component.name));
        }

        /// <summary>
        /// Creates a child gameObject.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Transform AddChild(this UComponent component)
        {
            GameObject child = new GameObject();

            child.transform.position = component.transform.position;
            child.transform.rotation = component.transform.rotation;

            child.transform.parent = component.transform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;

            return child.transform;
        }

        /// <summary>
        /// Creates a parent gameObject.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Transform AddParent(this UComponent component)
        {
            GameObject parent = new GameObject();

            parent.transform.position = component.transform.position;
            parent.transform.rotation = component.transform.rotation;

            if (component.transform.parent != null)
                parent.transform.parent = component.transform.parent;

            component.transform.parent = parent.transform;
            component.transform.localPosition = Vector3.zero;
            component.transform.localRotation = Quaternion.identity;

            return parent.transform;
        }

        /// <summary>
        /// Returns an existing component of T or a new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this UComponent component) where T : UComponent
        {
            T result = component.GetComponent<T>();

            if (result == null)
                result = component.gameObject.AddComponent<T>();

            return result;
        }

        #endregion Static Methods
    }
}