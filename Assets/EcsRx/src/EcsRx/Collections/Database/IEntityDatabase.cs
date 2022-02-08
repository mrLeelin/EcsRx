﻿using System;
using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Collections.Database
{
    public static class EntityCollectionLookups
    {
        public const int NoCollectionDefined = -1;
        public const int DefaultCollectionId = 0;
    }
    
    /// <summary>
    /// This acts as the database to store all entities, rather than containing all entities directly
    /// within itself, it partitions them into collections which can contain differing amounts of entities.
    /// </summary>
    public interface IEntityDatabase : INotifyingEntityCollection, IDisposable
    {
        /// <summary>
        /// All the entity collections that the manager contains
        /// </summary>
        IReadOnlyList<IEntityCollection> Collections { get; }
        
        /// <summary>
        /// Fired when a collection has been added
        /// </summary>
        IObservable<IEntityCollection> CollectionAdded { get; }
        
        /// <summary>
        /// Fired when a collection has been removed
        /// </summary>
        IObservable<IEntityCollection> CollectionRemoved { get; }

        /// <summary>
        /// Gets an enumerable collection of entities for you to iterate through,
        /// it will by default search across ALL collections within the manager unless constrained.
        /// This is not cached and will always query the live data.
        /// </summary>
        /// <remarks>
        /// So in most cases an IObservableGroup is a better option to use for repeat queries as it internally
        /// will update a maintained list of entities without having to enumerate the entire collection/s.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <param name="collectionId">The optional collection name to use (defaults to null)</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        IEnumerable<IEntity> GetEntitiesFor(IGroup group, int collectionId = EntityCollectionLookups.NoCollectionDefined);
        
        /// <summary>
        /// Creates a new collection within the database
        /// </summary>
        /// <remarks>
        /// This is primarily useful for when you want to isolate certain entities, such as short lived ones which would
        /// be constantly being destroyed and recreated, like bullets etc. In most cases you will probably not need more than 1.
        /// </remarks>
        /// <param name="id">The name to give the collection</param>
        /// <returns>A newly created collection with that name</returns>
        IEntityCollection CreateCollection(int id);
        
        /// <summary>
        /// Adds an existing collection within the database
        /// </summary>
        /// <remarks>
        /// This is mainly used for when you have persisted a collection and want to re-load it
        /// </remarks>
        /// <param name="collection">The collection to add</param>
        void AddCollection(IEntityCollection collection);
        
        /// <summary>
        /// Gets a collection by name from within the manager, if no name is provided the default pool is returned
        /// </summary>
        /// <param name="id">The optional name of collection to return</param>
        /// <returns>The located collection</returns>
        IEntityCollection GetCollection(int id = EntityCollectionLookups.DefaultCollectionId);
        
        /// <summary>
        /// Removes a collection from the manager
        /// </summary>
        /// <param name="id">The collection to remove</param>
        /// <param name="disposeEntities">if the entities should all be disposed too</param>
        void RemoveCollection(int id, bool disposeEntities = true);

        IEnumerable<IEntity> GetEntitiesFor(LookupGroup lookupGroup, params int[] collectionIds);
        IEnumerable<IEntity> GetEntitiesFor(IGroup group, params int[] collectionIds);
    }
}