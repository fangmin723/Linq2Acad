﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Linq2AcDb
{
  class AcdbEnumerable<T> : IEnumerable<T>, IAcadEnumerableData where T : DBObject
  {
    private AcdbEnumerable(Lazy<Transaction> transaction, IEnumerable<ObjectId> ids)
    {
      Transaction = transaction;
      IDs = ids;
    }

    public bool IsEnumerating { get; private set; }

    public IEnumerable<ObjectId> IDs { get; private set; }

    public Lazy<Transaction> Transaction { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
      IsEnumerating = true;

      foreach (var id in IDs)
      {
        yield return (T)Transaction.Value.GetObject(id, OpenMode.ForRead);
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public static IEnumerable<T> Create(Lazy<Transaction> transaction, IEnumerable<ObjectId> ids, bool filter = false)
    {
      if (filter)
      {
        return new AcdbEnumerable<T>(transaction, EnumerateIds(ids.GetEnumerator(), o => (ObjectId)o, "AcDb" + typeof(T).Name));
      }
      else
      {
        return new AcdbEnumerable<T>(transaction, ids);
      }
    }

    public static IEnumerable<T> Create(Lazy<Transaction> transaction, ObjectId containerID, bool filter = false)
    {
      if (filter)
      {
        return new AcdbEnumerable<T>(transaction, EnumerateIds(transaction, containerID, o => (ObjectId)o, "AcDb" + typeof(T).Name));
      }
      else
      {
        return new AcdbEnumerable<T>(transaction, EnumerateIds(transaction, containerID, o => (ObjectId)o));
      }
    }

    public static IEnumerable<T> Create(Lazy<Transaction> transaction, ObjectId containerID, Func<object, ObjectId> getObjectID)
    {
      return new AcdbEnumerable<T>(transaction, EnumerateIds(transaction, containerID, getObjectID));
    }

    private static IEnumerable<ObjectId> EnumerateIds(Lazy<Transaction> transaction, ObjectId containerID, Func<object, ObjectId> getObjectID, string acDbType = null)
    {
      var container = (IEnumerable)transaction.Value.GetObject(containerID, OpenMode.ForRead);
      var idEnumerator = container.GetEnumerator();
      return EnumerateIds(idEnumerator, getObjectID, acDbType);
    }

    private static IEnumerable<ObjectId> EnumerateIds(IEnumerator idEnumerator, Func<object, ObjectId> getObjectID, string acDbType = null)
    {
      if (acDbType != null)
      {
        while (idEnumerator.MoveNext())
        {
          var id = getObjectID(idEnumerator.Current);

          if (id.ObjectClass.Name != acDbType)
          {
            continue;
          }

          yield return id;
        }
      }
      else
      {
        while (idEnumerator.MoveNext())
        {
          yield return getObjectID(idEnumerator.Current);
        }
      }
    }
  }
}