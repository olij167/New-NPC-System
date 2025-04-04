using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Industry
{
    [Tooltip("Name of the industry (e.g., Retail, Technology, Finance).")]
    public string industryName;

    [Tooltip("List of business templates (subcategories) for this industry.")]
    public List<BusinessTemplate> businessTemplates = new List<BusinessTemplate>();
}
