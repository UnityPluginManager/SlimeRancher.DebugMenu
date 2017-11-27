using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Debug functions that were removed from the game</summary>
public static class SrDebugExtensions
{
    /// <summary>Retrieves a private field of type T2 from a class instance of type T1</summary>
    /// <param name="fieldName">The name of the field to retrieve</param>
    /// <param name="instance">The instance to retrieve the field from</param>
    private static T2 GetPrivateField<T1, T2>(string fieldName, T1 instance)
    {
        var field = typeof(T1).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T2) field?.GetValue(instance);
    }

    // PediaDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears unlocked pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugClearUnlocked(this PediaDirector self)
    {
        var unlock = typeof(PediaDirector).GetMethod("Unlock", BindingFlags.NonPublic | BindingFlags.Instance);
        GetPrivateField<PediaDirector, HashSet<PediaDirector.Id>>("unlocked", self).Clear();
        foreach (var t in self.initUnlocked)
            unlock.Invoke(self, new object[] { t });
    }

    /// <summary>Unlocks all pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugAllUnlocked(this PediaDirector self)
    {
        var unlock = typeof(PediaDirector).GetMethod("Unlock", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (PediaDirector.Id id in Enum.GetValues(typeof(PediaDirector.Id)))
            unlock.Invoke(self, new object[] { id });
    }

    // TutorialDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all completed tutorials</summary>
    /// <param name="self">The TutorialDirector instance</param>
    public static void DebugClearCompleted(this TutorialDirector self)
    {
        GetPrivateField<TutorialDirector, HashSet<TutorialDirector.Id>>("completed", self).Clear();
    }

    /// <summary>Completes all tutorials</summary>
    /// <param name="self">The TutorialDirector instance</param>
    public static void DebugAllCompleted(this TutorialDirector self)
    {
        var completed = GetPrivateField<TutorialDirector, HashSet<TutorialDirector.Id>>("completed", self);
        foreach (TutorialDirector.Id id in Enum.GetValues(typeof(TutorialDirector.Id))) completed.Add(id);
    }

    // AchievementsDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all awarded achievements</summary>
    /// <param name="self">The AchievementsDirector instance</param>
    public static void DebugClearAwarded(this AchievementsDirector self)
    {
        GetPrivateField<AchievementsDirector, HashSet<AchievementsDirector.Achievement>>("earnedAchievements", self).Clear();
        GetPrivateField<AchievementsDirector, Dictionary<AchievementsDirector.BoolStat, bool>>("boolStatDict", self).Clear();
        GetPrivateField<AchievementsDirector, Dictionary<AchievementsDirector.IntStat, int>>("intStatDict", self).Clear();
        GetPrivateField<AchievementsDirector, Dictionary<AchievementsDirector.EnumStat, HashSet<Enum>>>("enumStatDict", self).Clear();
        GetPrivateField<AchievementsDirector, Dictionary<AchievementsDirector.GameFloatStat, float>>("gameFloatStatDict", self).Clear();
    }

    /// <summary>Awards all achievements</summary>
    /// <param name="self">The AchievementsDirector instance</param>
    public static void DebugAllAwarded(this AchievementsDirector self)
    {
        var earnedAchievements = GetPrivateField<AchievementsDirector, HashSet<AchievementsDirector.Achievement>>("earnedAchievements", self);
        foreach (AchievementsDirector.Achievement achievement in Enum.GetValues(typeof(AchievementsDirector.Achievement))) earnedAchievements.Add(achievement);
    }

    // ProgressDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all progress</summary>
    /// <param name="self">The ProgressDirector instance</param>
    public static void DebugClearProgress(this ProgressDirector self)
    {
        GetPrivateField<ProgressDirector, Dictionary<ProgressDirector.ProgressType, int>>("progressDict", self).Clear();
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
    public static void DebugFillRandomAmmo(this Ammo self, int fillTo)
    {
        var potentialAmmo = GetPrivateField<Ammo, GameObject[]>("potentialAmmo", self);
        var numSlots = GetPrivateField<Ammo, int>("numSlots", self);

        var ammoSlot = typeof(Ammo).GetNestedType("Slot", BindingFlags.NonPublic | BindingFlags.Instance);
        var slots = GetPrivateField<Ammo, object[]>("slots", self);
        var emotions = ammoSlot.GetField("emotions", BindingFlags.Public | BindingFlags.Instance);

        for (var i = 0; i < numSlots; i++)
        {
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
    }

    /// <summary>Refills the player's inventory</summary>
    /// <param name="self">The Ammo instance</param>
    /// <param name="fillTo"></param>
    public static void DebugRefillAmmo(this Ammo self, int fillTo)
    {
        var ammoSlot = typeof(Ammo).GetNestedType("Slot", BindingFlags.NonPublic | BindingFlags.Instance);
        var slotCount = ammoSlot.GetField("count");
        var numSlots = GetPrivateField<Ammo, int>("numSlots", self);
        var slots = GetPrivateField<Ammo, object[]>("slots", self);

        for (var i = 0; i < numSlots; i++)
        {
            if (slots[i] != null)
                slotCount?.SetValue(slots[i], fillTo);
            else if (i == numSlots - 1) // refill water if slot empty
                slots[i] = Activator.CreateInstance(ammoSlot, Identifiable.Id.WATER_LIQUID, fillTo);
        }
    }
}
