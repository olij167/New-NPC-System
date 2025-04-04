using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBusinessInfo", menuName = "Business/BusinessInfo", order = 3)]
public class BusinessInfo : ScriptableObject
{
    [Header("Basic Information")]
    public string businessName;
    public Faction businessFaction;
    public Industry industry;
    public BusinessTemplate businessTemplate;

    [Header("Company Structure")]
    public List<FactionGenerator.CompanyPosition> positions = new List<FactionGenerator.CompanyPosition>();

    [Header("Financial Information")]
    public float capital;
    public float revenue;
    public float profitabilityMargin;

    public void Initialize(Faction faction, List<FactionGenerator.CompanyPosition> companyPositions, Industry selectedIndustry, BusinessTemplate selectedTemplate, float initialCapital, float profitMargin)
    {
        businessFaction = faction;
        positions = new List<FactionGenerator.CompanyPosition>(companyPositions);
        industry = selectedIndustry;
        businessTemplate = selectedTemplate;
        businessName = faction.factionName;
        capital = initialCapital;
        revenue = 0;
        profitabilityMargin = profitMargin;

        Debug.Log("[BusinessInfo] Initialized BusinessInfo for " + businessName + " with capital: " + initialCapital + " and profit margin: " + profitMargin);
    }

    public void SimulateEconomicEvent()
    {
        float eventRevenue = Random.Range(1000f, 10000f);
        revenue += eventRevenue;
        capital += eventRevenue;
        Debug.Log("[BusinessInfo] " + businessName + " generated revenue: " + eventRevenue + " (Total Revenue: " + revenue + ")");
    }
}
