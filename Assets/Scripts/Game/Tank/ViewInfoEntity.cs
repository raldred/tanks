// Unity Framework
using UnityEngine;

public enum ViewInfoEntityType
{
    Tank,
    NonDestructableObject,
    DestructableObject
}

public class ViewInfoEntity
{
    // Type of entity
    public ViewInfoEntityType entityType;
     
    // Position of the entity
    public Vector3 position;
    
    // Face direction of the object (Vector3.zero if has not forward)
    public Vector3 forward;
}
