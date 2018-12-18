import { Difficulty } from "src/app/enums/difficulty";
import { InjectionToken } from "@angular/core";

export interface IBrickDecorationService {

    getBrickMarginLength(difficultyType: Difficulty): number;

    getBrickSideLength(difficultyType: Difficulty): number;

    getBoundaryBrickSpaceLength(difficultyType: Difficulty): number;

    getBoundaryLength(difficulty: Difficulty): number;
}

export let IBrickDecorationServiceToken: InjectionToken<IBrickDecorationService> =
    new InjectionToken<IBrickDecorationService>('IBrickDecorationServiceToken');