﻿using Fluxor;

namespace MuensterData.Domain.Traffic.States;

public record TrafficState(Accidents Accidents);

public class TrafficFeatureState : Feature<TrafficState>
{
    public override string GetName()
    {
        return nameof(TrafficState);
    }

    protected override TrafficState GetInitialState()
    {
        LightCondition[] lightConditions =
        [
            new("Tageslicht", 0),
            new("Dämmerung", 1),
            new("Dunkelheit", 2)
        ];

        return new TrafficState(
            new Accidents(
                [],
                [],
                new AllAccidentFilters(
                    [],
                    lightConditions
                ),
                new ActiveAccidentFilters(
                    [],
                    [lightConditions[0].Value],
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                ),
                new MapSettings(0.4)
            )
        );
    }
}