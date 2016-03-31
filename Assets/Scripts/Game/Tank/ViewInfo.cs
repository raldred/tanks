// Mono Framework
using System.Collections.Generic;

public class ViewInfo
{
    // List of entities
    public List<ViewInfoEntity> visibleEntities;
    
    // Add the tank information to the visibleEntities list
    public void AddTankInfo(Tank tank)
    {
        if (visibleEntities == null) visibleEntities = new List<ViewInfoEntity>();
        
        ViewInfoEntity vie = new ViewInfoEntity();
        
        vie.entityType = ViewInfoEntityType.Tank;
        vie.position = tank.transform.position;
        vie.forward = tank.transform.forward;
        
    }
}
