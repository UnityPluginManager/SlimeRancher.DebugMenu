using System;
using System.Reflection;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using SrDebugDirector;
/// <summary>Debug functions that were removed from the game</summary>
public static class SrDebugExtensions
{
    private static BindingFlags all = BindingFlags.Public
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
    

    //TimeDirector
    // --------------------------------------------------------------------------------

    /// <summary>Adjusting Time of Day</summary>
    /// <param name="self">The TimeDirector instance</param>
    /// <param name="byDayFraction">byDayFraction(Idk)</param>
    public static void AdjustTimeOfDay(this TimeDirector self,float byDayFraction)
    {
        self.worldModel.worldTime += byDayFraction * 86400f;
    }

    /// <summary>Setting a World Time</summary>
    /// <param name="self">The TimeDirector instance</param>
    /// <param name="worldTime">Setting a worldtime</param>
    public static void SetWorldTime(this TimeDirector self, double worldTime)
    {
        self.worldModel.worldTime = worldTime;
        self.worldModel.lastWorldTime = worldTime;
    }
    // PediaDirector
    // --------------------------------------------------------------------------------

    /// <summary>Clears unlocked pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugClearUnlocked(this PediaDirector self)
    {
        self.pediaModel.unlocked.Clear();
    }

    /// <summary>Unlocks all pedia entries</summary>
    /// <param name="self">The PediaDirector instance</param>
    public static void DebugAllUnlocked(this PediaDirector self)
    {
        var unlock = typeof(PediaDirector).GetMethod("Unlock", all);
        PediaDirector.Id[] id = (PediaDirector.Id[]) Enum.GetValues(typeof(PediaDirector.Id));

        if (unlock is not null) unlock.Invoke(self, new object[] {id});
    }

    // TutorialDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all completed tutorials</summary>
    public static void DebugClearCompleted(this TutorialDirector self)
    {
        self.tutModel.completedIds.Clear();

    }

    /// <summary>Completes all tutorials</summary>
    /// <param name="self">The TutorialDirector instance</param>
    public static void DebugAllCompleted(this TutorialDirector self)
    {
        foreach (TutorialDirector.Id id in Enum.GetValues(typeof(TutorialDirector.Id)))
        {
            self.tutModel.completedIds.Add(id);

        }
        
    }

    // AchievementsDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all awarded achievements</summary>
    /// <param name="self">The AchievementsDirector instance</param>
    public static void DebugClearAwarded(this AchievementsDirector self)
    {
        var unlock = typeof(ProfileAchievesModel).GetMethod("Reset", all);

        if (unlock is not null) unlock.Invoke(self.profileAchievesModel, new object[] { });
    }
    

    // ProgressDirector
    // --------------------------------------------------------------------------------
    
    /// <summary>Clears all progress</summary>
    /// <param name="self">The ProgressDirector instance</param>
    public static void DebugClearProgress(this ProgressDirector self)
    {
        self.model.progressDict.Clear();
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
    public static void DebugFillRandomAmmo(this Ammo self)
    {
        var potentialAmmo = self.potentialAmmo;
        var numSlots = self.numSlots;
        var ammoSlot = typeof(Ammo).GetNestedType("Slot", all);
        var slots = self.Slots;
        var emotions = ammoSlot.GetField("emotions", all);
        for (var i = 0; i < numSlots; i++)
        {
            int fillTo = self.GetSlotMaxCount(i);
            
            // pick a random item to insert into the slot

            var plucked = Randoms.SHARED.Pluck(new HashSet<Identifiable.Id>(potentialAmmo), Identifiable.Id.NONE);
            
            // instantiate ammo slot
            var constructor = ammoSlot.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            if (parameters[0].ParameterType == typeof(Identifiable.Id))
            {
                slots[i] = (Ammo.Slot) Activator.CreateInstance(ammoSlot, plucked, fillTo);
            }

            if (!Identifiable.IsSlime(plucked)) continue;
            var emotiondata = new SlimeEmotionData
            {
                [SlimeEmotions.Emotion.AGITATION] = 0,
                [SlimeEmotions.Emotion.HUNGER] = .5f,
                [SlimeEmotions.Emotion.FEAR] = 0,

            };
            emotions?.SetValue(slots[i], emotiondata);
        }
        // Refill Ammo again, because some mods might change the max value depending on what is in the slot
        DebugRefillAmmo(self);
    }


    /// <summary>Refills the player's inventory</summary>
    /// <param name="self">The Ammo instance</param>
    public static void DebugRefillAmmo(this Ammo self)
    {
        var ammoSlot = typeof(Ammo).GetNestedType("Slot", all);
        var slotCount = ammoSlot.GetField("count");
        var numSlots = self.numSlots;
        var slots = self.Slots;


        for (var i = 0; i < numSlots; i++)
        {
            int fillTo = self.GetSlotMaxCount(i);

            if (slots[i] != null)
                slotCount?.SetValue(slots[i], fillTo);
            else if (i == numSlots - 1) // refill water if slot empty
                slots[i] = (Ammo.Slot) Activator.CreateInstance(ammoSlot, Identifiable.Id.WATER_LIQUID, fillTo);
        }
    }
}
