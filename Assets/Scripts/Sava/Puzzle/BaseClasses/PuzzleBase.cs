using UnityEngine;
using System;
public class PuzzleBase : Singleton<PuzzleBase>
{
    public virtual void RotateNode() { }
    public virtual void SwithNode() { }
    public virtual void CheckCompletion() { }
    public virtual void GenerateNodesUI() { }
    public virtual void CreateGrid(string definition, Action onComplete = null) { }
}
