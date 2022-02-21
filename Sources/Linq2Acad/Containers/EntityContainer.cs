﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Linq2Acad
{
  /// <summary>
  /// Represents a container that holds Entity objects.
  /// </summary>
  public class EntityContainer : ContainerEnumerableBase<Entity>
  {
    internal EntityContainer(Database database, Transaction transaction, ObjectId containerID)
      : base(database, transaction, containerID, i => (ObjectId)i)
    {
    }

    /// <summary>
    /// The ObjectId of the container.
    /// </summary>
    public ObjectId ObjectId
      => ID;

    /// <summary>
    /// Adds an Entity to the container.
    /// </summary>
    /// <param name="entity">The Entity to be added.</param>
    /// <param name="setDatabaseDefaults">True, if the database defaults should be set.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when parameter  <i>entity</i> is null.</exception>
    /// <exception cref="System.Exception">Thrown when the given Entity belongs to another block or an AutoCAD error occurs.</exception>
    /// <returns>The ObjectId of the Entity that was added.</returns>
    public ObjectId Add(Entity entity, bool setDatabaseDefaults = false)
    {
      Require.ParameterNotNull(entity, nameof(entity));
      Require.NewlyCreated(entity, nameof(entity));

      return AddInternal(new[] { entity }, setDatabaseDefaults).First();
    }

    /// <summary>
    /// Adds Entities to the container.
    /// </summary>
    /// <param name="entities">The Entities to be added.</param>
    /// <param name="setDatabaseDefaults">True, if the database defaults should be set.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when parameter  <i>entities</i> is null.</exception>
    /// <exception cref="System.Exception">Thrown when the an Entity belongs to another block or an AutoCAD error occurs.</exception>
    /// <returns>The ObjectIds of the Entities that were added.</returns>
    public IEnumerable<ObjectId> AddRange(IEnumerable<Entity> entities, bool setDatabaseDefaults = false)
    {
      Require.ParameterNotNull(entities, nameof(entities));
      Require.ElementsNotNull(entities, nameof(entities));

      foreach (var entity in entities)
      {
        Require.NewlyCreated(entity, nameof(entity));
      }

      return AddInternal(entities, setDatabaseDefaults);
    }

    /// <summary>
    /// Adds Entities to the container.
    /// </summary>
    /// <param name="items">The Entities to be added.</param>
    /// <param name="setDatabaseDefaults">True, if the database defaults should be set.</param>
    /// <returns>The ObjectIds of the Entities that were added.</returns>
    private IEnumerable<ObjectId> AddInternal(IEnumerable<Entity> items, bool setDatabaseDefaults)
    {
      var btr = (BlockTableRecord)transaction.GetObject(ID, OpenMode.ForWrite);
      return items.Select(i =>
                          {
                            if (setDatabaseDefaults)
                            {
                              i.SetDatabaseDefaults();
                            }

                            var id = btr.AppendEntity(i);
                            transaction.AddNewlyCreatedDBObject(i, true);
                            return id;
                          });
    }

    /// <summary>
    /// Removes all Entities from this container.
    /// </summary>
    /// <exception cref="System.Exception">Thrown when an AutoCAD error occurs.</exception>
    public void Clear()
    {
      foreach (var entity in this.UpgradeOpen())
      {
        entity.Erase();
      }
    }
  }
}