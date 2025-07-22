using System;
using UnityEngine;

public class DetailsController : TextWindowController
{
    protected override Func<UnitData, RectTransform> ObjectToInstantiate => static unitData => unitData.DetailsPrefab;
}
