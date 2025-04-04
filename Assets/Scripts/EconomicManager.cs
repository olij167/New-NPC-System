using System.Collections.Generic;
using UnityEngine;

public class EconomicManager : MonoBehaviour
{
    public List<BusinessInfo> allBusinesses = new List<BusinessInfo>();

    void Update()
    {
        // Simulate economic events roughly every 5 seconds (assuming 60 FPS).
        if (Time.frameCount % 300 == 0)
        {
            foreach (BusinessInfo business in allBusinesses)
            {
                business.SimulateEconomicEvent();
                Debug.Log("[EconomicManager] Business " + business.businessName + " now has revenue: " + business.revenue);
            }
        }
    }

    public void RegisterBusiness(BusinessInfo business)
    {
        if (!allBusinesses.Contains(business))
        {
            allBusinesses.Add(business);
            Debug.Log("[EconomicManager] Registered business: " + business.businessName);
        }
    }
}
