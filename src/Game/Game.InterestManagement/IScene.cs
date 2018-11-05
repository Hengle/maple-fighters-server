﻿namespace Game.InterestManagement
{
    /// <summary>
    /// Represents the scene which has the regions.
    /// </summary>
    public interface IScene
    {
        /// <summary>
        /// Gets the access to the scene regions.
        /// </summary>
        IMatrixRegion MatrixRegion { get; }

        /// <summary>
        /// Adds a new scene object to the scene.
        /// </summary>
        /// <param name="sceneObject">The new scene object.</param>
        void AddSceneObject(ISceneObject sceneObject);

        /// <summary>
        /// Removes the scene object from the scene.
        /// </summary>
        /// <param name="sceneObject">The scene object.</param>
        void RemoveSceneObject(ISceneObject sceneObject);
    }
}