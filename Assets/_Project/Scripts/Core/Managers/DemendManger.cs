using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DemendManger : MonoBehaviour
{
    public event Action<DemandData, int> OnDemandDisplayRequested;
    public event Action OnDemandCompleted;

    [SerializeField]private List<DemandData> demands = new List<DemandData>();
    [SerializeField] private MainStorrage mainStorrage;
    [SerializeField] private BarometerManager barometerManager;
    private DemandData currentDemend;
    private int nextDemandIndex;
    private int dueRound;
    private int latestRound;

    public void TriggerDemand(DemandData demand, int currentRound) //Erhält die aktuelle Forderung, berechnet die Deadline und setzt dann den Index hoch.
    {
        if (currentDemend != null)
        {
            return;
        }

        currentDemend = demand;
        dueRound = currentRound + demand.deadline;
        nextDemandIndex++;

        OnDemandDisplayRequested?.Invoke(demand, currentRound);
    }

    public void CheckDemandsForRound(int currentRound) //Prüfen ob schoon eine Forderung aktiv und ob in der Liste noch eine ist. Wenn ja Demand Triggern
    {
        latestRound = currentRound;

        if (currentDemend != null)
        {
            if (currentRound >= dueRound)
            {
                barometerManager.ChangeValue(currentDemend.changeInValueNegative);
                CompleteDemand();
            }

            return;
        }

        if (nextDemandIndex >= demands.Count)
        {
            return;
        }

        DemandData nextDemand = demands[nextDemandIndex];

        if (currentRound < nextDemand.triggerRound)
        {
            return;
        }

        TriggerDemand(nextDemand, currentRound);
    }

    public void GiveAll()
    {

        if (CanGiveAll() == true)
        {
            mainStorrage.RemoveResource(currentDemend.quantity, currentDemend.resource);
            barometerManager.ChangeValue(currentDemend.changeInValuePositive);
            CompleteDemand();
        }
    }

    public void GiveHalf()
    {
        if (CanGiveHalf() == true)
        {
            mainStorrage.RemoveResource(currentDemend.quantity / 2, currentDemend.resource);
            barometerManager.ChangeValue(currentDemend.changeInValueHalf);
            CompleteDemand();
        }
    }

    public void MoreTime()
    {
        if (currentDemend == null)
        {
            return;
        }

        dueRound += currentDemend.deferralPeriod;
        barometerManager.ChangeValue(currentDemend.changeInDeferalInValue);
    }

    public bool HasActiveDemand()
    {
        return currentDemend != null;
    }

    public bool CanGiveAll()
    {
        return currentDemend != null && mainStorrage.HasResources(currentDemend.quantity, currentDemend.resource);
    }

    public bool CanGiveHalf()
    {
        return currentDemend != null && mainStorrage.HasResources(currentDemend.quantity / 2, currentDemend.resource);
    }

    private void CompleteDemand()
    {
        currentDemend = null;
        OnDemandCompleted?.Invoke();
    }

    public int GetRemainingRounds()
    {
        return Mathf.Max(0, dueRound - latestRound);
    }
}
