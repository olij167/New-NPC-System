using System.Collections.Generic;
using UnityEngine;

public class BusinessInfoGenerator : MonoBehaviour
{
    [Header("References")]
    public FactionGenerator factionGenerator;
    public EconomicManager economicManager;
    public FactionManager factionManager;
    public NPCManager npcManager;

    [Header("Industry Settings")]
    public List<Industry> industries = new List<Industry>();

    [Header("Default Business Positions (Optional)")]
    public List<FactionGenerator.CompanyPosition> defaultPositions;

    [Header("Financial Settings")]
    public float initialCapital = 50000f;
    public float minProfitMargin = 0.05f;
    public float maxProfitMargin = 0.20f;

    public BusinessInfo GenerateBusinessInfo()
    {
        // If industries list is empty, initialize default industries.
        if (industries == null || industries.Count == 0)
        {
            InitializeDefaultIndustries();
        }
        if (industries == null || industries.Count == 0)
        {
            Debug.LogError("[BusinessInfoGenerator] No industries available even after initialization.");
            return null;
        }

        // Do not clear all factions here; assume community factions are preserved.
        // Generate the business faction using FactionGenerator.
        Faction businessFaction = factionGenerator.GenerateBusinessFaction();

        // Select an Industry from the list.
        Industry selectedIndustry = industries[Random.Range(0, industries.Count)];

        // Select a BusinessTemplate from the chosen Industry.
        BusinessTemplate selectedTemplate = null;
        if (selectedIndustry.businessTemplates != null && selectedIndustry.businessTemplates.Count > 0)
        {
            selectedTemplate = selectedIndustry.businessTemplates[Random.Range(0, selectedIndustry.businessTemplates.Count)];
        }
        else
        {
            Debug.LogWarning("[BusinessInfoGenerator] No BusinessTemplates found in Industry: " + selectedIndustry.industryName + ". Default positions will be used.");
        }

        // Generate a business name; optionally append template info.
        string businessName = factionGenerator.GenerateBusinessName();
        if (selectedTemplate != null)
        {
            businessName += " (" + selectedTemplate.templateName + ")";
        }
        businessFaction.factionName = businessName;
        businessFaction.description = "A business faction named " + businessName + " in the " + selectedIndustry.industryName + " industry.";
        // Note: FactionGenerator.GenerateBusinessFaction already adds this faction to factionManager.allFactions.
        Debug.Log("[BusinessInfoGenerator] Business faction created: " + businessFaction.factionName);

        // Determine company positions.
        List<FactionGenerator.CompanyPosition> finalPositions = new List<FactionGenerator.CompanyPosition>();
        if (selectedTemplate != null && selectedTemplate.defaultPositions != null && selectedTemplate.defaultPositions.Count > 0)
        {
            foreach (FactionGenerator.CompanyPosition pos in selectedTemplate.defaultPositions)
            {
                FactionGenerator.CompanyPosition newPos = new FactionGenerator.CompanyPosition();
                newPos.positionName = pos.positionName;
                newPos.requiredSkillName = pos.requiredSkillName;
                newPos.requiredSkillLevel = pos.requiredSkillLevel;
                finalPositions.Add(newPos);
            }
        }
        else
        {
            if (defaultPositions != null && defaultPositions.Count > 0)
            {
                foreach (FactionGenerator.CompanyPosition pos in defaultPositions)
                {
                    FactionGenerator.CompanyPosition newPos = new FactionGenerator.CompanyPosition();
                    newPos.positionName = pos.positionName;
                    newPos.requiredSkillName = pos.requiredSkillName;
                    newPos.requiredSkillLevel = pos.requiredSkillLevel;
                    finalPositions.Add(newPos);
                }
            }
            else
            {
                // Fallback hard-coded defaults.
                FactionGenerator.CompanyPosition boss = new FactionGenerator.CompanyPosition();
                boss.positionName = "Boss";
                boss.requiredSkillName = "Charisma";
                boss.requiredSkillLevel = 70f;
                finalPositions.Add(boss);

                FactionGenerator.CompanyPosition manager = new FactionGenerator.CompanyPosition();
                manager.positionName = "Manager";
                manager.requiredSkillName = "Logic";
                manager.requiredSkillLevel = 60f;
                finalPositions.Add(manager);

                FactionGenerator.CompanyPosition worker = new FactionGenerator.CompanyPosition();
                worker.positionName = "Worker";
                worker.requiredSkillName = "Craftsmanship"; // Standardized name.
                worker.requiredSkillLevel = 50f;
                finalPositions.Add(worker);

                FactionGenerator.CompanyPosition apprentice = new FactionGenerator.CompanyPosition();
                apprentice.positionName = "Apprentice";
                apprentice.requiredSkillName = "Athletics";
                apprentice.requiredSkillLevel = 30f;
                finalPositions.Add(apprentice);
            }
        }

        Debug.Log("[BusinessInfoGenerator] Business faction '" + businessFaction.factionName + "' generated with positions.");

        float profitMargin = Random.Range(minProfitMargin, maxProfitMargin);
        BusinessInfo newBusiness = ScriptableObject.CreateInstance<BusinessInfo>();
        newBusiness.Initialize(businessFaction, finalPositions, selectedIndustry, selectedTemplate, initialCapital, profitMargin);
        if (economicManager != null)
        {
            economicManager.RegisterBusiness(newBusiness);
        }
        else
        {
            Debug.LogWarning("[BusinessInfoGenerator] EconomicManager not assigned.");
        }

        // Use the centralized OccupationAssignmentManager for candidate selection.
        if (OccupationAssignmentManager.Instance != null)
        {
            OccupationAssignmentManager.Instance.AssignOccupations(newBusiness);
        }
        else
        {
            Debug.LogWarning("[BusinessInfoGenerator] OccupationAssignmentManager not found.");
        }

        return newBusiness;
    }

