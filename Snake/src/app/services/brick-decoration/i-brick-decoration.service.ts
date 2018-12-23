import { Difficulty } from "src/app/enums/difficulty";
import { InjectionToken } from "@angular/core";

export interface IBrickDecorationService {

    getBoundaryBrickSpaceLength(difficulty: Difficulty): number;

    getBoundaryLength(difficulty: Difficulty): number;

    getBrickMarginLength(difficulty: Difficulty): number;

    getBrickSideLength(difficulty: Difficulty): number;
}

export let IBrickDecorationServiceToken: InjectionToken<IBrickDecorationService> =
    new InjectionToken<IBrickDecorationService>('IBrickDecorationServiceToken');