using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Debug functions that were removed from the game</summary>
public static class SrDebugExtensions
{
    public static BindingFlags all = BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Instance
        | BindingFlags.Static
        | BindingFlags.GetField
        | BindingFlags.SetField
        | BindingFlags.GetProperty
        | BindingFlags.SetProperty;

    /// <summary>Retrieves a field/property (which can be private, but doesn't have to be) 
	/// of type T2 from a class instance of type T1</summary>
    /// <param name="fieldName">The name of the field/property to retrieve</param>
    /// <param name="instance">The instance to retrieve the field/property from</param>
    private static T2 GetFieldOrPropertyValue<T1, T2>(string fieldName, T1 instance)
    {
        var field = typeof(T1).GetField(fieldName, all);
        if (field != null)
            return (T2)field?.GetValue(instance);

        var property = typeof(T1).GetProperty(fieldName, all);
        return (T2)property?.GetValue(instance, null);
    }

    // PediaDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears unlocked pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugClearUnlocked(this PediaDirector self)
    {
        var unlock = typeof(PediaDirector).GetMethod("Unlock", all);
        GetFieldOrPropertyValue<PediaDirector, HashSet<PediaDirector.Id>>("unlocked", self).Clear();
        foreach (var t in self.initUnlocked)
            unlock.Invoke(self, new object[] { t });
    }

    /// <summary>Unlocks all pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugAllUnlocked(this PediaDirector self)
    {
        var unlock = typeof(PediaDirector).GetMethod("Unlock", all);
        foreach (PediaDirector.Id id in Enum.GetValues(typeof(PediaDirector.Id)))
            unlock.Invoke(self, new object[] { id });
    }

    // TutorialDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all completed tutorials</summary>
    /// <param name="self">The TutorialDirector instance</param>
    public static void DebugClearCompleted(this TutorialDirector self)
    {
        GetFieldOrPropertyValue<TutorialDirector, HashSet<TutorialDirector.Id>>("completed", self).Clear();
    }

    /// <summary>Completes all tutorials</summary>
    /// <param name="self">The TutorialDirector instance</param>
    public static void DebugAllCompleted(this TutorialDirector self)
    {
        var completed = GetFieldOrPropertyValue<TutorialDirector, HashSet<TutorialDirector.Id>>("completed", self);
        foreach (TutorialDirector.Id id in Enum.GetValues(typeof(TutorialDirector.Id))) completed.Add(id);
    }

    // AchievementsDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all awarded achievements</summary>
    /// <param name="self">The AchievementsDirector instance</param>
    public static void DebugClearAwarded(this AchievementsDirector self)
    {
        GetFieldOrPropertyValue<AchievementsDirector, HashSet<AchievementsDirector.Achievement>>("earnedAchievements", self).Clear();
        GetFieldOrPropertyValue<AchievementsDirector, Dictionary<AchievementsDirector.BoolStat, bool>>("boolStatDict", self).Clear();
        GetFieldOrPropertyValue<AchievementsDirector, Dictionary<AchievementsDirector.IntStat, int>>("intStatDict", self).Clear();
        GetFieldOrPropertyValue<AchievementsDirector, Dictionary<AchievementsDirector.EnumStat, HashSet<Enum>>>("enumStatDict", self).Clear();
        GetFieldOrPropertyValue<AchievementsDirector, Dictionary<AchievementsDirector.GameFloatStat, float>>("gameFloatStatDict", self).Clear();
    }

    /// <summary>Awards all achievements</summary>
    /// <param name="self">The AchievementsDirector instance</param>
    public static void DebugAllAwarded(this AchievementsDirector self)
    {
        var earnedAchievements = GetFieldOrPropertyValue<AchievementsDirector, HashSet<AchievementsDirector.Achievement>>("earnedAchievements", self);
        foreach (AchievementsDirector.Achievement achievement in Enum.GetValues(typeof(AchievementsDirector.Achievement))) earnedAchievements.Add(achievement);
    }

    // ProgressDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all progress</summary>
    /// <param name="self">The ProgressDirector instance</param>
    public static void DebugClearProgress(this ProgressDirector self)
    {
        GetFieldOrPropertyValue<ProgressDirector, Dictionary<ProgressDirector.ProgressType, int>>("progressDict", self).Clear();
    }

    /// <summary>Unlocks all progress</summary>
    /// <param name="self">The ProgressDirector instance</param>
    public static void DebugUnlockProgress(this ProgressDirector self)
    {
        foreach (ProgressDirector.ProgressType type in
            Enum.GetValues(typeof(ProgressDirector.ProgressType)))
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case ProgressDirector.ProgressType.CORPORATE_PARTNER:
                    self.SetProgress(type, 28);
                    break;
                default:
                    self.SetProgress(type, 1);
                    break;
            }
        }
    }

    // PlayerState
    // --------------------------------------------------------------------------------
    
    /// <summary>Gives the player all upgrades</summary>
    /// <param name="self">The PlayerState instance</param>
    public static void DebugGiveAllUpgrades(this PlayerState self)
    {
        foreach (PlayerState.Upgrade upgrade in Enum.GetValues(typeof(PlayerState.Upgrade)))
            self.AddUpgrade(upgrade);
    }

    // Ammo
    // --------------------------------------------------------------------------------
    
    /// <summary>Fills the player's inventory with random items</summary>
    /// <param name="self">The Ammo instance</param>
    /// <param name="fillTo"></param>
    public static void DebugFillRandomAmmo(this Ammo self)
    {
        var potentialAmmo = GetFieldOrPropertyValue<Ammo, GameObject[]>("potentialAmmo", self);
        var numSlots = GetFieldOrPropertyValue<Ammo, int>("numSlots", self);

        var ammoSlot = typeof(Ammo).GetNestedType("Slot", all);
        var slots = GetFieldOrPropertyValue<Ammo, object[]>("slots", self);
        var emotions = ammoSlot.GetField("emotions", all);

        for (var i = 0; i < numSlots; i++)
        {
            int fillTo = self.GetSlotMaxCount(i);

            // pick a random item to insert into the slot
            var plucked = Randoms.SHARED.Pluck(new List<GameObject>(potentialAmmo), null);
            
            // instantiate ammo slot
            var constructor = ammoSlot.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            if (parameters[0].ParameterType == typeof(GameObject))
                slots[i] = Activator.CreateInstance(ammoSlot, plucked, fillTo);
            else if (parameters[0].ParameterType == typeof(Identifiable.Id)) // 0.6.0+
                slots[i] = Activator.CreateInstance(ammoSlot, plucked.GetComponent<Identifiable>().id, fillTo);
            
            if (!Identifiable.IsSlime(plucked.GetComponent<Identifiable>().id)) continue;
            var emotionData = new SlimeEmotionData
            {
                [SlimeEmotions.Emotion.AGITATION] = 0,
                [SlimeEmotions.Emotion.HUNGER] = .5f,
                [SlimeEmotions.Emotion.FEAR] = 0,
            };
            emotions?.SetValue(slots[i], emotionData);
        }

        // Refill Ammo again, because some mods might change the max value depending on what is in the slot
        DebugRefillAmmo(self);
    }

    /// <summary>Refills the player's inventory</summary>
    /// <param name="self">The Ammo instance</param>
    /// <param name="fillTo"></param>
    public static void DebugRefillAmmo(this Ammo self)
    {
        var ammoSlot = typeof(Ammo).GetNestedType("Slot", all);
        var slotCount = ammoSlot.GetField("count");
        var numSlots = GetFieldOrPropertyValue<Ammo, int>("numSlots", self);
        var slots = GetFieldOrPropertyValue<Ammo, object[]>("slots", self);

        for (var i = 0; i < numSlots; i++)
        {
            int fillTo = self.GetSlotMaxCount(i);

            if (slots[i] != null)
                slotCount?.SetValue(slots[i], fillTo);
            else if (i == numSlots - 1) // refill water if slot empty
                slots[i] = Activator.CreateInstance(ammoSlot, Identifiable.Id.WATER_LIQUID, fillTo);
        }
    }
}
