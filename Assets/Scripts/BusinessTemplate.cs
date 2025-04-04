using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BusinessTemplate
{
    [Tooltip("Name of the business template (e.g., Clothing Shop, Department Store, Grocery Store).")]
    public string templateName;

    [Tooltip("Description of the business template.")]
    [TextArea]
    public string description;

    [Tooltip("Default job positions relevant to this business template.")]
    public List<FactionGenerator.CompanyPosition> defaultPositions = new List<FactionGenerator.CompanyPosition>();
}