    private void AddPositionToTemplate(BusinessTemplate template, string positionName, string requiredSkillName, float requiredSkillLevel)
    {
        FactionGenerator.CompanyPosition pos = new FactionGenerator.CompanyPosition();
        pos.positionName = positionName;
        pos.requiredSkillName = requiredSkillName;
        pos.requiredSkillLevel = requiredSkillLevel;
        template.defaultPositions.Add(pos);
    }
    private void InitializeDefaultIndustries()
    {
        industries = new List<Industry>();

        // ---- Retail Industry ----
        Industry retailIndustry = new Industry();
        retailIndustry.industryName = "Retail";
        retailIndustry.businessTemplates = new List<BusinessTemplate>();

        // Boutique Clothing Store
        BusinessTemplate boutiqueTemplate = new BusinessTemplate();
        boutiqueTemplate.templateName = "Boutique Clothing Store";
        boutiqueTemplate.description = "An upscale boutique for designer clothing and accessories.";
        boutiqueTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(boutiqueTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(boutiqueTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(boutiqueTemplate, "Worker", "Craftsmanship", 50f);
        AddPositionToTemplate(boutiqueTemplate, "Apprentice", "Athletics", 30f);
        retailIndustry.businessTemplates.Add(boutiqueTemplate);

        // Supermarket
        BusinessTemplate supermarketTemplate = new BusinessTemplate();
        supermarketTemplate.templateName = "Supermarket";
        supermarketTemplate.description = "A large store offering a wide range of groceries and household items.";
        supermarketTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(supermarketTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(supermarketTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(supermarketTemplate, "Cashier", "Linguistics", 50f);
        AddPositionToTemplate(supermarketTemplate, "Worker", "Craftsmanship", 50f);
        retailIndustry.businessTemplates.Add(supermarketTemplate);

        // Convenience Store
        BusinessTemplate convenienceTemplate = new BusinessTemplate();
        convenienceTemplate.templateName = "Convenience Store";
        convenienceTemplate.description = "A small store offering quick-stop essentials.";
        convenienceTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(convenienceTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(convenienceTemplate, "Worker", "Craftsmanship", 45f);
        retailIndustry.businessTemplates.Add(convenienceTemplate);

        // Electronics Outlet
        BusinessTemplate electronicsTemplate = new BusinessTemplate();
        electronicsTemplate.templateName = "Electronics Outlet";
        electronicsTemplate.description = "A store specializing in consumer electronics and gadgets.";
        electronicsTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(electronicsTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(electronicsTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(electronicsTemplate, "Technician", "Mathematics", 55f);
        AddPositionToTemplate(electronicsTemplate, "Worker", "Craftsmanship", 50f);
        retailIndustry.businessTemplates.Add(electronicsTemplate);

        // Bookstore
        BusinessTemplate bookstoreTemplate = new BusinessTemplate();
        bookstoreTemplate.templateName = "Bookstore";
        bookstoreTemplate.description = "A quaint store offering books and literary merchandise.";
        bookstoreTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(bookstoreTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(bookstoreTemplate, "Worker", "Linguistics", 50f);
        AddPositionToTemplate(bookstoreTemplate, "Apprentice", "Athletics", 30f);
        retailIndustry.businessTemplates.Add(bookstoreTemplate);

        industries.Add(retailIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + retailIndustry.industryName);

        // ---- Technology Industry ----
        Industry techIndustry = new Industry();
        techIndustry.industryName = "Technology";
        techIndustry.businessTemplates = new List<BusinessTemplate>();

        // Software Company
        BusinessTemplate softwareTemplate = new BusinessTemplate();
        softwareTemplate.templateName = "Software Company";
        softwareTemplate.description = "A company focused on developing and selling software solutions.";
        softwareTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(softwareTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(softwareTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(softwareTemplate, "Developer", "Mathematics", 60f);
        AddPositionToTemplate(softwareTemplate, "Tester", "Linguistics", 50f);
        techIndustry.businessTemplates.Add(softwareTemplate);

        // IT Consultancy
        BusinessTemplate consultancyTemplate = new BusinessTemplate();
        consultancyTemplate.templateName = "IT Consultancy";
        consultancyTemplate.description = "A firm providing expert IT consulting services.";
        consultancyTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(consultancyTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(consultancyTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(consultancyTemplate, "Consultant", "Logic", 55f);
        techIndustry.businessTemplates.Add(consultancyTemplate);

        // Hardware Store
        BusinessTemplate hardwareTemplate = new BusinessTemplate();
        hardwareTemplate.templateName = "Hardware Store";
        hardwareTemplate.description = "A retailer of computer and electronic hardware.";
        hardwareTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(hardwareTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(hardwareTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(hardwareTemplate, "Worker", "Craftsmanship", 50f);
        techIndustry.businessTemplates.Add(hardwareTemplate);

        // Gadget Outlet
        BusinessTemplate gadgetTemplate = new BusinessTemplate();
        gadgetTemplate.templateName = "Gadget Outlet";
        gadgetTemplate.description = "A store specializing in the latest gadgets and accessories.";
        gadgetTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(gadgetTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(gadgetTemplate, "Sales Associate", "Charisma", 50f);
        techIndustry.businessTemplates.Add(gadgetTemplate);

        // Tech Startup
        BusinessTemplate startupTemplate = new BusinessTemplate();
        startupTemplate.templateName = "Tech Startup";
        startupTemplate.description = "A small, innovative company developing cutting-edge technology.";
        startupTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(startupTemplate, "Founder", "Charisma", 75f);
        AddPositionToTemplate(startupTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(startupTemplate, "Developer", "Mathematics", 60f);
        techIndustry.businessTemplates.Add(startupTemplate);

        industries.Add(techIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + techIndustry.industryName);

        // ---- Finance Industry ----
        Industry financeIndustry = new Industry();
        financeIndustry.industryName = "Finance";
        financeIndustry.businessTemplates = new List<BusinessTemplate>();

        // Bank
        BusinessTemplate bankTemplate = new BusinessTemplate();
        bankTemplate.templateName = "Bank";
        bankTemplate.description = "A traditional bank offering financial services.";
        bankTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(bankTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(bankTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(bankTemplate, "Teller", "Linguistics", 50f);
        AddPositionToTemplate(bankTemplate, "Worker", "Craftsmanship", 50f);
        financeIndustry.businessTemplates.Add(bankTemplate);

        // Investment Firm
        BusinessTemplate investmentTemplate = new BusinessTemplate();
        investmentTemplate.templateName = "Investment Firm";
        investmentTemplate.description = "A company specializing in investment management.";
        investmentTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(investmentTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(investmentTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(investmentTemplate, "Analyst", "Mathematics", 60f);
        financeIndustry.businessTemplates.Add(investmentTemplate);

        // Insurance Agency
        BusinessTemplate insuranceTemplate = new BusinessTemplate();
        insuranceTemplate.templateName = "Insurance Agency";
        insuranceTemplate.description = "A firm providing various insurance products.";
        insuranceTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(insuranceTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(insuranceTemplate, "Agent", "Charisma", 55f);
        financeIndustry.businessTemplates.Add(insuranceTemplate);

        // Credit Union
        BusinessTemplate creditTemplate = new BusinessTemplate();
        creditTemplate.templateName = "Credit Union";
        creditTemplate.description = "A member-owned financial cooperative.";
        creditTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(creditTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(creditTemplate, "Teller", "Linguistics", 50f);
        financeIndustry.businessTemplates.Add(creditTemplate);

        // Accounting Firm
        BusinessTemplate accountingTemplate = new BusinessTemplate();
        accountingTemplate.templateName = "Accounting Firm";
        accountingTemplate.description = "A firm offering accounting and financial advisory services.";
        accountingTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(accountingTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(accountingTemplate, "Accountant", "Mathematics", 55f);
        financeIndustry.businessTemplates.Add(accountingTemplate);
        industries.Add(financeIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + financeIndustry.industryName);

        // ---- Healthcare Industry ----
        Industry healthcareIndustry = new Industry();
        healthcareIndustry.industryName = "Healthcare";
        healthcareIndustry.businessTemplates = new List<BusinessTemplate>();

        // Hospital
        BusinessTemplate hospitalTemplate = new BusinessTemplate();
        hospitalTemplate.templateName = "Hospital";
        hospitalTemplate.description = "A full-service hospital offering a wide range of medical services.";
        hospitalTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(hospitalTemplate, "Chief Medical Officer", "Charisma", 75f);
        AddPositionToTemplate(hospitalTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(hospitalTemplate, "Nurse", "Linguistics", 50f);
        AddPositionToTemplate(hospitalTemplate, "Technician", "Mathematics", 55f);
        healthcareIndustry.businessTemplates.Add(hospitalTemplate);

        // Clinic
        BusinessTemplate clinicTemplate = new BusinessTemplate();
        clinicTemplate.templateName = "Clinic";
        clinicTemplate.description = "A smaller facility offering outpatient care and consultations.";
        clinicTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(clinicTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(clinicTemplate, "Doctor", "Charisma", 70f);
        AddPositionToTemplate(clinicTemplate, "Nurse", "Linguistics", 50f);
        healthcareIndustry.businessTemplates.Add(clinicTemplate);

        // Pharmacy
        BusinessTemplate pharmacyTemplate = new BusinessTemplate();
        pharmacyTemplate.templateName = "Pharmacy";
        pharmacyTemplate.description = "A retail pharmacy providing prescription and over-the-counter medications.";
        pharmacyTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(pharmacyTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(pharmacyTemplate, "Pharmacist", "Mathematics", 65f);
        AddPositionToTemplate(pharmacyTemplate, "Worker", "Craftsmanship", 45f);
        healthcareIndustry.businessTemplates.Add(pharmacyTemplate);

        // Diagnostic Center
        BusinessTemplate diagnosticTemplate = new BusinessTemplate();
        diagnosticTemplate.templateName = "Diagnostic Center";
        diagnosticTemplate.description = "A center offering medical imaging and diagnostic tests.";
        diagnosticTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(diagnosticTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(diagnosticTemplate, "Technician", "Mathematics", 55f);
        healthcareIndustry.businessTemplates.Add(diagnosticTemplate);
        industries.Add(healthcareIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + healthcareIndustry.industryName);

        // ---- Hospitality Industry ----
        Industry hospitalityIndustry = new Industry();
        hospitalityIndustry.industryName = "Hospitality";
        hospitalityIndustry.businessTemplates = new List<BusinessTemplate>();

        // Hotel
        BusinessTemplate hotelTemplate = new BusinessTemplate();
        hotelTemplate.templateName = "Hotel";
        hotelTemplate.description = "A full-service hotel offering accommodation and amenities.";
        hotelTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(hotelTemplate, "General Manager", "Charisma", 75f);
        AddPositionToTemplate(hotelTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(hotelTemplate, "Receptionist", "Linguistics", 50f);
        AddPositionToTemplate(hotelTemplate, "Worker", "Craftsmanship", 50f);
        hospitalityIndustry.businessTemplates.Add(hotelTemplate);

        // Restaurant
        BusinessTemplate restaurantTemplate = new BusinessTemplate();
        restaurantTemplate.templateName = "Restaurant";
        restaurantTemplate.description = "A dining establishment offering a variety of cuisines.";
        restaurantTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(restaurantTemplate, "Chef", "Creativity", 70f);
        AddPositionToTemplate(restaurantTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(restaurantTemplate, "Server", "Charisma", 50f);
        hospitalityIndustry.businessTemplates.Add(restaurantTemplate);

        // Cafe
        BusinessTemplate cafeTemplate = new BusinessTemplate();
        cafeTemplate.templateName = "Cafe";
        cafeTemplate.description = "A cozy cafe serving coffee and light snacks.";
        cafeTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(cafeTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(cafeTemplate, "Barista", "Charisma", 50f);
        hospitalityIndustry.businessTemplates.Add(cafeTemplate);

        // Bar
        BusinessTemplate barTemplate = new BusinessTemplate();
        barTemplate.templateName = "Bar";
        barTemplate.description = "A lively bar offering drinks and entertainment.";
        barTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(barTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(barTemplate, "Bartender", "Charisma", 55f);
        hospitalityIndustry.businessTemplates.Add(barTemplate);
        industries.Add(hospitalityIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + hospitalityIndustry.industryName);

        // ---- Manufacturing Industry ----
        Industry manufacturingIndustry = new Industry();
        manufacturingIndustry.industryName = "Manufacturing";
        manufacturingIndustry.businessTemplates = new List<BusinessTemplate>();

        // Factory
        BusinessTemplate factoryTemplate = new BusinessTemplate();
        factoryTemplate.templateName = "Factory";
        factoryTemplate.description = "A large-scale manufacturing facility producing goods.";
        factoryTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(factoryTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(factoryTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(factoryTemplate, "Worker", "Craftsmanship", 55f);
        manufacturingIndustry.businessTemplates.Add(factoryTemplate);

        // Assembly Plant
        BusinessTemplate assemblyTemplate = new BusinessTemplate();
        assemblyTemplate.templateName = "Assembly Plant";
        assemblyTemplate.description = "A plant specializing in assembling products from parts.";
        assemblyTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(assemblyTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(assemblyTemplate, "Worker", "Craftsmanship", 50f);
        manufacturingIndustry.businessTemplates.Add(assemblyTemplate);

        // Industrial Workshop
        BusinessTemplate workshopTemplate = new BusinessTemplate();
        workshopTemplate.templateName = "Industrial Workshop";
        workshopTemplate.description = "A smaller manufacturing unit focused on custom orders.";
        workshopTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(workshopTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(workshopTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(workshopTemplate, "Worker", "Craftsmanship", 50f);
        manufacturingIndustry.businessTemplates.Add(workshopTemplate);
        industries.Add(manufacturingIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + manufacturingIndustry.industryName);

        // ---- Entertainment Industry ----
        Industry entertainmentIndustry = new Industry();
        entertainmentIndustry.industryName = "Entertainment";
        entertainmentIndustry.businessTemplates = new List<BusinessTemplate>();

        // Movie Theater
        BusinessTemplate theaterTemplate = new BusinessTemplate();
        theaterTemplate.templateName = "Movie Theater";
        theaterTemplate.description = "A venue for screening films and live events.";
        theaterTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(theaterTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(theaterTemplate, "Ticket Seller", "Linguistics", 50f);
        AddPositionToTemplate(theaterTemplate, "Worker", "Craftsmanship", 45f);
        entertainmentIndustry.businessTemplates.Add(theaterTemplate);

        // Concert Venue
        BusinessTemplate concertTemplate = new BusinessTemplate();
        concertTemplate.templateName = "Concert Venue";
        concertTemplate.description = "A facility for hosting live music events.";
        concertTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(concertTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(concertTemplate, "Security", "Athletics", 50f);
        entertainmentIndustry.businessTemplates.Add(concertTemplate);

        // Arcade
        BusinessTemplate arcadeTemplate = new BusinessTemplate();
        arcadeTemplate.templateName = "Arcade";
        arcadeTemplate.description = "A fun venue featuring a variety of arcade games.";
        arcadeTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(arcadeTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(arcadeTemplate, "Worker", "Craftsmanship", 50f);
        entertainmentIndustry.businessTemplates.Add(arcadeTemplate);
        industries.Add(entertainmentIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + entertainmentIndustry.industryName);

        // ---- Transportation Industry ----
        Industry transportationIndustry = new Industry();
        transportationIndustry.industryName = "Transportation";
        transportationIndustry.businessTemplates = new List<BusinessTemplate>();

        // Taxi Company
        BusinessTemplate taxiTemplate = new BusinessTemplate();
        taxiTemplate.templateName = "Taxi Company";
        taxiTemplate.description = "A company offering taxi services throughout the city.";
        taxiTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(taxiTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(taxiTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(taxiTemplate, "Driver", "Athletics", 50f);
        transportationIndustry.businessTemplates.Add(taxiTemplate);

        // Bus Transit
        BusinessTemplate busTemplate = new BusinessTemplate();
        busTemplate.templateName = "Bus Transit";
        busTemplate.description = "A public transportation company operating bus routes.";
        busTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(busTemplate, "Manager", "Logic", 60f);
        AddPositionToTemplate(busTemplate, "Driver", "Athletics", 50f);
        transportationIndustry.businessTemplates.Add(busTemplate);

        // Logistics Company
        BusinessTemplate logisticsTemplate = new BusinessTemplate();
        logisticsTemplate.templateName = "Logistics Company";
        logisticsTemplate.description = "A firm managing the transport of goods and warehousing.";
        logisticsTemplate.defaultPositions = new List<FactionGenerator.CompanyPosition>();
        AddPositionToTemplate(logisticsTemplate, "Boss", "Charisma", 70f);
        AddPositionToTemplate(logisticsTemplate, "Manager", "Logic", 65f);
        AddPositionToTemplate(logisticsTemplate, "Worker", "Craftsmanship", 55f);
        transportationIndustry.businessTemplates.Add(logisticsTemplate);
        industries.Add(transportationIndustry);
        Debug.Log("[BusinessInfoGenerator] Initialized default industry: " + transportationIndustry.industryName);
    }

}
