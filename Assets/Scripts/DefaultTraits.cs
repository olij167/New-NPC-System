using System.Collections.Generic;
using UnityEngine;

public static class DefaultTraits
{
    public static List<Trait> GetDefaultTraits()
    {
        List<Trait> traits = new List<Trait>();

        Trait skilled = new Trait();
        skilled.traitName = "Skilled";
        skilled.description = "High proficiency in relevant skills.";
        skilled.category = "Skill";
        skilled.modifiers.Add("SkillModifier", 0.2f);

        Trait incompetent = new Trait();
        incompetent.traitName = "Incompetent";
        incompetent.description = "Low proficiency in relevant skills.";
        incompetent.category = "Skill";
        incompetent.modifiers.Add("SkillModifier", -0.2f);

        Trait quickLearner = new Trait();
        quickLearner.traitName = "Quick Learner";
        quickLearner.description = "Learns skills at a faster rate.";
        quickLearner.category = "Skill";
        quickLearner.modifiers.Add("LearningRate", 0.3f);

        Trait slowLearner = new Trait();
        slowLearner.traitName = "Slow Learner";
        slowLearner.description = "Learns skills at a slower rate.";
        slowLearner.category = "Skill";
        slowLearner.modifiers.Add("LearningRate", -0.3f);

        Trait brave = new Trait();
        brave.traitName = "Brave";
        brave.description = "Faces threats head on.";
        brave.category = "Behavior";
        brave.modifiers.Add("FearResponse", -0.2f);

        Trait skittish = new Trait();
        skittish.traitName = "Skittish";
        skittish.description = "Tends to flee from threats.";
        skittish.category = "Behavior";
        skittish.modifiers.Add("FearResponse", 0.2f);

        Trait adventurous = new Trait();
        adventurous.traitName = "Adventurous";
        adventurous.description = "Eager to explore and take risks.";
        adventurous.category = "Behavior";
        adventurous.modifiers.Add("RiskTaking", 0.3f);

        Trait cautious = new Trait();
        cautious.traitName = "Cautious";
        cautious.description = "Avoids unnecessary risks.";
        cautious.category = "Behavior";
        cautious.modifiers.Add("RiskTaking", -0.3f);

        Trait bipolar = new Trait();
        bipolar.traitName = "Bipolar";
        bipolar.description = "Experiences mood swings.";
        bipolar.category = "Mental";
        bipolar.modifiers.Add("MoodStability", -0.4f);

        Trait narcoleptic = new Trait();
        narcoleptic.traitName = "Narcoleptic";
        narcoleptic.description = "May fall asleep unexpectedly.";
        narcoleptic.category = "Mental";
        narcoleptic.modifiers.Add("Alertness", -0.3f);

        Trait insomniac = new Trait();
        insomniac.traitName = "Insomniac";
        insomniac.description = "Struggles with sleep.";
        insomniac.category = "Mental";
        insomniac.modifiers.Add("Restfulness", -0.3f);

        Trait empathetic = new Trait();
        empathetic.traitName = "Empathetic";
        empathetic.description = "Sensitive to others' feelings.";
        empathetic.category = "Social";
        empathetic.modifiers.Add("Empathy", 0.3f);

        Trait aloof = new Trait();
        aloof.traitName = "Aloof";
        aloof.description = "Emotionally distant.";
        aloof.category = "Social";
        aloof.modifiers.Add("Empathy", -0.3f);

        Trait charismatic = new Trait();
        charismatic.traitName = "Charismatic";
        charismatic.description = "Influential and charming.";
        charismatic.category = "Social";
        charismatic.modifiers.Add("Charisma", 0.3f);

        Trait aggressive = new Trait();
        aggressive.traitName = "Aggressive";
        aggressive.description = "Prone to confrontations.";
        aggressive.category = "Behavior";
        aggressive.modifiers.Add("Aggression", 0.3f);

        Trait energetic = new Trait();
        energetic.traitName = "Energetic";
        energetic.description = "Highly active.";
        energetic.category = "Behavior";
        energetic.modifiers.Add("Activity", 0.3f);

        Trait lethargic = new Trait();
        lethargic.traitName = "Lethargic";
        lethargic.description = "Low energy.";
        lethargic.category = "Behavior";
        lethargic.modifiers.Add("Activity", -0.3f);

        Trait analytical = new Trait();
        analytical.traitName = "Analytical";
        analytical.description = "Approaches problems logically.";
        analytical.category = "Cognitive";
        analytical.modifiers.Add("Logic", 0.3f);

        Trait creative = new Trait();
        creative.traitName = "Creative";
        creative.description = "Innovative and imaginative.";
        creative.category = "Cognitive";
        creative.modifiers.Add("Innovation", 0.3f);

        Trait impulsive = new Trait();
        impulsive.traitName = "Impulsive";
        impulsive.description = "Acts without much forethought.";
        impulsive.category = "Behavior";
        impulsive.modifiers.Add("Impulsiveness", 0.3f);

        Trait methodical = new Trait();
        methodical.traitName = "Methodical";
        methodical.description = "Takes a systematic approach.";
        methodical.category = "Behavior";
        methodical.modifiers.Add("Impulsiveness", -0.3f);

        Trait keenEyed = new Trait();
        keenEyed.traitName = "Keen-eyed";
        keenEyed.description = "Exceptional vision.";
        keenEyed.category = "Perception";
        keenEyed.modifiers.Add("SightMultiplier", 0.2f);

        Trait poorSighted = new Trait();
        poorSighted.traitName = "Poor-sighted";
        poorSighted.description = "Diminished vision.";
        poorSighted.category = "Perception";
        poorSighted.modifiers.Add("SightMultiplier", -0.2f);

        Trait sensitive = new Trait();
        sensitive.traitName = "Sensitive";
        sensitive.description = "Highly responsive to stimuli.";
        sensitive.category = "Perception";
        sensitive.modifiers.Add("HearingMultiplier", 0.2f);

        Trait hardOfHearing = new Trait();
        hardOfHearing.traitName = "Hard of Hearing";
        hardOfHearing.description = "Reduced auditory perception.";
        hardOfHearing.category = "Perception";
        hardOfHearing.modifiers.Add("HearingMultiplier", -0.2f);

        traits.Add(skilled);
        traits.Add(incompetent);
        traits.Add(quickLearner);
        traits.Add(slowLearner);
        traits.Add(brave);
        traits.Add(skittish);
        traits.Add(adventurous);
        traits.Add(cautious);
        traits.Add(bipolar);
        traits.Add(narcoleptic);
        traits.Add(insomniac);
        traits.Add(empathetic);
        traits.Add(aloof);
        traits.Add(charismatic);
        traits.Add(aggressive);
        traits.Add(energetic);
        traits.Add(lethargic);
        traits.Add(analytical);
        traits.Add(creative);
        traits.Add(impulsive);
        traits.Add(methodical);
        traits.Add(keenEyed);
        traits.Add(poorSighted);
        traits.Add(sensitive);
        traits.Add(hardOfHearing);

        return traits;
    }
}
