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

		visibleEntities.Add(vie);
    }

	// Add the obstacle information to the visibleEntities list
	public void AddObstacleInfo(MapObstacle obs)
	{
		if (visibleEntities == null) visibleEntities = new List<ViewInfoEntity>();
        
        ViewInfoEntity vie = new ViewInfoEntity();

        vie.entityType = (obs.isBreakable ? ViewInfoEntityType.DestructableObject : ViewInfoEntityType.NonDestructableObject);
		vie.position = obs.transform.position;
		vie.forward = obs.transform.forward;

		visibleEntities.Add(vie);
	}

	// Returns the number of tanks in the view
	public int GetTankCount()
	{
		int num = 0;

		for (int i=0; i<visibleEntities.Count; i++)
		{
			if (visibleEntities[i].entityType == ViewInfoEntityType.Tank) num++;
		}

		return num;
	}

	public int GetEntityCount()
	{
		return visibleEntities.Count;
	}
}
