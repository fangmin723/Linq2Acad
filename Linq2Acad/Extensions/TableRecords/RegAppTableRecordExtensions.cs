﻿using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad
{
  public static class RegAppTableRecordExtensions
  {
    public static bool IsValidName(this IEnumerable<RegAppTableRecord> source, string name, bool allowVerticalBar)
    {
      return TableHelpers.IsValidName(name, allowVerticalBar);
    }

    public static RegAppTableRecord GetItem(this IEnumerable<RegAppTableRecord> source, string name)
    {
      return TableHelpers.GetItem<RegAppTableRecord, RegAppTable>(source, rat => rat[name]);
    }

    public static bool Contains(this IEnumerable<RegAppTableRecord> source, string name)
    {
      return TableHelpers.Contains<RegAppTableRecord, RegAppTable>(source, rat => rat.Has(name));
    }

    public static bool Contains(this IEnumerable<RegAppTableRecord> source, ObjectId id)
    {
      return TableHelpers.Contains<RegAppTableRecord, RegAppTable>(source, rat => rat.Has(id));
    }

    public static ObjectId Add(this IEnumerable<RegAppTableRecord> source, RegAppTableRecord item)
    {
      return TableHelpers.Add<RegAppTableRecord, RegAppTable>(source, item);
    }

    public static IEnumerable<ObjectId> Add(this IEnumerable<RegAppTableRecord> source, IEnumerable<RegAppTableRecord> items)
    {
      return TableHelpers.AddRange<RegAppTableRecord, RegAppTable>(source, items);
    }
  }
}