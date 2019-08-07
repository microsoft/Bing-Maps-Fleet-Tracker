// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { AvoidTypes, OptimizeValue, HazardousMaterial } from './dispatching-parameters';

export const AvoidOptions = [
    { name: 'Highways', value: AvoidTypes.Highways, isChecked: false, disabled: true },
    { name: 'Tolls', value: AvoidTypes.Tolls, isChecked: false, disabled: true }
];

export const MinimizeOptions = [
    { name: 'Highways', value: AvoidTypes.MinimizeHighways, isChecked: false, disabled: true },
    { name: 'Tolls', value: AvoidTypes.MinimizeTolls, isChecked: false, disabled: true }
];

export const OptimizeOptions = [
    { name: 'Time', value: OptimizeValue.Time },
    { name: 'Time considering traffic', value: OptimizeValue.TimeWithTraffic },
];

export const HazardousMaterialOptions = [
    { name: 'Explosive', value: HazardousMaterial.Explosive, isChecked: false },
    { name: 'Gas', value: HazardousMaterial.Gas, isChecked: false },
    { name: 'Flammable', value: HazardousMaterial.Flammable, isChecked: false },
    { name: 'Combustable', value: HazardousMaterial.Combustable, isChecked: false },
    { name: 'Flammable Solid', value: HazardousMaterial.FlammableSolid, isChecked: false },
    { name: 'Organic', value: HazardousMaterial.Organic, isChecked: false },
    { name: 'Poison', value: HazardousMaterial.Poison, isChecked: false },
    { name: 'Radioactive', value: HazardousMaterial.RadioActive, isChecked: false },
    { name: 'Corrosive', value: HazardousMaterial.Corrosive, isChecked: false },
    { name: 'Poisonous Inhalation', value: HazardousMaterial.PoisonousInhalation, isChecked: false },
    { name: 'Goods Harmful to Water', value: HazardousMaterial.GoodsHarmfulToWater, isChecked: false },
    { name: 'Other', value: HazardousMaterial.Other, isChecked: false }
];

export const HazardousPermitOptions = [
    { name: 'Explosive', value: HazardousMaterial.Explosive, isChecked: false },
    { name: 'Gas', value: HazardousMaterial.Gas, isChecked: false },
    { name: 'Flammable', value: HazardousMaterial.Flammable, isChecked: false },
    { name: 'Combustable', value: HazardousMaterial.Combustable, isChecked: false },
    { name: 'Flammable Solid', value: HazardousMaterial.FlammableSolid, isChecked: false },
    { name: 'Organic', value: HazardousMaterial.Organic, isChecked: false },
    { name: 'Poison', value: HazardousMaterial.Poison, isChecked: false },
    { name: 'Radioactive', value: HazardousMaterial.RadioActive, isChecked: false },
    { name: 'Corrosive', value: HazardousMaterial.Corrosive, isChecked: false },
    { name: 'Poisonous Inhalation', value: HazardousMaterial.PoisonousInhalation, isChecked: false },
    { name: 'All Appropriate For Load', value: HazardousMaterial.AllApproppriateForLoad, isChecked: false }
];

