using System;
using UnityEngine;

public class TerminologyController : TextWindowController
{
    protected override Func<UnitData, RectTransform> ObjectToInstantiate => static unitData => unitData.TerminologyPrefab;
}
